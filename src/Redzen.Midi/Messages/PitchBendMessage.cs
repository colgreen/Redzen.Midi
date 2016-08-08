using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Pitch Bend message.
    /// </summary>
    public class PitchBendMessage : ChannelMessage
    {
        readonly int _value;

        #region Constructor

        /// <summary>
        /// Constructs a Pitch Bend message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="value">Pitch bend value, 0..16383; central value is 8192.</param>        
        public PitchBendMessage(Channel channel, int value)
            : base(channel)
        {
            if(value < 0 || value > 16383) {
                throw new ArgumentOutOfRangeException("value");
            }
            _value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Pitch bend value, 0..16383; central value is 8192.
        /// </summary>
        public int Value { get { return _value; } }

        #endregion
    }
}
