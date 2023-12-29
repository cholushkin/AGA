using System.Collections.Generic;
using System.Linq;

namespace PentominoesLib
{
    public static class Pieces
    {
        public static IEnumerable<Piece> AllPieces
        {
            get
            {
                List<Piece> pieces = new List<Piece>();
                foreach (var pieceDescription in PieceDescriptions.AllPieceDescriptions)
                {
                    pieces.Add(MakePiece(pieceDescription));
                }
                return pieces;
            }
        }

        private static Piece MakePiece(PieceDescription pieceDescription)
        {
            var northPattern = pieceDescription.Pattern;
            var westPattern = StringManipulations.RotateStrings(northPattern);
            var southPattern = StringManipulations.RotateStrings(westPattern);
            var eastPattern = StringManipulations.RotateStrings(southPattern);
            var northReflectedPattern = StringManipulations.ReflectStrings(northPattern);
            var westReflectedPattern = StringManipulations.ReflectStrings(westPattern);
            var southReflectedPattern = StringManipulations.ReflectStrings(southPattern);
            var eastReflectedPattern = StringManipulations.ReflectStrings(eastPattern);

            var allPatternVariations = new List<PatternVariation>
            {
                new PatternVariation(Orientation.North, false, northPattern),
                new PatternVariation(Orientation.West, false, westPattern),
                new PatternVariation(Orientation.South, false, southPattern),
                new PatternVariation(Orientation.East, false, eastPattern),
                new PatternVariation(Orientation.North, true, northReflectedPattern),
                new PatternVariation(Orientation.West, true, westReflectedPattern),
                new PatternVariation(Orientation.South, true, southReflectedPattern),
                new PatternVariation(Orientation.East, true, eastReflectedPattern)
            };

            var uniquePatternVariations = new List<PatternVariation>();
            var comparer = new PatternVariationComparer();
            foreach (var upv in allPatternVariations)
            {
                if (!uniquePatternVariations.Exists(pv => comparer.Equals(pv, upv)))
                {
                    uniquePatternVariations.Add(upv);
                }
            }

            var uniqueVariations = new List<Variation>();
            foreach (var upv in uniquePatternVariations)
            {
                uniqueVariations.Add(new Variation(upv.Orientation, upv.Reflected, PatternToCoords(upv.Pattern)));
            }

            return new Piece(pieceDescription.Label, uniqueVariations);
        }

        private static IEnumerable<Coords> PatternToCoords(IEnumerable<string> pattern)
        {
            List<Coords> result = new List<Coords>();
            var patternList = new List<string>(pattern);
            for (int x = 0; x < patternList[0].Length; x++)
            {
                for (int y = 0; y < patternList.Count; y++)
                {
                    if (patternList[y][x] == 'X')
                    {
                        result.Add(new Coords(x, y));
                    }
                }
            }
            return result;
        }
    }

    class PatternVariation
    {
        public PatternVariation(Orientation orientation, bool reflected, IEnumerable<string> pattern)
        {
            Orientation = orientation;
            Reflected = reflected;
            Pattern = pattern;
        }

        public readonly Orientation Orientation;
        public readonly bool Reflected;
        public readonly IEnumerable<string> Pattern;
    }

    class PatternVariationComparer : IEqualityComparer<PatternVariation>
    {
        public bool Equals(PatternVariation pv1, PatternVariation pv2)
        {
            return pv1.Pattern.SequenceEqual(pv2.Pattern);
        }

        public int GetHashCode(PatternVariation pv)
        {
            return 0;
        }
    }
}
