using System;
using System.Threading;
using RedZen.Midi;

namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// An implementation of MidiClock that fires clock events based on a beats-per-minute setting and use of the
    /// system clock to time MIDI clock ticks.
    /// </summary>
    public class InternalMidiClock : MidiClock
    {
        int _bpm;
        int _newbpm;
        int _period;
        Timer _timer;
        int _isDisposing = 0;

        #region Constructor

        /// <summary>
        /// Construct an internal clock.
        /// </summary>
        /// <param name="bpm">Beats per minute.</param>
        public InternalMidiClock(int bpm)
        {
            _bpm = bpm;
            _newbpm = bpm;

            // Calc the delay in milliseconds between timing clocks.
            _period = (int)Math.Round(1000.0 / ((_bpm * MidiConsts.ClocksPerBeat)/60.0));
            _timer = new Timer(OnTick, null, Timeout.Infinite, _period);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Beats per minute.
        /// </summary>
        public int BeatsPerMin
        {
            get { return _newbpm;  }
            set
            {
                // Note. we don't call _timer.Change() here because it does not provide an means of
                // updating its period without retriggering. Instead we update the timer when it next triggers.
                _newbpm =  Math.Min(360, Math.Max(1, value));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the clock.
        /// </summary>
        public override void Start()
        {
            _timer.Change(0, _period);
        }

        /// <summary>
        /// Stop the clock.
        /// </summary>
        public override void Stop()
        {
            _timer.Change(Timeout.Infinite, _period);
        }

        /// <summary>
        /// Releases all resources used by the clock.
        /// </summary>
        public override void Dispose()
        {
            Interlocked.Exchange(ref _isDisposing, 1);
            _timer.Dispose();
        }

        #endregion

        #region Private Methods

        private void OnTick(object stateInfo)
        {
            // Avoid raising a timing clock event if the class is disposing (or disposed).
            // There may be cases where Dispose() has been called but we get a further clock tick to handle, and to avoid 
            // complications we avoid propagating such an event any further.
            if(1 == Interlocked.CompareExchange(ref _isDisposing, 1, 1)) {
                return;
            }

            // Update the time rperiod if necessary.
            if(_newbpm != _bpm)
            {
                // Calc the delay in milliseconds between timing clocks.
                _period = (int)Math.Round(1000.0 / ((_bpm * MidiConsts.ClocksPerBeat)/60.0));
                _timer.Change(_period, _period);

                _bpm = _newbpm;
            }

            OnTimingClock();
        }

        #endregion
    }
}
