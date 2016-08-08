using System;
using Redzen.Midi.Messages;

namespace Redzen.Midi.Win32
{
    /// <summary>
    /// Utility functions for encoding and decoding short messages.
    /// </summary>
    static class ShortMsg
    {
        #region Public Static Methods [NoteOn]

        /// <summary>
        /// Returns true if the given short message describes a Note On message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsNoteOn(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf0) == 0x90;
        }

        /// <summary>
        /// Decodes a Note On short message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <param name="timestamp">Note timestamp.</param>
        public static void DecodeNoteOn(UIntPtr dwParam1, UIntPtr dwParam2,
            out Channel channel, out int noteId, out int velocity, out UInt32 timestamp)
        {
            if(!IsNoteOn(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a Note On message.");
            }
            channel = (Channel)((int)dwParam1 & 0x0f);
            noteId = (((int)dwParam1 & 0xff00) >> 8);
            velocity = (((int)dwParam1 & 0xff0000) >> 16);
            timestamp = (UInt32)dwParam2;
        }

        /// <summary>
        /// Encodes a Note On short message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <returns>A value that can be passed to midiOutShortMsg.</returns>
        /// <exception cref="ArgumentOutOfRangeException">noteId is not in MIDI range.</exception>
        public static UInt32 EncodeNoteOn(Channel channel, int noteId, int velocity)
        {
            channel.Validate();
            if(!MidiUtils.IsInMidiRange(noteId)) {
                throw new ArgumentOutOfRangeException("Pitch out of MIDI range.");
            }
            if(velocity < 0 || velocity > 127) {
                throw new ArgumentOutOfRangeException("Velocity is out of range.");
            }
            return (UInt32)(0x90 | ((int)channel) | (noteId << 8) | (velocity << 16));
        }

        #endregion

        #region Public Static Methods [NoteOff]

        /// <summary>
        /// Returns true if the given short message describes a Note Off message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsNoteOff(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf0) == 0x80;
        }

        /// <summary>
        /// Decodes a Note Off short message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <param name="timestamp">Note timestamp.</param>
        public static void DecodeNoteOff(UIntPtr dwParam1, UIntPtr dwParam2,
            out Channel channel, out int noteId, out int velocity, out UInt32 timestamp)
        {
            if(!IsNoteOff(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a Note Off message.");
            }
            channel = (Channel)((int)dwParam1 & 0x0f);
            noteId = ((int)dwParam1 & 0xff00) >> 8;
            velocity = (((int)dwParam1 & 0xff0000) >> 16);
            timestamp = (UInt32)dwParam2;
        }

        /// <summary>
        /// Encodes a Note Off short message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="noteId">Note ID.</param>
        /// <param name="velocity">Note velocity.</param>
        /// <returns>A value that can be passed to midiOutShortMsg.</returns>
        public static UInt32 EncodeNoteOff(Channel channel, int noteId, int velocity)
        {
            channel.Validate();
            if(!MidiUtils.IsInMidiRange(noteId)) {
                throw new ArgumentOutOfRangeException("Pitch out of MIDI range.");
            }
            if(velocity < 0 || velocity > 127) {
                throw new ArgumentOutOfRangeException("Velocity is out of range.");
            }
            return (UInt32)(0x80 | ((int)channel) | (noteId << 8) | (velocity << 16));
        }

        #endregion

        #region Public Static Methods [ControlChange]

        /// <summary>
        /// Returns true if the given short message describes a Control Change message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsControlChange(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf0) == 0xB0;
        }

        /// <summary>
        /// Decodes a Control Change short message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="control">Control type.</param>
        /// <param name="value">Control value.</param>
        /// <param name="timestamp">Message timestamp.</param>
        public static void DecodeControlChange(UIntPtr dwParam1, UIntPtr dwParam2,
            out Channel channel, out Control control, out int value, out UInt32 timestamp)
        {
            if(!IsControlChange(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a control message.");
            }
            channel = (Channel)((int)dwParam1 & 0x0f);
            control = (Control)(((int)dwParam1 & 0xff00) >> 8);
            value = (((int)dwParam1 & 0xff0000) >> 16);
            timestamp = (UInt32)dwParam2;
        }

        /// <summary>
        /// Encodes a Control Change short message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="control">Control type.</param>
        /// <param name="value">Control value.</param>
        /// <returns>A value that can be passed to midiOutShortMsg.</returns>
        public static UInt32 EncodeControlChange(Channel channel, Control control, int value)
        {
            channel.Validate();
            control.Validate();
            if(value < 0 || value > 127) {
                throw new ArgumentOutOfRangeException("Value is out of range.");
            }
            return (UInt32)(0xB0 | (int)(channel) | ((int)control << 8) | (value << 16));
        }

        #endregion

        #region Public Static Methods [ProgramChange]

        /// <summary>
        /// Returns true if the given short message a Program Change message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsProgramChange(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf0) == 0xC0;
        }

