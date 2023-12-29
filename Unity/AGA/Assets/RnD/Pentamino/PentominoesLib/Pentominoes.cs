using System;
using System.Collections.Generic;
using System.Linq;
using DlxLib;

namespace PentominoesLib
{
    public static class Pentominoes
    {
        public static IEnumerable<Solution> Solve(Action<IEnumerable<Placement>, Solution, int> onSolutionFound)
        {
            List<Placement> rows = BuildRows();
            List<List<int>> matrix = BuildMatrix(rows);
            Dlx dlx = new Dlx();
            dlx.SolutionFound += (_, e) =>
            {
                if (onSolutionFound != null)
                    onSolutionFound(rows, e.Solution, e.SolutionIndex);
            };
            return dlx.Solve(matrix, d => d, r => r);
        }

        private static bool PlacementIsValid(Placement placement)
        {
            foreach (var coords in placement.Variation.Coords)
            {
                var x = placement.Location.X + coords.X;
                var y = placement.Location.Y + coords.Y;
                if (x >= 8 || y >= 8) return false;
                if ((x == 3 || x == 4) && (y == 3 || y == 4)) return false;
            }
            return true;
        }

        private static IEnumerable<Coords> AllLocations()
        {
            List<Coords> result = new List<Coords>();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    result.Add(new Coords(x, y));
                }
            }
            return result;
        }

        private static List<Placement> AllPlacements()
        {
            List<Placement> result = new List<Placement>();
            foreach (var piece in Pieces.AllPieces)
            {
                foreach (var variation in piece.Variations)
                {
                    foreach (var location in AllLocations())
                    {
                        result.Add(new Placement(piece, variation, location));
                    }
                }
            }
            return result;
        }

        private static List<Placement> BuildRows()
        {
            List<Placement> result = new List<Placement>();
            foreach (var placement in AllPlacements())
            {
                if (PlacementIsValid(placement))
                {
                    result.Add(placement);
                }
            }
            return result;
        }

        private static IEnumerable<int> MakePieceColumns(Placement placement)
        {
            List<Piece> piecesList = Pieces.AllPieces.ToList();
            int pieceIndex = -1;
            for (int i = 0; i < piecesList.Count; i++)
            {
                if (piecesList[i].Label == placement.Piece.Label)
                {
                    pieceIndex = i;
                    break;
                }
            }
            List<int> result = new List<int>();
            for (int i = 0; i < piecesList.Count; i++)
            {
                result.Add(i == pieceIndex ? 1 : 0);
            }
            return result;
        }

        private static IEnumerable<int> MakeLocationColumns(Placement placement)
        {
            List<int> result = new List<int>();
            List<int> locationIndices = new List<int>();
            foreach (var coords in placement.Variation.Coords)
            {
                var x = placement.Location.X + coords.X;
                var y = placement.Location.Y + coords.Y;
                locationIndices.Add(y * 8 + x);
            }

            int[] excludeIndices = { 27, 28, 35, 36 };
            for (int index = 0; index < 64; index++)
            {
                if (!Array.Exists(excludeIndices, element => element == index))
                {
                    result.Add(locationIndices.Contains(index) ? 1 : 0);
                }
            }

            return result;
        }

        private static List<List<int>> BuildMatrix(IEnumerable<Placement> rows)
        {
            List<List<int>> result = new List<List<int>>();
            foreach (var placement in rows)
            {
                List<int> pieceColumns = new List<int>(MakePieceColumns(placement));
                List<int> locationColumns = new List<int>(MakeLocationColumns(placement));
                pieceColumns.AddRange(locationColumns);
                result.Add(pieceColumns);
            }
            return result;
        }
    }
}
