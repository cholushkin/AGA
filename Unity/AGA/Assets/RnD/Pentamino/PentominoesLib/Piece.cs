using System.Collections.Generic;

namespace PentominoesLib
{
    public class Piece
    {
        public Piece(string label, List<Variation> variations)
        {
            Label = label;
            Variations = variations;
        }

        public readonly string Label;
        public readonly List<Variation> Variations;
    }
}