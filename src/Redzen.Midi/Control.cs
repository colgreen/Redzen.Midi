
namespace Redzen.Midi
{
    /// <summary>
    /// MIDI Control types; used in Control Change messages.
    /// </summary>
    /// <remarks>
    /// In MIDI, Control Change messages are used to influence various auxiliary "controls"
    /// on a device, such as knobs, levers, and pedals.  Controls are specified with
    /// integers in [0..127].  This enum provides an incomplete list of controls, as each device will
    /// have it's own custom control settings. Even for the ones listed here, the details of how the values
    /// are arcane.  Please see the MIDI spec for details.
    /// 
    /// The most commonly used control is SustainPedal, which is considered off when &lt; 64,
    /// on when &gt; 64.
    /// </remarks>
    public enum Control
    {
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        ModulationWheel = 1,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        DataEntryMSB = 6,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        Volume = 7,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        Pan = 10,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        Expression = 11,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        DataEntryLSB = 38,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        SustainPedal = 64,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        ReverbLevel = 91,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        TremoloLevel = 92,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        ChorusLevel = 93,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        CelesteLevel = 94,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        PhaserLevel = 95,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        NonRegisteredParameterLSB = 98,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        NonRegisteredParameterMSB = 99,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        RegisteredParameterNumberLSB = 100,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        RegisteredParameterNumberMSB = 101,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        AllControllersOff = 121,
        /// <summary>General MIDI Control--See MIDI spec for details.</summary>
        AllNotesOff = 123
    }
}
