using System;
using System.Collections.Generic;
using Redzen.Midi;
using Redzen.Midi.Devices;
using Redzen.Midi.NoteSets;
using Redzen.Midi.Sequencing;
using Redzen.Random;
using Redzen.Sorting;
using RedZen.Midi;
using RedzenMidiDrummer.Patterns;

namespace RedzenMidiDrummer
{
    public class DrummerConsole
    {
        #region Instance Fields

        MidiClock _midiClock;
        OutputDevice _outDev;
        Sequencer _sequencer;

        int _chan = (int)Channel.Channel10;
        int _seqLen = 16 * MidiConsts.ClocksPerBeat;
        int _quant = 12;
        double _prob = 0.4;
        int _noteId = 60;
        HashSet<int> _patternNoteSet = new HashSet<int>();

        readonly SequenceGenerator _seqGen = new SequenceGenerator();

        readonly Dictionary<string,NoteSet> _noteSetDict;

        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();

        #endregion

        #region Constructor

        public DrummerConsole()
        {
            _noteSetDict = new Dictionary<string, NoteSet>();
            foreach(NoteSet noteSet in NoteSetUtils.CreateDefaultNoteSets()) {
                _noteSetDict.Add(noteSet.Name.ToLowerInvariant(), noteSet);
            }
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            _midiClock = SelectClockSource();
            _outDev = SelectOutputDevice();
            _sequencer = new Sequencer(_midiClock, _outDev);
            _outDev.Open();
            _midiClock.Start();

            MainControlLoop();
        }

        #endregion

        #region Private Methods

        private void MainControlLoop()
        {
            PrintCommandHelp();

            for(;;)
            {
                Console.Write(">");

                // Read user keypress.
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Handle single key options.
                bool quit;
                if(HandleCmd(keyInfo, out quit))
                {
                    if(quit) return;
                    continue;
                }

                // Handle line based options.
                char ch = char.ToLowerInvariant(keyInfo.KeyChar);
                string line = ch + Console.ReadLine();
                if(!HandleCmd(line))
                {   // Invalid option, print menu.
                    Console.WriteLine($"unrecognised command: {line}");
                }
            }
        }

        #endregion

        #region Private Methods [HandleCmd - Single Key Commands]

        private bool HandleCmd(ConsoleKeyInfo keyInfo, out bool quit)
        {
            quit = false;

            char ch = char.ToLowerInvariant(keyInfo.KeyChar);
            switch(ch)
            {
                case 's':

                    if(ConsoleModifiers.Shift == keyInfo.Modifiers) {
                        HandleOption_PauseContinue();
                    } else {
                        HandleOption_StartStop();
                    }
                    return true;

                case '+':
                case '=': // = is on the same key, so let's accept that as an alias to +.
                    HandleCmd_Tempo(true);
                    return true;

                case '-':
                case '_': // _ is on the same key, so let's accept that as an alias to +.
                    HandleCmd_Tempo(false);
                    return true;

                case 'a':
                    HandleCmd_AddSequence();
                    return true;
                case 'r':
                    HandleCmd_PrintSequences();
                    return true;
                case 't':
                    HandleCmd_PrintState();
                    return true;
                case 'j':
                    Console.WriteLine("");
                    Console.WriteLine($"patternNotes = {Utils.ToString(_patternNoteSet)}");
                    return true;
                case '1':
                    return HandleCmd_AddPattern(ch);
                case '2':
                    return HandleCmd_AddPattern(ch);
                case 'h':
                    PrintCommandHelp();
                    return true;
                case 'x':
                    HandleCmd_Exit();
                    quit = true;
                    return true;
            }
            return false;
        }

        private void HandleOption_StartStop()
        {
            if(_sequencer.IsRunning)
            {
                _sequencer.Stop();
                Console.WriteLine("->stop");
            }
            else
            {
                _sequencer.Start();
                Console.WriteLine("->start");
            }
        }

        private void HandleOption_PauseContinue()
        {
            if(_sequencer.IsRunning)
            {
                _sequencer.Stop();
                Console.WriteLine("->pause");
            }
            else
            {
                _sequencer.Continue();
                Console.WriteLine("->continue");
            }
        }

        private void HandleCmd_Tempo(bool increment)
        {
            InternalMidiClock midiClock = _midiClock as InternalMidiClock;
            if(null == midiClock)
            {
                Console.WriteLine("Can't change tempo; synced to external clock.");
                return;
            }

            int bpm = midiClock.BeatsPerMin + (increment ? 1 : -1);
            bpm = Math.Min(360, Math.Max(1, bpm));
            midiClock.BeatsPerMin = bpm;
            Console.WriteLine($"bpm = {bpm}");
        }

