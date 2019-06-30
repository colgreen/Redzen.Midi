using System;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Common base class for input and output devices.
    /// </summary>
    /// This base class exists mainly so that input and output devices can both go into the same
    /// kinds of MidiMessages.
    public abstract class DeviceBase : IDisposable
    {
        #region Instance Fields / Auto Properties

        protected UIntPtr _deviceId;
        protected bool _isOpen;

        /// <summary>
        /// The name of this device.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="name">The name of this device.</param>
        protected DeviceBase(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Releases all resources used by the device.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Throws a MidiDeviceException if this device is not open.
        /// </summary>
        protected void CheckOpen()
        {
            if(!_isOpen) {
                throw new InvalidOperationException("device not open");
            }
        }

        /// <summary>
        /// Throws a MidiDeviceException if this device is open.
        /// </summary>
        protected void CheckNotOpen()
        {
            if(_isOpen) {
                throw new InvalidOperationException("device open");
            }
        }

        #endregion
    }
}
