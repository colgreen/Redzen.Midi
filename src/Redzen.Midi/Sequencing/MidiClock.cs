using System;

namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// Midi clock base class.
    /// </summary>
    public abstract class MidiClock : IDisposable
    {
        #region Delegates / Events

        /// <summary>
        /// Timing clock event delegate.
        /// </summary>
        public delegate void TimingClockHandler();
        /// <summary>
        /// Timing clock event.
        /// </summary>
        event TimingClockHandler _timingClockEvent;

        #endregion

        #region Statics / Consts

        /// <summary>
        /// The number of timing clocks per MIDI beat. This is a fixed number in the MIDI spec.
        /// </summary>
        public const int __timingClocksPerBeat = 24;

        #endregion

        #region Event Accessors

        /// <summary>
        /// Clock tick event.
        /// </summary>
        public event TimingClockHandler TimingClock
        {
            add { _timingClockEvent += value; }
            remove { _timingClockEvent -= value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the clock.
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// Stops the clock.
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// Releases all resources used by the clock.
        /// </summary>
        public abstract void Dispose();
    
        #endregion

        #region Protected Methods

        /// <summary>
        /// Invokes the TimingClock event.
        /// </summary>
        protected virtual void OnTimingClock()
        {
            _timingClockEvent?.Invoke();
        }

        #endregion
    }
}
