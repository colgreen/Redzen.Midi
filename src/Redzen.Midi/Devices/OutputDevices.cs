using System;
using System.Collections.ObjectModel;
using Redzen.Midi.Win32;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Static class for enumerating avialbale MIDI output devices.
    /// </summary>
    public static class OutputDevices
    {
        #region Static Fields

        static readonly object __lockObj = new object();
        static OutputDevice[] __installedDevices = null;

        #endregion

        #region Static Properties

        /// <summary>
        /// List of devices installed on this system.
        /// </summary>
        public static ReadOnlyCollection<OutputDevice> InstalledDevices
        {
            get
            {
                lock(__lockObj)
                {
                    if(null == __installedDevices) {
                        __installedDevices = MakeDeviceList();
                    }
                    return new ReadOnlyCollection<OutputDevice>(__installedDevices);
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
        /// Private method for constructing the array of MidiOutputDevices by calling the Win32 api.
        /// </summary>
        /// <returns></returns>
        private static OutputDevice[] MakeDeviceList()
        {
            uint outDevs = Win32API.midiOutGetNumDevs();
            OutputDevice[] result = new OutputDevice[outDevs];
            for (uint deviceId = 0; deviceId < outDevs; deviceId++)
            {
                Win32API.MIDIOUTCAPS caps = new Win32API.MIDIOUTCAPS();
                Win32API.midiOutGetDevCaps((UIntPtr)deviceId, out caps);
                result[deviceId] = new OutputDevice((UIntPtr)deviceId, caps);
            }
            return result;
        }

        #endregion
    }
}
