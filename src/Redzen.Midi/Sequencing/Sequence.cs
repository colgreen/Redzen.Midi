using System.Collections.Generic;
using RedZen.Midi;

namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// Represents a sequence of notes.
    /// </summary>
    /// <remarks>A note is allocated to a sequencer timeslot. 
    /// There are 24 slots per beat.
    /// Multiple notes can be assigned to the same timeslot.</remarks>
    public class Sequence
    {
        #region Instance Fields

        readonly int _lengthInTicks;

        // Note. There can be multiple notes assigned to each time slot, hence there is a List of notes at each timeslot.
        readonly List<SequenceNote>[] _noteSeq;

        /// <summary>
        /// Supplemetray info associated with the sequence.
        /// </summary>
        public Dictionary<string,string> InfoById = new Dictionary<string, string>();

        #endregion

        #region Auto Properties / Properties

        /// <summary>
        /// The MIDI channel the sequence messages will be sent to.
        /// </summary>
        public Channel Channel { get; }

        /// <summary>
        /// The sequence length (in MIDI clock ticks).
        /// </summary>
        public int Length => _noteSeq.Length;

        /// <summary>
        /// Get the notes assigned to the specified step/slot in the sequence
        /// </summary>
        /// <param name="idx">The step/slot.</param>
        /// <returns>A list of notes assigned to the step/slot in the sequence.</returns>
        public List<SequenceNote> this[int idx] => _noteSeq[idx];

        /// <summary>
        /// The current position of the sequence.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Indicates if the sequence is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create an empty sequence.
        /// </summary>
        /// <param name="chan">MIDI channel that the sequence is bound to.</param>
        /// <param name="lengthInTicks">The sequence length in timing clocks. Note. there are 24 timing clocks per beat. 
        /// So e.g. if you require a 1 minute sequence at 120 bpm then the length should be 120*24 = 2880.</param>
        public Sequence(Channel chan, int lengthInTicks)
        {
            this.Channel = chan;
            _lengthInTicks = lengthInTicks;
            _noteSeq = new List<SequenceNote>[_lengthInTicks];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a note to the sequence at the specified step/slot.
        /// </summary>
        /// <param name="seqNote">The note to add.</param>
        /// <param name="timeSlot">The step/slot in the sequence to add the note to.</param>
        public void AddNote(SequenceNote seqNote, int timeSlot)
        {
            // Record in the note-on schedule.
            List<SequenceNote> list = _noteSeq[timeSlot];
            if(null == list) {
                list = _noteSeq[timeSlot] = new List<SequenceNote>();
            }
            list.Add(seqNote);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a sequence with the specified length in MIDI clock ticks.
        /// Note. there are 24 clock ticks per beat. so e.g. 120 bpm gives a length in ticks of 120 * 24 = 2880.
        /// </summary>
        /// <param name="chan">MIDI channel</param>
        /// <param name="ticks">Length in MIDI clock ticks.</param>
        public static Sequence CreateLengthInMidiClockTicks(Channel chan, int ticks)
        {
            return new Sequence(chan, ticks);
        }

        /// <summary>
        /// Create a sequence with the specified length in MIDI beats.
        /// Note. there are 24 clock ticks per beat. so e.g. 120 bpm gives a length in ticks of 120 * 24 = 2880.
        /// </summary>
        /// <param name="chan">MIDI channel</param>
        /// <param name="beats">Length in MIDI beats.</param>
        public static Sequence CreateLengthInBeats(Channel chan, int beats)
        {
            return new Sequence(chan, beats * MidiConsts.ClocksPerBeat);
        }

        #endregion
    }
}
