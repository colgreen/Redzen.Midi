using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Redzen.Midi.Messages;
using Redzen.Midi.Win32;

namespace Redzen.Midi.Devices
{
    /// <summary>
    /// Represents an input MIDI device.
    /// </summary>
    public class InputDevice : DeviceBase
    {
        #region Instance Fields

        Win32API.HMIDIIN _handle;
        Win32API.MidiInProc _inputCallbackDelegate;
        readonly object _lockObj = new object();

        // A list of pointers to all buffers created for handling Long Messages.
        readonly List<IntPtr> _longMsgBuffers = new List<IntPtr>();
        bool _isClosing = false;
        bool _terminateMessageThread = false;
        bool _isReceiving;

        // A queue for buffering received MIDI messages.
        readonly BlockingCollection<MidiInCallbackData> _msgQueue = new BlockingCollection<MidiInCallbackData>();

        #endregion

        #region Delegates

        /// <summary>Note On message received delegate.</summary>
        public delegate void NoteOnHandler(NoteMessage msg);

        /// <summary>Note Off message received delegate.</summary>
        public delegate void NoteOffHandler(NoteMessage msg);

        /// <summary>Control Change message received delegate.</summary>
        public delegate void ControlChangeHandler(ControlChangeMessage msg);

        /// <summary>Program Change message received delegate.</summary>
        public delegate void ProgramChangeHandler(ProgramChangeMessage msg);

        /// <summary>Pitch Bend message received delegate.</summary>
        public delegate void PitchBendHandler(PitchBendMessage msg);

        /// <summary>SysEx message received delegate.</summary>
        public delegate void SysExHandler(SysExMessage msg);

        /// <summary>Real Time message received delegate.</summary>
        public delegate void RealTimeHandler(RealTimeMessage msg);

        #endregion

        #region Events

        /// <summary>Note On message received.</summary>
        public event NoteOnHandler NoteOn;

        /// <summary>Note Off message received.</summary>
        public event NoteOffHandler NoteOff;

        /// <summary>Control Change message received.</summary>
        public event ControlChangeHandler ControlChange;

        /// <summary>Program Change message received.</summary>
        public event ProgramChangeHandler ProgramChange;

        /// <summary>Pitch Bend message received.</summary>
        public event PitchBendHandler PitchBend;

        /// <summary>SysEx message received.</summary>
        public event SysExHandler SysEx;

        /// <summary>Real Time message received.</summary>
        public event RealTimeHandler RealTime;

        #endregion

        #region Constructor

