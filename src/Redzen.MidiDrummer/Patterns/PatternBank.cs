using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedzenMidiDrummer.Patterns
{
    public static class PatternBank
    {
        public static Dictionary<string,Pattern> PatternsById = new Dictionary<string,Pattern>();

        static PatternBank()
        {
            PatternsById.Add("1", CreatePatternA());
        }

        private static Pattern CreatePatternA()
        {
            Pattern pat = new Pattern();
            pat.PatternSequenceList.Add(new PatternSequence(0.25, 1, 1));
            pat.PatternSequenceList.Add(new PatternSequence(0.5, 1, 1));
            pat.PatternSequenceList.Add(new PatternSequence(0.5, 1, 1));
            pat.PatternSequenceList.Add(new PatternSequence(1, 0.5, 1));
            pat.PatternSequenceList.Add(new PatternSequence(0.125, 1, 1));
            pat.PatternSequenceList.Add(new PatternSequence(0.25, 1, 1));
            return pat;
        }

        
    }
}
