using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// Real time message.
    /// </summary>
    public class RealTimeMessage : Message
    {
        readonly RealTimeMessageType _msgType;

        #region Constructor

        /// <summary>
        /// Constructs a real time message.
        /// </summary>
        /// <param name="msgType">Real time message type.</param>
        public RealTimeMessage(RealTimeMessageType msgType)
        {
            _msgType = msgType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Real time message type.
        /// </summary>
        public RealTimeMessageType MessageType
        {
            get { return _msgType; }
        }

        #endregion
    }
}
