using System;
using System.Text;
using Redzen.Midi.Messages;
using Redzen.Midi.Win32;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Represents an input MIDI device.
    /// </summary>
    public class OutputDevice : DeviceBase
    {
        #region Instance Fields

        Win32API.HMIDIOUT _handle;
        readonly object _lockObj = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Private Constructor, only called by the getter for the InstalledDevices property.
        /// </summary>
        /// <param name="deviceId">Position of this device in the list of all devices.</param>
        /// <param name="caps">Win32 Struct with device metadata</param>
        internal OutputDevice(UIntPtr deviceId, Win32API.MIDIOUTCAPS caps)
            : base(caps.szPname)
        {
            _deviceId = deviceId;
            _isOpen = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if this device is open.
        /// </summary>
        public bool IsOpen
        {
            get { lock(_lockObj) { return _isOpen; } }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the output device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is already open.</exception>
        /// <exception cref="DeviceException">The device cannot be opened.</exception>
        public void Open()
        {
            lock(_lockObj)
            {
                CheckNotOpen();
                CheckReturnCode(Win32API.midiOutOpen(out _handle, _deviceId, null, (UIntPtr)0));
                _isOpen = true;
            }
        }

        /// <summary>
        /// Closes the output device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The device cannot be closed.</exception>
        public void Close()
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutClose(_handle));
                _isOpen = false;
            }
        }

        /// <summary>
        /// Releases all resources used by the device.
        /// </summary>
        public override void Dispose()
        {
            Close();
        }

        #endregion

        #region Public Methods [Send*]

        /// <summary>
        /// Silences all notes on this output device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendReset()
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutReset(_handle));
            }
        }

        /// <summary>
        /// Send the provided MIDI message.
        /// </summary>
        /// <param name="msg">Th emessage to send.</param>
        public void Send(Message msg)
        {
            if(msg is NoteMessage noteMsg)
            {
                if(noteMsg.NoteOn) {
                    SendNoteOn(noteMsg.Channel, noteMsg.NoteId, noteMsg.Velocity);
                }
                else {
                    SendNoteOff(noteMsg.Channel, noteMsg.NoteId, noteMsg.Velocity);
                }
            }
        }

        /// <summary>
        /// Send a MIDI note-on message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID / pitch.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Channel, pitch, or velocity is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendNoteOn(Channel channel, int noteId, int velocity)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(_handle, ShortMsg.EncodeNoteOn(channel, noteId, velocity)));
            }
        }

        /// <summary>
        /// Send a MIDI note-off message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID / pitch.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Channel, pitch, or velocity is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendNoteOff(Channel channel, int noteId, int velocity)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(_handle, ShortMsg.EncodeNoteOff(channel, noteId, velocity)));
            }
        }

        /// <summary>
        /// Sends a Control Change message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="control">Control type.</param>
        /// <param name="value">Control value.</param>
        /// <exception cref="ArgumentOutOfRangeException">channel, control, or value is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendControlChange(Channel channel, Control control, int value)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(_handle, ShortMsg.EncodeControlChange(
                    channel, control, value)));
            }
        }

        /// <summary>
        /// Sends a Pitch Bend message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="value">The pitch bend value, 0..16383; Central value is 8192.</param>
        /// <exception cref="ArgumentOutOfRangeException">channel or value is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendPitchBend(Channel channel, int value)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(_handle, ShortMsg.EncodePitchBend(channel,
                    value)));
            }
        }

        /// <summary>
        /// Sends a Program Change message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="instrument">The instrument.</param>
        /// <exception cref="ArgumentOutOfRangeException">channel or instrument is out-of-range.</exception>
        /// <exception cref="InvalidOperationException">The device is not open.</exception>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        /// <remarks>
        /// A Program Change message is used to switch among instrument settings, generally
        /// instrument voices.  An instrument conforming to General Midi 1 will have the
        /// instruments described in the <see cref="Instrument"/> enum; other instruments
        /// may have different instrument sets.
        /// </remarks>
        public void SendProgramChange(Channel channel, Instrument instrument)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckReturnCode(Win32API.midiOutShortMsg(_handle, ShortMsg.EncodeProgramChange(
                    channel, instrument)));
            }
        }

        /// <summary>
        /// Sends a System Exclusive (sysex) message.
        /// </summary>
        /// <param name="data">The message to send (as byte array)</param>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        public void SendSysEx(byte[] data)
        {
            lock(_lockObj)
            {
                //Win32API.MMRESULT result;
                IntPtr ptr;
                UInt32 size = (UInt32)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR));
                Win32API.MIDIHDR header = new Win32API.MIDIHDR();
                header.lpData = System.Runtime.InteropServices.Marshal.AllocHGlobal(data.Length);
                for (int i = 0; i < data.Length; i++)
                    System.Runtime.InteropServices.Marshal.WriteByte(header.lpData, i, data[i]);
                header.dwBufferLength = data.Length;
                header.dwBytesRecorded = data.Length;
                header.dwFlags = 0;

                try
                {
                    ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR)));
                }
                catch (Exception)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                    throw;
                }

                try
                {
                    System.Runtime.InteropServices.Marshal.StructureToPtr(header, ptr, false);
                }
                catch (Exception)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                    throw;
                }

                //result = Win32API.midiOutPrepareHeader(handle, ptr, size);
                //if (result == 0) result = Win32API.midiOutLongMsg(handle, ptr, size);
                //if (result == 0) result = Win32API.midiOutUnprepareHeader(handle, ptr, size);
                CheckReturnCode(Win32API.midiOutPrepareHeader(_handle, ptr, size));
                CheckReturnCode(Win32API.midiOutLongMsg(_handle, ptr, size));
                CheckReturnCode(Win32API.midiOutUnprepareHeader(_handle, ptr, size));

                System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Tests if rc is MidiWin32Wrapper.MMSYSERR_NOERROR; if not then throws an exception
        /// with an appropriate error message.
        /// </summary>
        /// <param name="rc"></param>
        private static void CheckReturnCode(Win32API.MMRESULT rc)
        {
            if(rc != Win32API.MMRESULT.MMSYSERR_NOERROR)
            {
                StringBuilder errorMsg = new StringBuilder(128);
                rc = Win32API.midiOutGetErrorText(rc, errorMsg);
                if(rc != Win32API.MMRESULT.MMSYSERR_NOERROR) {
                    throw new DeviceException("no error details");
                }
                throw new DeviceException(errorMsg.ToString());
            }
        }

        #endregion
    }
}
