
namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Base class for MIDI channel messages.
    /// </summary>
    public abstract class ChannelMessage : Message
    {
        readonly Channel _channel;

        /// <summary>
        /// Constructs a channel message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        protected ChannelMessage(Channel channel)
        {
            channel.Validate();
            _channel = channel;
        }

        /// <summary>
        /// Channel.
        /// </summary>
        public Channel Channel { get { return _channel; } }
    }
}
