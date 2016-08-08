using System;

namespace Redzen.Midi.Messages
{
    /// <summary>
    /// System exclusive message.
    /// </summary>
    public class SysExMessage : Message
    {
        readonly byte[] _data;

        #region Constructor

        /// <summary>
        /// Constructs a system exclusive message
        /// </summary>
        public SysExMessage(byte[] data)
        {
            _data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// System exclusive message data.
        /// </summary>
        public byte[] Data { get { return _data; } }

        #endregion
    }
}
