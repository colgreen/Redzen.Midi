using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Midi.Notes;


namespace RedzenMidiDrummer
{
    class Program
    {
        static void Main(string[] args)
        {
            DrummerConsole con = new DrummerConsole();
            con.Run();
        }
    }
}
