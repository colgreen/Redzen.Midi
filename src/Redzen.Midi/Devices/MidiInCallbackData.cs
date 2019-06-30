using System;
using Redzen.Midi.Win32;

namespace Redzen.Midi.Devices
{
    #pragma warning disable

    public class MidiInCallbackData
    {
        internal Win32API.HMIDIIN hMidiIn;
        internal Win32API.MidiInMessage wMsg;
        public UIntPtr dwInstance;
        public UIntPtr dwParam1;
        public UIntPtr dwParam2;
    }

    #pragma warning restore
}