        internal InputDevice(UIntPtr deviceId, Win32API.MIDIINCAPS caps)
            : base(caps.szPname)
        {
            _deviceId = deviceId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if this device has been successfully opened.
        /// </summary>
        public bool IsOpen
        {
            get { lock(_lockObj) { return _isOpen; } }
        }

        /// <summary>
        /// True if this device is receiving messages.
        /// </summary>
        public bool IsReceiving
        {
            get { lock(_lockObj) { return _isReceiving; } }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the input device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is already open.</exception>
        /// <exception cref="DeviceException">The device cannot be opened.</exception>
        /// <remarks>Note that Open() establishes a connection to the device, but no messages will
        /// be received until <see cref="StartReceiving()"/> is called.</remarks>
        public void Open()
        {
            lock(_lockObj)
            {
                CheckNotOpen();

                _inputCallbackDelegate = this.InputCallback;
                StartMessageProcessingThread();

                CheckReturnCode(Win32API.midiInOpen(out _handle, _deviceId, _inputCallbackDelegate, (UIntPtr)0));
                _isOpen = true;
            }
        }

        /// <summary>
        /// Closes the input device.
        /// </summary>
        /// <exception cref="InvalidOperationException">The device is not open or is still
        /// receiving.</exception>
        /// <exception cref="DeviceException">The device cannot be closed.</exception>
        public void Close()
        {
            lock(_lockObj)
            {
                CheckOpen();

                _isClosing = true;
                if(_longMsgBuffers.Count > 0) {
                    CheckReturnCode(Win32API.midiInReset(_handle));
                }

                //Destroy any Long Message buffers we created when opening this device.
                //foreach (IntPtr buffer in LongMsgBuffers)
                //{
                //    if (DestroyLongMsgBuffer(buffer)) {
                //        LongMsgBuffers.Remove(buffer);
                //    }
                //}

                CheckReturnCode(Win32API.midiInClose(_handle));
                _terminateMessageThread = true;
                _isOpen = false;
                _isClosing = false;
            }
        }

        /// <summary>
        /// Releases all resources used by the device.
        /// </summary>
        public override void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Start receiving MIDI messages.
        /// </summary>
        public void StartReceiving()
        {
            StartReceiving(false);
        }

        /// <summary>
        /// Start receiving MIDI messages.
        /// </summary>
        /// <param name="handleSysEx">Set to true to enable receiving of SysEx messages (long messages)</param>
        public void StartReceiving(bool handleSysEx)
        {
            lock(_lockObj)
            {
                CheckOpen();
                CheckNotReceiving();

                if(handleSysEx) {
                    _longMsgBuffers.Add(CreateLongMsgBuffer());
                }

                CheckReturnCode(Win32API.midiInStart(_handle));
                _isReceiving = true;
            }
        }
        
        /// <summary>
        /// Stop receiving MIDI messages.
        /// </summary>
        public void StopReceiving()
        {
            lock(_lockObj)
            {
                CheckReceiving();
                CheckReturnCode(Win32API.midiInStop(_handle));
                _isReceiving = false;
            }
        }

        /// <summary>
        /// Removes all event handlers from the input events on this device.
        /// </summary>
        public void RemoveAllEventHandlers()
        {
            NoteOn = null;
            NoteOff = null;
            ControlChange = null;
            ProgramChange = null;
            PitchBend = null;
            SysEx = null;
        }

        #endregion

        #region Private Methods [Check Methods]

        private static void CheckReturnCode(Win32API.MMRESULT rc)
        {
            if(rc != Win32API.MMRESULT.MMSYSERR_NOERROR)
            {
                StringBuilder errorMsg = new StringBuilder(128);
                rc = Win32API.midiInGetErrorText(rc, errorMsg);
                if(rc != Win32API.MMRESULT.MMSYSERR_NOERROR) {
                    throw new DeviceException("no error details");
                }

                throw new DeviceException(errorMsg.ToString());
            }
        }

        /// <summary>
        /// Throws a MidiDeviceException if this device is not receiving.
        /// </summary>
        private void CheckReceiving()
        {
            if(!_isReceiving) {
                throw new DeviceException("device not receiving");
            }
        }

        /// <summary>
        /// Throws a MidiDeviceException if this device is receiving.
        /// </summary>
        private void CheckNotReceiving()
        {
            if(_isReceiving) {
                throw new DeviceException("device receiving");
            }
        }

        #endregion

        #region Private Methods [MessageProcessingThread]

        private void StartMessageProcessingThread()
        {
            Thread thread = new Thread(MessageProcessingThreadMethod);
            thread.Start();
        }

        private void MessageProcessingThreadMethod()
        {
            for(;;)
            {
                MidiInCallbackData cbData;
                if(_msgQueue.TryTake(out cbData, 200)) {
                    HandleMidiInMessage(cbData);
                }

                if(_terminateMessageThread) {
                    return;
                }
            }
        }

        #endregion

        #region Private Methods [MIDI Message Handling]

        /// <summary>
        /// The input callback for midiOutOpen.
        /// </summary>
        private void InputCallback(Win32API.HMIDIIN hMidiIn, Win32API.MidiInMessage wMsg,
                                   UIntPtr dwInstance, UIntPtr dwParam1, UIntPtr dwParam2)
        {
            // Package up the callback data and place onto a queue to be processed on another thread.
            // This allows the callback thread to return immediately and avoids the possibility of the callback thread
            // performing any unsafe operations, such as raising an event that might ultimately result in calling back
            // into the midi device on that same thread (which is not thread safe and will likely cause a deadlock).
            MidiInCallbackData cbData = new MidiInCallbackData();
            cbData.hMidiIn = hMidiIn;
            cbData.wMsg = wMsg;
            cbData.dwInstance = dwInstance;
            cbData.dwParam1 = dwParam1;
            cbData.dwParam2 = dwParam2;
            _msgQueue.Add(cbData);
        }

        private void HandleMidiInMessage(MidiInCallbackData cbData)
        {
            switch(cbData.wMsg)
            {
                case Win32API.MidiInMessage.MIM_DATA:
                    InputCallback_MidiInMessage_Data(cbData.dwParam1, cbData.dwParam2);
                    break;
                case Win32API.MidiInMessage.MIM_LONGDATA:
                    InputCallback_LongData(cbData.dwParam1, cbData.dwParam2);
                    break;

                // The rest of these are just for long message testing
                case Win32API.MidiInMessage.MIM_MOREDATA:
                    this.SysEx(new SysExMessage(new byte[] { 0x13 }));
                    break;
                case Win32API.MidiInMessage.MIM_OPEN:
                    //SysEx(new SysExMessage(this, new byte[] { 0x01 }, 1));
                    break;
                case Win32API.MidiInMessage.MIM_CLOSE:
                    //SysEx(new SysExMessage(this, new byte[] { 0x02 }, 2));
                    break;
                case Win32API.MidiInMessage.MIM_ERROR:
                    this.SysEx(new SysExMessage(new byte[] { 0x03 }));
                    break;
                case Win32API.MidiInMessage.MIM_LONGERROR:
                    SysEx(new SysExMessage(new byte[] { 0x04 }));
                    break;
                default:
                    this.SysEx(new SysExMessage(new byte[] { 0x05 }));
                    break;
            }
        }

        private void InputCallback_MidiInMessage_Data(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            Channel channel;
            int noteId;
            int velocity;
            int value;
            uint win32Timestamp;
            if(ShortMsg.IsNoteOn(dwParam1, dwParam2))
            {
                if(null != this.NoteOn)
                {
                    ShortMsg.DecodeNoteOn(dwParam1, dwParam2, out channel, out noteId, out velocity, out win32Timestamp);
                    NoteOn(new NoteMessage(channel, true, noteId, velocity));
                }
            }
            else if(ShortMsg.IsNoteOff(dwParam1, dwParam2))
            {
                if(null != this.NoteOff)
                {
                    ShortMsg.DecodeNoteOff(dwParam1, dwParam2, out channel, out noteId,
                                            out velocity, out win32Timestamp);
                    NoteOff(new NoteMessage(channel, false, noteId, velocity));
                }
            }
            else if(ShortMsg.IsControlChange(dwParam1, dwParam2))
            {
                if(null != this.ControlChange)
                {
                    ShortMsg.DecodeControlChange(dwParam1, dwParam2, out channel, out Control control, out value, out win32Timestamp);
                    ControlChange(new ControlChangeMessage(channel, control, value));
                }
            }
            else if(ShortMsg.IsProgramChange(dwParam1, dwParam2))
            {
                if(null != this.ProgramChange)
                {
                    ShortMsg.DecodeProgramChange(dwParam1, dwParam2, out channel, out Instrument instrument, out win32Timestamp);
                    ProgramChange(new ProgramChangeMessage(channel, instrument));
                }
            }
            else if(ShortMsg.IsPitchBend(dwParam1, dwParam2))
            {
                if(null != this.PitchBend)
                {
                    ShortMsg.DecodePitchBend(dwParam1, dwParam2, out channel, out value, out win32Timestamp);
                    PitchBend(new PitchBendMessage(channel, value));
                }
            }
            else if(ShortMsg.IsRealTime(dwParam1, dwParam2))
            {
                if(null != this.RealTime)
                {
                    RealTimeMessageType msgType;
                    ShortMsg.DecodeRealTime(dwParam1, dwParam2, out msgType, out win32Timestamp);

                    RealTime(new RealTimeMessage(msgType));
                }
            }
            else
            {
                // Unsupported messages are ignored.
                //Debug.WriteLine($"{dwParam1:X4} | {dwParam2:X4}");
            }
        }

        private void InputCallback_LongData(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            if(LongMsgUtils.IsSysEx(dwParam1, dwParam2))
            {
                if(null != this.SysEx)
                {
                    LongMsgUtils.DecodeSysEx(dwParam1, dwParam2, out byte[] data, out uint win32Timestamp);
                    if(0 != data.Length)
                    {
                        SysEx(new SysExMessage(data));
                    }

                    if(_isClosing)
                    {   //buffers no longer needed
                        DestroyLongMsgBuffer(dwParam1);
                    }
                    else
                    {   //prepare the buffer for the next message
                        RecycleLongMsgBuffer(dwParam1);
                    }
                }
            }
        }

        #endregion

        #region Private Methods [Long Message Buffer]

        private IntPtr CreateLongMsgBuffer()
        {
            //add a buffer so we can receive SysEx messages
            IntPtr ptr;
            uint size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR));
            Win32API.MIDIHDR header = new Win32API.MIDIHDR
            {
                lpData = System.Runtime.InteropServices.Marshal.AllocHGlobal(4096),
                dwBufferLength = 4096,
                dwFlags = 0
            };

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

            CheckReturnCode(Win32API.midiInPrepareHeader(_handle, ptr, size));
            CheckReturnCode(Win32API.midiInAddBuffer(_handle, ptr, size));
            //CheckReturnCode(Win32API.midiInUnprepareHeader(handle, ptr, size));

            return ptr;
        }

