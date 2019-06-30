using Redzen.Random;

namespace Redzen.Midi.Sequencing
{
    /// <summary>
    /// For generating random <see cref="Sequence"/> instances.
    /// </summary>
    public class SequenceGenerator
    {
        readonly IRandomSource _rng;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SequenceGenerator() {
            _rng = RandomDefaults.CreateRandomSource();
        }

        /// <summary>
        /// Construct with a given random number generator seed.
        /// </summary>
        /// <param name="seed">Random number generator seed.</param>
        public SequenceGenerator(int seed) {
            _rng = RandomDefaults.CreateRandomSource();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the random number generator seed
        /// </summary>
        /// <param name="seed">Seed</param>
        public void SetSeed(ulong seed)
        {
            _rng.Reinitialise(seed);
        }

        /// <summary>
        /// Create a <see cref="Sequence"/> with randomized note timings.
        /// </summary>
        /// <param name="chan">MIDI channel.</param>
        /// <param name="lengthInBeats">Seqeucen length in MIDI beats.</param>
        /// <param name="probabilty">The probability of a note being on in any of the sequence slots (subject to quantization).</param>
        /// <param name="quantizeTicks">Defines what slots are considered for on/off status.</param>
        /// <param name="noteId">The note ID to assign to ON notes.</param>
        /// <returns>A new <see cref="Sequence"/>.</returns>
        public Sequence CreateRandom(Channel chan, int lengthInBeats, double probabilty, int quantizeTicks, int noteId)
        {
            Sequence seq = Sequence.CreateLengthInBeats(chan, lengthInBeats);
            for(int i=0; i < seq.Length; i += quantizeTicks)
            {
                if(_rng.NextDouble() >= probabilty) {
                    continue;
                }
                seq.AddNote(new SequenceNote(noteId, 80, 4), i);
            }
            return seq;
        }

        #endregion
    }
}