        private void HandleCmd_AddSequence()
        {
            Sequence seq = _seqGen.CreateRandom((Channel)_chan, _seqLen / MidiConsts.ClocksPerBeat, _prob, _quant, _noteId);
            seq.InfoById.Add("q", _quant.ToString());
            seq.InfoById.Add("n", _noteId.ToString());

            _sequencer.AddSequence(seq);
            Console.WriteLine("");
            Console.WriteLine(ToString(seq, _sequencer.SequenceList.Count-1));
        }

        private void HandleCmd_PrintSequences()
        {
            Console.Clear();
            Console.WriteLine(" --- sequences ---");
            Console.WriteLine($"idx e ch    note len    quant");

            List<Sequence> seqList = _sequencer.SequenceList;
            int count = seqList.Count;

            for(int i=0; i<count; i++)
            {
                Sequence seq = seqList[i];
                Console.WriteLine(ToString(seq, i));
            }
        }

        private void HandleCmd_PrintState()
        {
            Console.Clear();
            Console.WriteLine(" --- print state ---");
            Console.WriteLine($"c[han]        = {(int)_chan + 1}");
            Console.WriteLine($"l[en]         = {_seqLen} = {_seqLen / MidiConsts.ClocksPerBeat} beats.");
            Console.WriteLine($"q[uant]       = {_quant}");
            Console.WriteLine($"n[ote]        = {_noteId}");
            Console.WriteLine($"p[robability] = 1 / {1.0 / _prob}");
        }

        private bool HandleCmd_AddPattern(char patternCh)
        {
            patternCh = char.ToLowerInvariant(patternCh);

            if(!PatternBank.PatternsById.TryGetValue(patternCh.ToString(), out Pattern pat))
            {
                Console.WriteLine("");
                Console.WriteLine($"Unrecognised pattern ID [{patternCh}]");
                return true;
            }

            if(0 == _patternNoteSet.Count)
            {
                Console.WriteLine("");
                Console.WriteLine("No pattern notes to create a pattern with.");
                return true;
            }

            List<int> noteList = new List<int>(_patternNoteSet);
            SortUtils.Shuffle(noteList, _rng);

            int noteCount = noteList.Count;

            int patIdx = 0;
            foreach(PatternSequence patSeq in pat.PatternSequenceList)
            {
                int len = Math.Max(1, (int)(_seqLen * patSeq.LengthFactor)) / MidiConsts.ClocksPerBeat;
                double p = Math.Min(1.0, Math.Max(0.0, _prob * patSeq.ProbabilityFactor));
                int q = Math.Max(1, (int)(_quant * patSeq.QuantizationFactor));
                int noteId = noteList[patIdx % noteCount];

                Sequence seq = _seqGen.CreateRandom((Channel)_chan, len, p, q, noteId);
                seq.InfoById.Add("q", q.ToString());
                seq.InfoById.Add("n", noteId.ToString());

                _sequencer.AddSequence(seq);
                Console.WriteLine(ToString(seq, _sequencer.SequenceList.Count-1));

                if(patIdx++ == noteCount) {
                    break;
                };
            }

            return true;
        }

        private void HandleCmd_Exit()
        {
            _sequencer.Stop();
            _midiClock.Stop();
            _midiClock.Dispose();
            _outDev.Dispose();
        }

        #endregion

        #region Private Methods [HandleCmd - Line Commands]

        private bool HandleCmd(string str)
        {
            if(string.IsNullOrWhiteSpace(str)) {
                return false;
            }

            string[] parts = SplitCommandString(str);
            if(2 != parts.Length) {
                return false;
            }

            switch(parts[0])
            {
                case "c":
                case "lc":
                case "lb":
                case "q":
                case "n":
                case "p":
                    return HandleCmd_NumericState(parts[0], parts[1]);
                case "ns":
                case "d":
                case "m":
                case "u":
                case "o":
                    return HandleCmd_NonNumericState(parts[0], parts[1]);
            }
            return false;
        }

