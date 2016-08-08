
namespace Redzen.Midi
{
    /// <summary>
    /// MIDI Channels.
    /// </summary>
    /// <remarks>
    /// Each MIDI device has 16 independent channels.  Channels are named starting at 1, but
    /// are encoded programmatically starting at 0.
    /// All of the channels are general-purpose except for Channel10, which is the dedicated 
    /// percussion channel. Any notes sent to that channel will play percussion notes regardless of
    /// anyProgram Change messages sent on that channel.
    /// </remarks>
    public enum Channel
    {
        /// <summary> MIDI Channel 1. </summary>
        Channel1 = 0,
        /// <summary> MIDI Channel 2. </summary>
        Channel2 = 1,
        /// <summary> MIDI Channel 3. </summary>
        Channel3 = 2,
        /// <summary> MIDI Channel 4. </summary>
        Channel4 = 3,
        /// <summary> MIDI Channel 5. </summary>
        Channel5 = 4,
        /// <summary> MIDI Channel 6. </summary>
        Channel6 = 5,
        /// <summary> MIDI Channel 7. </summary>
        Channel7 = 6,
        /// <summary> MIDI Channel 8. </summary>
        Channel8 = 7,
        /// <summary> MIDI Channel 9. </summary>
        Channel9 = 8,
        /// <summary> MIDI Channel 10, the dedicated percussion channel. </summary>
        Channel10 = 9,
        /// <summary> MIDI Channel 11. </summary>
        Channel11 = 10,
        /// <summary> MIDI Channel 12. </summary>
        Channel12 = 11,
        /// <summary> MIDI Channel 13. </summary>
        Channel13 = 12,
        /// <summary> MIDI Channel 14. </summary>
        Channel14 = 13,
        /// <summary> MIDI Channel 15. </summary>
        Channel15 = 14,
        /// <summary> MIDI Channel 16. </summary>
        Channel16 = 15
    };
}