        private IntPtr RecycleLongMsgBuffer(UIntPtr ptr)
        {
            IntPtr newPtr = unchecked((IntPtr)(long)(ulong)ptr);
            UInt32 size = (UInt32)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR));
            CheckReturnCode(Win32API.midiInUnprepareHeader(_handle, newPtr, size));

            CheckReturnCode(Win32API.midiInPrepareHeader(_handle, newPtr, size));
            CheckReturnCode(Win32API.midiInAddBuffer(_handle, newPtr, size));
            //return unchecked((UIntPtr)(ulong)(long)newPtr);
            return newPtr;
        }

        /// <summary>
        /// Releases the resources associated with the specified MidiHeader pointer.
        /// </summary>
        /// <param name="ptr">
        /// The pointer to MIDIHDR buffer.
        /// </param>
        private bool DestroyLongMsgBuffer(UIntPtr ptr)
        {
            IntPtr newPtr = unchecked((IntPtr)(long)(ulong)ptr);
            uint size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR));
            CheckReturnCode(Win32API.midiInUnprepareHeader(_handle, newPtr, size));

            Win32API.MIDIHDR header = (Win32API.MIDIHDR)System.Runtime.InteropServices.Marshal.PtrToStructure(newPtr, typeof(Win32API.MIDIHDR));
            System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(newPtr);

            _longMsgBuffers.Remove(newPtr);
            
            return true;
        }

        #endregion
    }
}