        private bool HandleCmd_NumericState(string cmd, string valStr)
        {
            if(!int.TryParse(valStr, out int val)) {
                return false;
            }

            switch(cmd)
            {
                case "c":
                    if(!IsInRange(val, 1, 17))
                    {
                        Console.WriteLine($"invalid chan {val}; Range is 1-16.");
                        return true;
                    }
                    // Note. the Channel enum has Channel 1 == 0 (blame the MIDI spec!).
                    _chan = val-1;
                    return true;

                case "lc":
                    if(!IsInRange(val, 0, 100000))
                    {
                        Console.WriteLine($"invalid length {val}; Max len in clock ticks is 100,000");
                        return false;
                    }
                    _seqLen = val;
                    Console.WriteLine($"len = {_seqLen} = {_seqLen / MidiConsts.ClocksPerBeat} beats.");
                    return true;

                case "lb":
                    if(!IsInRange(val, 0, 100000))
                    {
                        Console.WriteLine($"invalid length {val}; Max len in beats is 100,000");
                        return true;
                    }
                    _seqLen = val * MidiConsts.ClocksPerBeat;
                    Console.WriteLine($"len = {_seqLen} = {val} beats.");
                    return true;

                case "q":
                    if(!IsInRange(val, 1, 25))
                    {
                        Console.WriteLine($"invalid quantization {val}; Range is 1-24.");
                        return true;
                    }
                    _quant = val;
                    return true;

                case "n":
                    if(!IsInRange(val, 0, 128))
                    {
                        Console.WriteLine($"invalid note {val}; Range is 0-127.");
                        return true;
                    }
                    _noteId = val;
                    return true;

                case "p":
                    if(!IsInRange(val, 1, 1000))
                    {
                        Console.WriteLine($"invalid probability {val}; Range is 1-1000.");
                        return true;
                    }
                    _prob = 1.0 / val;
                    return true;
            }
            return false;
        }

