using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Control change message.
    /// </summary>
    public class ControlChangeMessage : ChannelMessage
    {
        #region Auto Properties

        /// <summary>
        /// Control type.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// Control value.
        /// </summary>
        public int Value { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a Control Change message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="control">Control type.</param>
        /// <param name="value">Control value.</param>
        public ControlChangeMessage(Channel channel, Control control, int value)
            : base(channel)
        {
            control.Validate();
            if(value < 0 || value > 127) {
                throw new ArgumentOutOfRangeException("control");
            }
            this.Control = control;
            this.Value = value;
        }

        #endregion
    }
}
