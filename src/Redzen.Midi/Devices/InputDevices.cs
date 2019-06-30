using System;
using System.Collections.ObjectModel;
using Redzen.Midi.Win32;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Static class for enumerating available MIDI input devices.
    /// </summary>
    public static class InputDevices
    {
        #region Static Fields

        static readonly object __lockObj = new object();
        static InputDevice[] __installedDevices = null;

        #endregion

        #region Static Properties

        /// <summary>
        /// List of input devices installed on this system.
        /// </summary>
        public static ReadOnlyCollection<InputDevice> InstalledDevices
        {
            get
            {
                lock(__lockObj)
                {
                    if(null == __installedDevices) {
                        __installedDevices = MakeDeviceList();
                    }
                    return new ReadOnlyCollection<InputDevice>(__installedDevices);
                }
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Refresh the list of input devices
        /// </summary>
        public static void UpdateInstalledDevices()
        {
            lock(__lockObj) {
                __installedDevices = null;
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Private method for constructing the array of MidiInputDevices by calling the Win32 api.
        /// </summary>
        /// <returns></returns>
        private static InputDevice[] MakeDeviceList()
        {
            uint inDevs = Win32API.midiInGetNumDevs();
            InputDevice[] result = new InputDevice[inDevs];
            for(uint deviceId = 0; deviceId < inDevs; deviceId++)
            {
                Win32API.MIDIINCAPS caps = new Win32API.MIDIINCAPS();
                Win32API.midiInGetDevCaps((UIntPtr)deviceId, out caps);
                result[deviceId] = new InputDevice((UIntPtr)deviceId, caps);
            }
            return result;
        }

        #endregion
    }
}
