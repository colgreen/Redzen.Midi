
namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// A note for use within a <see cref="Sequence"/>.
    /// This class contains a pitch and a duration (and a velocity), and therefore represents a pair of MIDI on and off note messages.
    /// The term 'noteId' is used at this level because it could refer to a pitch or a percussion hit.
    /// </summary>
    public class SequenceNote
    {
        readonly int _noteId;
        readonly int _velocity;
        readonly int _durationTicks;

        #region Constructor

        /// <summary>
        /// Construct with the provided note parameters.
        /// </summary>
        /// <param name="noteId">Note ID (pitch of percussion type).</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="durationTicks">Duration in MIDI clock ticks.</param>
        public SequenceNote(int noteId, int velocity, int durationTicks)
        {
            _noteId = noteId;
            _velocity = velocity;
            _durationTicks = durationTicks;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Note ID (pitch of percussion type).
        /// </summary>
        public int NoteId
        {
            get { return _noteId; }
        }

        /// <summary>
        /// Velocity.
        /// </summary>
        public int Velocity
        {
            get { return _velocity; }
        }
        
        /// <summary>
        /// Duration in MIDI clock ticks.
        /// </summary>
        public int DurationTicks
        {
            get { return _durationTicks; }

        }

        #endregion
    }
}
