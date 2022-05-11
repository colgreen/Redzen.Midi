using System.Threading;
using Redzen.Midi.Devices;
using Redzen.Midi.Messages;

namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// An implementation of MidiClock that fires clock events based on MIDI timing clock messages from a MIDI input device.
    /// </summary>
    /// <remarks>
    /// As a general principle the MidiClock classes set-up timers and timing callbacks upon initialisation, and use a boolean flag 
    /// to indicate if the clock is currently propagating timing clocks to event listeners; this avoids any potential timing anomalies
    /// caused by setting up and tearing down callbacks and timers.
    /// </remarks>
    public class ExternalMidiClock : MidiClock
    {
        readonly InputDevice _inDev;
        readonly bool _ownsInputDevice;
        bool _isRunning = false;
        int _isDisposing = 0;

        #region Constructor

        /// <summary>
        /// Construct an external clock.
        /// </summary>
        /// <param name="inDev">The MIDI input device to receive timing clock messages from.</param>
        /// <param name="ownsInputDevice">Indicates if this class owns the InputDevice, and is therefore
        /// responsible for calling Dispose() on it.</param>
        public ExternalMidiClock(InputDevice inDev, bool ownsInputDevice)
        {
            // Register real time message handler.
            _inDev = inDev;
            _ownsInputDevice = ownsInputDevice;
            inDev.RealTime += RealTime;

            if(ownsInputDevice)
            {
                inDev.Open();
                inDev.StartReceiving();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the clock.
        /// </summary>
        public override void Start()
        {
            _isRunning = true;
        }

        /// <summary>
        /// Stop the clock.
        /// </summary>
        public override void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Releases all resources used by the clock.
        /// </summary>
        public override void Dispose()
        {
            Interlocked.Exchange(ref _isDisposing, 1);
            _inDev.RealTime -= RealTime;

            if(_ownsInputDevice)
            {
                _inDev.StopReceiving();
                _inDev.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private void RealTime(RealTimeMessage msg)
        {
            // Avoid raising a timing clock event if the class is disposing (or disposed).
            // There may be cases where Dispose() has been called but we get a further clock tick to handle, and to avoid 
            // complications we avoid propagating such an event any further.
            if(1 == Interlocked.CompareExchange(ref _isDisposing, 1, 1)) {
                return;
            }

            if(_isRunning && msg.MessageType == RealTimeMessageType.TimingClock) {
                OnTimingClock();
            }
        }

        #endregion
    }
}
