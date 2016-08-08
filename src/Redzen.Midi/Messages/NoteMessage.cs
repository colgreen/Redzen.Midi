using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Base class for messages relevant to a specific note.
    /// </summary>
    public abstract class NoteMessage : ChannelMessage
    {
        readonly int _noteId;
        readonly int _velocity;

        #region Constructor

        /// <summary>
        /// Constructs a note message.
        /// </summary>
        protected NoteMessage(Channel channel, int noteId, int velocity)
            : base(channel)
        {
            if(!MidiUtils.IsInMidiRange(noteId)) {
                throw new ArgumentOutOfRangeException("note ID is out of MIDI range.");
            }

            if(velocity < 0 || velocity > 127) {
                throw new ArgumentOutOfRangeException("velocity");
            }
            _noteId = noteId;
            _velocity = velocity;
        }

        #endregion

        #region Properties

        /// <summary>MIDI note ID.</summary>
        public int NoteId { get { return _noteId; } }
        /// <summary>
        /// Note velocity; range is 0..127.
        /// </summary>
        public int Velocity { get { return _velocity; } }

        #endregion        
    }
}
