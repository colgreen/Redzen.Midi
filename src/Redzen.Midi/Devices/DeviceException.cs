using System;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Exception thrown when an operation on a MIDI device cannot be satisfied.
    /// </summary>
    public class DeviceException : ApplicationException
    {
        /// <summary>
        /// Constructs exception with a specific error message.
        /// </summary>
        /// <param name="message"></param>
        public DeviceException(string message) { }
    }
}
