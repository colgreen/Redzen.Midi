namespace Redzen.Midi
{
    public class MidiUtils
    {
        /// <summary>
        /// Returns true if pitch is in the MIDI range [1..127].
        /// </summary>
        /// <param name="noteId">MIDI note ID.</param>
        /// <returns>True if the pitch is in [0..127].</returns>
        public static bool IsInMidiRange(int noteId)
        {
            return noteId >= 0 && noteId < 128;
        }

    }
}