        /// <summary>
        /// Decodes a Program Change short message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="instrument">Instrument type.</param>
        /// <param name="timestamp">Note timestamp.</param>
        public static void DecodeProgramChange(UIntPtr dwParam1, UIntPtr dwParam2,
            out Channel channel, out Instrument instrument, out UInt32 timestamp)
        {
            if(!IsProgramChange(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a program change message.");
            }
            channel = (Channel)((int)dwParam1 & 0x0f);
            instrument = (Instrument)(((int)dwParam1 & 0xff00) >> 8);
            timestamp = (UInt32)dwParam2;
        }

        /// <summary>
        /// Encodes a Program Change short message.
        /// </summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="instrument">Instrument type.</param>
        /// <returns>A value that can be passed to midiOutShortMsg.</returns>
        public static UInt32 EncodeProgramChange(Channel channel, Instrument instrument)
        {
            channel.Validate();
            instrument.Validate();
            return (UInt32)(0xC0 | (int)(channel) | ((int)instrument << 8));
        }

        #endregion

        #region Public Static Methods [PitchBend]

        /// <summary>
        /// Returns true if the given MidiInProc params describe a Pitch Bend message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsPitchBend(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf0) == 0xE0;
        }

        /// <summary>
        /// Decodes a Pitch Bend message based on MidiInProc params.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="value">Pitch bend value; 0..16383, 8192 is centered.</param>
        /// <param name="timestamp">Message timestamp.</param>
        public static void DecodePitchBend(UIntPtr dwParam1, UIntPtr dwParam2,
                               out Channel channel, out int value, out UInt32 timestamp)
        {
            if(!IsPitchBend(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a pitch bend message.");
            }
            channel = (Channel)((int)dwParam1 & 0x0f);
            value = ((((int)dwParam1 >> 9) & 0x3f80) | (((int)dwParam1 >> 8) & 0x7f));
            timestamp = (UInt32)dwParam2;
        }

        /// <summary>
        /// Encodes a Pitch Bend short message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="value">The pitch bend value; 0..16383, 8192 is centered.</param>
        /// <returns>A value that can be passed to midiOutShortMsg.</returns>
        public static UInt32 EncodePitchBend(Channel channel, int value)
        {
            channel.Validate();
            if(value < 0 || value > 16383) {
                throw new ArgumentOutOfRangeException("Value is out of range.");
            }
            return (UInt32)(0xE0 | (int)(channel) | ((value & 0x7f) << 8) |
                ((value & 0x3f80) << 9));
        }

        #endregion

        #region Public Static Methods [TimingClock]

        /// <summary>
        /// Returns true if the given short message is a Timing Clock message.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        public static bool IsRealTime(UIntPtr dwParam1, UIntPtr dwParam2)
        {
            return ((int)dwParam1 & 0xf8) == 0xF8;
        }
        
        /// <summary>
        /// Decodes a Real Time message based on MidiInProc params.
        /// </summary>
        /// <param name="dwParam1">The dwParam1 arg passed to MidiInProc.</param>
        /// <param name="dwParam2">The dwParam2 arg passed to MidiInProc.</param>
        /// <param name="timestamp">Message timestamp.</param>
        public static void DecodeRealTime(UIntPtr dwParam1, UIntPtr dwParam2,
                                          out RealTimeMessageType msgType, 
                                          out UInt32 timestamp)
        {
            if(!IsRealTime(dwParam1, dwParam2)) {
                throw new ArgumentException("Not a real time message.");
            }

            int b = (int)dwParam1 & 0x07;
            switch(b)
            {
                case 0:
                    msgType = RealTimeMessageType.TimingClock;
                    break;
                case 2:
                    msgType = RealTimeMessageType.Start;
                    break;
                case 3:
                    msgType = RealTimeMessageType.Continue;
                    break;
                case 4:
                    msgType = RealTimeMessageType.Stop;
                    break;
                case 6:
                    msgType = RealTimeMessageType.ActiveSensing;
                    break;
                case 7:
                    msgType = RealTimeMessageType.SystemReset;
                    break;
                case 1:
                case 5:
                    msgType = RealTimeMessageType.Undefined;
                    break;
                default:
                    throw new ArgumentException("Invalid realtime message.");
            }
            timestamp = (UInt32)dwParam2;
        }

        #endregion
    }
}