        private bool HandleCmd_NonNumericState(string cmd, string valStr)
        {
            switch(cmd)
            {
                case "ns":
                { 
                    HashSet<int> noteIdSet = Utils.ParsePatternNotesDescriptor(valStr, _noteSetDict);
                    if(0 == noteIdSet.Count) {
                        return false;
                    }

                    _patternNoteSet.Clear();
                    _patternNoteSet.UnionWith(noteIdSet);
                    Console.WriteLine($"patternNotes = {Utils.ToString(noteIdSet)}");
                    return true;
                }
                case "d":
                { 
                    int[] idxArr = Utils.ParseSequenceDescriptorAsArray(valStr, _sequencer.SequenceList.Count);
                    for(int i=idxArr.Length-1; i>=0; i--) {
                        _sequencer.SequenceList.RemoveAt(idxArr[i]);
                    }
                    return true;
                }
                case "m":
                case "u":
                { 
                    bool isEnabled = ("u" == cmd);

                    int[] idxArr = Utils.ParseSequenceDescriptorAsArray(valStr, _sequencer.SequenceList.Count);
                    for(int i=idxArr.Length-1; i>=0; i--) {
                        _sequencer.SequenceList[idxArr[i]].IsEnabled = isEnabled;
                    }
                    return true;
                }
                case "o":
                {
                    HashSet<int> idxSet = Utils.ParseSequenceDescriptor(valStr, _sequencer.SequenceList.Count);
                    int count = _sequencer.SequenceList.Count;
                    for(int i=0; i<count; i++) {
                        _sequencer.SequenceList[i].IsEnabled = idxSet.Contains(i);
                    }    
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Private Methods [SelectClockSource]

        private MidiClock SelectClockSource()
        {
            for(;;)
            {
                SelectClockSource_PrintOptions();

                if(!SelectClockSource_SelectionOption(out int? bpm, out int? devIdx)) {
                    continue;
                }

                if(bpm.HasValue) {
                    return new InternalMidiClock(bpm.Value);
                }

                InputDevice inDev = InputDevices.InstalledDevices[devIdx.Value];
                return new ExternalMidiClock(inDev, true);
            }
        }

        private void SelectClockSource_PrintOptions()
        {
            Console.WriteLine("Select clock source");
            Console.WriteLine(" n: Internal clock (enter BPM)");

            char ch = 'a';
            for(int i=0; i < InputDevices.InstalledDevices.Count; i++) {
                Console.WriteLine($" {ch++}: {InputDevices.InstalledDevices[i].Name}");
            }
            Console.Write(">");
        }

        private bool SelectClockSource_SelectionOption(out int? bpm, out int? devIdx)
        {
            bpm = null;
            devIdx = null;

            // Get user input.
            string line = Console.ReadLine().Trim().ToLowerInvariant();

            // Attempt to read as BPM.
            if(int.TryParse(line, out int val))
            {
                if(val < 0 && val > 300)
                {
                    Console.WriteLine("BPM is outside of valid range [1, 300].");
                    return false;
                }
                bpm = val;
                return true;
            }

            // Attempt to read as an input device selection.
            if(1 != line.Length)
            {
                Console.WriteLine("Invalid selection.");
                return false;
            }

            int idx = line[0] - 'a';
            if(idx < 0 || idx >= InputDevices.InstalledDevices.Count)
            {
                Console.WriteLine("Invalid selection.");
                return false;
            }

            devIdx = idx;
            return true;
        }

        #endregion

        #region Private Methods [SelectOutputDevice]

        private OutputDevice SelectOutputDevice()
        {
            for(;;)
            {
                SelectOutputDevice_PrintOptions();

                if(!SelectOutputDevice_SelectionOption(out int devIdx)) {
                    continue;
                }

                OutputDevice outDev = OutputDevices.InstalledDevices[devIdx];
                return outDev;
            }
        }

        private void SelectOutputDevice_PrintOptions()
        {
            Console.WriteLine("Select output device");

            char ch = 'a';
            for(int i=0; i < OutputDevices.InstalledDevices.Count; i++) {
                Console.WriteLine($" {ch++}: {OutputDevices.InstalledDevices[i].Name}");
            }
            Console.Write(">");
        }

        private bool SelectOutputDevice_SelectionOption(out int devIdx)
        {
            devIdx = -1;

            // Get user input.
            string line = Console.ReadLine().Trim().ToLowerInvariant();

            // Attempt to read device selection.
            if(1 != line.Length)
            {
                Console.WriteLine("Invalid selection.");
                return false;
            }

            int idx = line[0] - 'a';
            if(idx < 0 || idx >= OutputDevices.InstalledDevices.Count)
            {
                Console.WriteLine("Invalid selection.");
                return false;
            }

            devIdx = idx;
            return true;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Split a command string. Commands may have two parts, the first part always consists of letters, the secondf part may be letters of numbers,
        /// and may be separated from teh first with an equals operator or a space. Here we just look for the first non-letter char, and skip any spaces or equals
        /// chars.
        /// </summary>
        /// <param name="str">The command string to split.</param>
        /// <returns>The command sub-strings.</returns>
        private string[] SplitCommandString(string str)
        {
            // Clean-up and canonicalize.
            str = str.Trim().ToLowerInvariant();

            // Scan for a non letter.
            int len = str.Length;
            int idx = 0;
            for(; idx<len; idx++)
            {
                char ch = str[idx];
                if( ch < 'a' || ch > 'z') {
                    break;
                }
            }

            if(idx == len)
            {   // No split found.
                return new string[] { str };
            }
            
            string[] parts = new string[2];
            parts[0] = str.Substring(0, idx);
            parts[1] = str.Substring(idx);

            // Remove separator chars.
            parts[1] = parts[1].Trim('=', ' ');
            return parts;
        }

        private static bool IsInRange(int v, int min, int max)
        {
            return v >= min && v < max;
        }

        private static string ToString(Sequence seq, int idx)
        {
            char en = seq.IsEnabled ? 'O' : 'X';
            int chan = (int)seq.Channel + 1;
            string n = seq.InfoById["n"];
            string q = seq.InfoById["q"];

            return $"{idx:D3} {en} ch={chan:D2} n={n:D3} l={seq.Length:D4} q={q}";
        }

        private static void PrintCommandHelp()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Red;
            //Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(@" ~~~ Redzen MIDI Sequencer Programmer ~~~ ");
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(@"");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(@" --- control ---");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" s     : start/stop (ctrl-s: pause/continue)");
            Console.WriteLine(" +/-   : tempo up/down (internal clock mode only)");
            Console.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(" --- state ---");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" c=n   : set channel");
            Console.WriteLine(" lc=n  : set sequence length (in clock ticks, 24 ticks per beat)");
            Console.WriteLine(" lb=n  : set sequence length (in beats)");
            Console.WriteLine(" q=n   : set sequence quantization (in clock ticks)");
            Console.WriteLine(" n=60  : set note (0-127)");
            Console.WriteLine(" p=5   : set note probability (stated as an inverse, i.e. 5 => 1/5th");
            Console.WriteLine(" t     : prinT state");
            Console.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(" --- sequences ---");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" a     : add new sequence");
            Console.WriteLine(" d 1   : delete sequence 1");
            Console.WriteLine(" m 1   : mute sequence 1");
            Console.WriteLine(" u 1   : un-mute sequence 1");
            Console.WriteLine(" o 1   : solo sequence 1");
            Console.WriteLine(" r     : pRint sequences");
            Console.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(" --- patterns ---");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" ns=60 : assign pattern notes (e.g. ns=60,61-63 ns=nordrum2 ns=volcabeats)");
            Console.WriteLine(" j     : print pattern notes");
            Console.WriteLine(" 1     : add pattern A");
            Console.WriteLine(" 2     : add pattern B");
            Console.WriteLine(" 3     : add pattern C");

            Console.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(" --- other ---");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" h     : help");
            Console.WriteLine(" x     : exit");
        }

        #endregion
    }
}
