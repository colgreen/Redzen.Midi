
namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Note Off message.
    /// </summary>
    public class NoteOffMessage : NoteMessage
    {
        #region Constructor

        /// <summary>
        /// Constructs a Note Off message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">MIDI note ID.</param>
        /// <param name="velocity">Velocity; range is 0..127.</param>
        public NoteOffMessage(Channel channel, int noteId, int velocity)
            : base(channel, noteId, velocity) { }

        #endregion
    }
}
