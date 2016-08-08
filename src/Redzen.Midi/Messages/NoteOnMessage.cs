
namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Note On message.
    /// </summary>
    public class NoteOnMessage : NoteMessage
    {
        /// <summary>
        /// Constructs a Note On message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">MIDI note ID.</param>
        /// <param name="velocity">Velocity; range is 0..127.</param>
        public NoteOnMessage(Channel channel, int noteId, int velocity)
            : base(channel, noteId, velocity) { }
    }
}
