using System.Collections.Generic;

namespace RedzenMidiDrummer.Patterns
{
    public static class PatternBank
    {
        public static Dictionary<string,Pattern> PatternsById = new Dictionary<string,Pattern>();

        static PatternBank()
        {
            PatternsById.Add("1", CreatePatternA());
            PatternsById.Add("2", CreatePatternB());
        }

        private static Pattern CreatePatternA()
        {
            Pattern pat = new Pattern();

            pat.PatternSequenceList.Add(new PatternSequence(1.0,   0.5, 0.4));
            pat.PatternSequenceList.Add(new PatternSequence(0.5,   1.0, 0.4));
            pat.PatternSequenceList.Add(new PatternSequence(0.5,   1.0, 0.4));

            pat.PatternSequenceList.Add(new PatternSequence(0.25,  1.0, 0.4));
            pat.PatternSequenceList.Add(new PatternSequence(0.25,  1.0, 0.4));
            
            pat.PatternSequenceList.Add(new PatternSequence(0.125, 1.0, 0.4));
            return pat;
        }

        private static Pattern CreatePatternB()
        {
            Pattern pat = new Pattern();

            pat.PatternSequenceList.Add(new PatternSequence(1.0,   0.5, 0.2));
            pat.PatternSequenceList.Add(new PatternSequence(0.5,   1.0, 0.2));
            pat.PatternSequenceList.Add(new PatternSequence(0.5,   0.5, 0.2));

            pat.PatternSequenceList.Add(new PatternSequence(0.25,  0.5, 0.2));
            pat.PatternSequenceList.Add(new PatternSequence(0.25,  0.5, 0.2));
            
            pat.PatternSequenceList.Add(new PatternSequence(0.125, 0.5, 0.2));
            pat.PatternSequenceList.Add(new PatternSequence(0.125, 0.5, 0.2));
            return pat;
        }
        
    }
}
