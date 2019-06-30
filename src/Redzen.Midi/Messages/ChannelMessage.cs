
namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Base class for MIDI channel messages.
    /// </summary>
    public abstract class ChannelMessage : Message
    {
        /// <summary>
        /// Channel.
        /// </summary>
        public Channel Channel { get; }

        /// <summary>
        /// Constructs a channel message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        protected ChannelMessage(Channel channel)
        {
            channel.Validate();
            this.Channel = channel;
        }
    }
}
