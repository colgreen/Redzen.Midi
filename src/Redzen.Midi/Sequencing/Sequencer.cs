using System.Collections.Generic;
using Redzen.Midi.Devices;
using Redzen.Midi.Messages;

namespace Redzen.Midi.Sequencing
{
    public class Sequencer
    {
        #region Instance Fields

        OutputDevice _outDev;
        MidiClock _midiClock;
        List<Sequence> _seqList;
        bool _isRunning = false;

        // A list of currently playing/live notes and the duration remaining for each (in clock ticks).
        LinkedList<LiveNote> _liveNoteList = new LinkedList<LiveNote>();

        // Temp storage (to avoid reallocation of memory within HandleNoteOffMessages()).
        List<LiveNote> _noteOffList = new List<LiveNote>(100);

        object _syncObj = new object();
        int _tickCount;

        #endregion

        #region Constructor

        public Sequencer(MidiClock midiClock, OutputDevice outDev)
        {
            _midiClock = midiClock;
            _outDev = outDev;
            _seqList = new List<Sequence>(100);
            midiClock.TimingClock += MidiClock_TimingClock;
        }

        #endregion

        #region Properties

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public List<Sequence> SequenceList
        {
            get { return _seqList; }
        }

        #endregion

        #region Public Methods [Control]

        public void Start()
        {
            foreach(Sequence seq in _seqList) {
                seq.Position = 0;
            }
            _isRunning = true;

        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Continue()
        {
            _isRunning = true;
        }

        #endregion

        #region Public Methods

        public void AddSequence(Sequence seq)
        {
            if(0 == seq.Length) return;

            lock(_syncObj)
            {
                _seqList.Add(seq);

                // Assign a position that is in sync with the existing sequences.
                seq.Position = _tickCount % seq.Length;
            }
        }

        #endregion

        #region Private Methods

        private void MidiClock_TimingClock()
        {
            if(!_isRunning) {
                return;
            }

            lock(_syncObj)
            {
                // Send all note off messages scheduled for this tick.
                HandleNoteOffMessages();

                // Send all note on messages scheduled for this tick.
                HandleNoteOnMessages();

                // Update sequence position pointers.
                foreach(Sequence seq in _seqList)
                {
                    if(seq.Length == ++seq.Position) {
                        seq.Position = 0;
                    }
                }
                _tickCount++;
            }            
        }

        #endregion

        #region Private Methods

        private void HandleNoteOffMessages()
        {
            // Find all notes scheduled to be off in this timestep.
            LinkedListNode<LiveNote> currNode = _liveNoteList.First;
            while(null != currNode && 1 == currNode.Value.TicksRemaining)
            {
                _noteOffList.Add(currNode.Value);

                var removeNode = currNode;
                currNode = currNode.Next;
                _liveNoteList.Remove(removeNode);
            }

            // Decrement time remaining for all remaining live notes.
            while(null != currNode)
            {
                currNode.Value.TicksRemaining--;
                currNode = currNode.Next;
            }

            // Send all scheduled note-off messages.
            foreach(LiveNote note in _noteOffList) {
                _outDev.Send(new NoteOffMessage(note.Channel, note.NoteId, 0));
            }

            // Tidy up.
            _noteOffList.Clear();
        }

        private void HandleNoteOnMessages()
        {
            for(int i=0; i<_seqList.Count; i++) {
                HandleNoteOnMessages(_seqList[i]);
            }
        }

        private void HandleNoteOnMessages(Sequence seq)
        {
            // Send all note on messages scheduled for this tick.
            List<SequenceNote> noteList = seq[seq.Position];
            if(!seq.IsEnabled || null == noteList) {
                return;
            }

            foreach(SequenceNote seqNote in noteList)
            {
                // Send MIDI message.
                _outDev.Send(new NoteOnMessage(seq.Channel, seqNote.NoteId, seqNote.Velocity));

                // Record what notes are live so that we can send a MIDI off message later.
                RegisterLiveNote(seqNote, seq.Channel);
            }
        }

        private void RegisterLiveNote(SequenceNote seqNote, Channel chan)
        {
            LiveNote liveNote = new LiveNote(seqNote.NoteId, chan,  seqNote.DurationTicks);

            // Insert new node based on its time remaining (order is low to high).
            // Start the scan from the high end (were the new note is most likely to be).
            LinkedListNode<LiveNote> currNode = _liveNoteList.Last;
            while(null != currNode && seqNote.DurationTicks < currNode.Value.TicksRemaining) {
                currNode = currNode.Previous;
            }

            if(null == currNode)
            {
                _liveNoteList.AddFirst(liveNote);
                return;
            }
            _liveNoteList.AddAfter(currNode, liveNote);
        }

        #endregion
    }
}
