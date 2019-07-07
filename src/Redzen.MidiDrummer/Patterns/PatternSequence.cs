
namespace RedzenMidiDrummer.Patterns
{
    public class PatternSequence
    {
        public double LengthFactor;
        public double QuantizationFactor;
        public double Probability;

        public PatternSequence(double lengthFactor, double quantizationFactor, double probability)
        {
            this.LengthFactor = lengthFactor;
            this.QuantizationFactor = quantizationFactor;
            this.Probability = probability;
        }
    }
}
