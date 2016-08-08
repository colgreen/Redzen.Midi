# Redzen.Midi
.NET / C# MIDI library and MIDI drummer console application.

**Redzen.Midi** is a library project that supplies access to some MIDI functionality in .NET via Win32 API calls.

**Redzen.Midi.Drummer** is a console application that allows keyboard control of creation and modification of drum sequences.

Currently the drummer app allows creation of new random sequences, the menu options gives some idea of the current functionality...

```
~~~ Redzen MIDI Sequencer Programmer ~~~

 --- control ---  
 s     : start/stop (ctrl-s: pause/continue)  
 +/-   : tempo up/down (internal clock mode only)  
 

 --- state ---
 c=n   : set channel
 lc=n  : set sequence length (in clock ticks, 24 ticks per beat)
 lb=n  : set sequence length (in beats)
 q=n   : set sequence quantization (in clock ticks)
 n=60  : set note (0-127)
 p=5   : set note probability (stated as an inverse, i.e. 5 => 1/5th
 t     : prinT state

 --- sequences ---
 a     : add new sequence
 d 1   : delete sequence 1
 m 1   : mute sequence 1
 u 1   : un-mute sequence 1
 o 1   : solo sequence 1
 r     : pRint sequences

 --- patterns ---
 ns=60 : assign pattern notes (e.g. ns=60,61-63 ns=nordrum2 ns=volcabeats)
 j     : print pattern notes
 1     : add pattern A
 2     : add pattern B
 3     : add pattern C

 --- other ---
 h     : help
 x     : exit
>
```
