using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// MIDI Note message.
    /// </summary>
    public class NoteMessage : ChannelMessage
    {
        #region Auto Properties

        /// <summary>
        /// Note on (true), or note off message (false).
        /// </summary>
        public bool NoteOn { get; }

        /// <summary>MIDI note ID.</summary>
        public int NoteId { get; }
        /// <summary>
        /// Note velocity; range is 0..127.
        /// </summary>
        public int Velocity { get; }

        #endregion 

        #region Constructor

        /// <summary>
        /// Constructs a note message.
        /// </summary>
        public NoteMessage(Channel channel, bool noteOn, int noteId, int velocity)
            : base(channel)
        {
            if(!MidiUtils.IsInMidiRange(noteId)) {
                throw new ArgumentOutOfRangeException("note ID is out of MIDI range.");
            }

            if(velocity < 0 || velocity > 127) {
                throw new ArgumentOutOfRangeException("velocity");
            }
            this.NoteOn = noteOn;
            this.NoteId = noteId;
            this.Velocity = velocity;
        }

        #endregion
    }
}
