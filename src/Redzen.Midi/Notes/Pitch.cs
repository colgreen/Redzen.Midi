using Redzen.Midi.Devices;

namespace Redzen.Midi.Notes
{
    #pragma warning disable

    /// <summary>
    /// Pitches supported by MIDI.
    /// </summary>
    /// <remarks>
    /// <para>MIDI defines 127 distinct pitches, in semitone intervals, ranging from C five octaves
    /// below middle C, up to G five octaves above middle C.  This covers several octaves above and
    /// below the range of a normal 88-key piano.</para>
    /// <para>These 127 pitches are the only ones directly expressible in MIDI. Precise
    /// variations in frequency can be achieved with <see cref="OutputDevice.SendPitchBend">Pitch
    /// Bend</see> messages, though Pitch Bend messages apply to the whole channel at once.</para>
    /// <para>In this enum, pitches are given C Major note names (eg "F", "GSharp") followed
    /// by the octave number.  Octaves use standard piano terminology: Middle C is in
    /// octave 4.  (Note that this is different from "MIDI octaves", which have Middle C in
    /// octave 0.)</para>
    /// </remarks>
    public enum Pitch
    {
        CNeg1 = 0,
        CSharpNeg1 = 1,
        DNeg1 = 2,
        DSharpNeg1 = 3,
        ENeg1 = 4,
        FNeg1 = 5,
        FSharpNeg1 = 6,
        GNeg1 = 7,
        GSharpNeg1 = 8,
        ANeg1 = 9,
        ASharpNeg1 = 10,
        BNeg1 = 11,

        C0 = 12,
        CSharp0 = 13,
        D0 = 14,
        DSharp0 = 15,
        E0 = 16,
        F0 = 17,
        FSharp0 = 18,
        G0 = 19,
        GSharp0 = 20,
        A0 = 21,
        ASharp0 = 22,
        B0 = 23,

        C1 = 24,
        CSharp1 = 25,
        D1 = 26,
        DSharp1 = 27,
        E1 = 28,
        F1 = 29,
        FSharp1 = 30,
        G1 = 31,
        GSharp1 = 32,
        A1 = 33,
        ASharp1 = 34,
        B1 = 35,

        C2 = 36,
        CSharp2 = 37,
        D2 = 38,
        DSharp2 = 39,
        E2 = 40,
        F2 = 41,
        FSharp2 = 42,
        G2 = 43,
        GSharp2 = 44,
        A2 = 45,
        ASharp2 = 46,
        B2 = 47,

        C3 = 48,
        CSharp3 = 49,
        D3 = 50,
        DSharp3 = 51,
        E3 = 52,
        F3 = 53,
        FSharp3 = 54,
        G3 = 55,
        GSharp3 = 56,
        A3 = 57,
        ASharp3 = 58,
        B3 = 59,

        C4 = 60,
        CSharp4 = 61,
        D4 = 62,
        DSharp4 = 63,
        E4 = 64,
        F4 = 65,
        FSharp4 = 66,
        G4 = 67,
        GSharp4 = 68,
        A4 = 69,
        ASharp4 = 70,
        B4 = 71,

        C5 = 72,
        CSharp5 = 73,
        D5 = 74,
        DSharp5 = 75,
        E5 = 76,
        F5 = 77,
        FSharp5 = 78,
        G5 = 79,
        GSharp5 = 80,
        A5 = 81,
        ASharp5 = 82,
        B5 = 83,

        C6 = 84,
        CSharp6 = 85,
        D6 = 86,
        DSharp6 = 87,
        E6 = 88,
        F6 = 89,
        FSharp6 = 90,
        G6 = 91,
        GSharp6 = 92,
        A6 = 93,
        ASharp6 = 94,
        B6 = 95,

        C7 = 96,
        CSharp7 = 97,
        D7 = 98,
        DSharp7 = 99,
        E7 = 100,
        F7 = 101,
        FSharp7 = 102,
        G7 = 103,
        GSharp7 = 104,
        A7 = 105,
        ASharp7 = 106,
        B7 = 107,

        C8 = 108,
        CSharp8 = 109,
        D8 = 110,
        DSharp8 = 111,
        E8 = 112,
        F8 = 113,
        FSharp8 = 114,
        G8 = 115,
        GSharp8 = 116,
        A8 = 117,
        ASharp8 = 118,
        B8 = 119,

        C9 = 120,
        CSharp9 = 121,
        D9 = 122,
        DSharp9 = 123,
        E9 = 124,
        F9 = 125,
        FSharp9 = 126,
        G9 = 127
    }

    #pragma warning restore
}
