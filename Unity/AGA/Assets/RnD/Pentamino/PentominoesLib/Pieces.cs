using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PentominoesLib
{
    public static class Pieces
    {
        public static List<Piece> AllPieces()
        {
            var pieces = new List<Piece>();
            foreach (var pieceDescription in PieceDescriptions.AllPieceDescriptions)
            {
                pieces.Add(MakePiece(pieceDescription));
            }
            return pieces;
        }


        private static Piece MakePiece(PieceDescription pieceDescription)
        {
            var northPattern = pieceDescription.Pattern.ToList();
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

            foreach (var patternVariation in allPatternVariations)
            {
                if (!uniquePatternVariations.Any(pv => comparer.Equals(pv, patternVariation)))
                {
                    uniquePatternVariations.Add(patternVariation);
                }
            }

            var uniqueVariations = uniquePatternVariations
                .Select(upv => new Variation(upv.Orientation, upv.Reflected, PatternToCoords(upv.Pattern)))
                .ToList();

            return new Piece(pieceDescription.Label, uniqueVariations);
        }


        private static List<Coords> PatternToCoords(List<string> pattern)
        {
            var coords = new List<Coords>();
            var w = pattern[0].Length;
            var h = pattern.Count;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (pattern[y][x] == 'X')
                    {
                        coords.Add(new Coords(x, y));
                    }
                }
            }

            return coords;
        }
    }

    class PatternVariation
    {
        public PatternVariation(Orientation orientation, bool reflected, List<string> pattern)
        {
            Orientation = orientation;
            Reflected = reflected;
            Pattern = pattern;
        }

        public readonly Orientation Orientation;
        public readonly bool Reflected;
        public readonly List<string> Pattern;
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
