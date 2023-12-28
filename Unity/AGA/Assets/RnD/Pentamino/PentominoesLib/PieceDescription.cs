using System.Collections.Generic;

namespace PentominoesLib
{
    public class PieceDescription
    {
        public PieceDescription(string label, List<string> pattern)
        {
            Label = label;
            Pattern = pattern;
        }

        public readonly string Label;
        public readonly List<string> Pattern;
    }
}
