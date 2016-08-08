using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedzenMidiDrummer.Patterns
{
    public class PatternSequence
    {
        public double LengthFactor;
        public double QuantizationFactor;
        public double ProbabilityFactor;

        public PatternSequence(double lengthFactor, double quantizationFactor, double probabilityFactor)
        {
            this.LengthFactor = lengthFactor;
            this.QuantizationFactor = quantizationFactor;
            this.ProbabilityFactor = probabilityFactor;
        }
    }
}
