using System;
using System.Linq;
using System.Collections.Generic;
using DlxLib;

namespace PentominoesLib
{
    public static class Pentominoes
    {
        class State
        {
            public State()
            {
                MaybeSolution = null;
                UniqueBoards = new List<string>();
            }

            public State(List<string> uniqueBoards)
            {
                MaybeSolution = null;
                UniqueBoards = uniqueBoards;
            }

            public State(List<Placement> solution, List<string> uniqueBoards)
            {
                MaybeSolution = solution;
                UniqueBoards = uniqueBoards;
            }

            public readonly List<Placement> MaybeSolution;
            public readonly List<string> UniqueBoards;
        }


        public static IEnumerable<List<Placement>> Solve()
        {
            var rows = BuildRows();
            var matrix = BuildMatrix(rows);
            var dlx = new Dlx();
            var allSolutions = dlx.Solve(matrix, d => d, r => r);

            var result = new List<List<Placement>>();
            var uniqueBoards = new List<string>();

            foreach (var solution in allSolutions)
            {
                var resolvedSolution = ResolveSolution(rows)(solution);
                var formattedBoard = FormatBoardOneLine(resolvedSolution);

                if (SolutionDeDuplicator.SolutionIsUnique(resolvedSolution, uniqueBoards))
                {
                    var state = new State(resolvedSolution, uniqueBoards);
                    uniqueBoards.Add(formattedBoard);
                    if (state.MaybeSolution != null)
                        result.Add(state.MaybeSolution);
                }
                else
                {
                    uniqueBoards.Add(formattedBoard);
                }
            }

            return result;
        }

        public static List<string> FormatSolution(List<Placement> solution)
        {
            var seed = new List<(int x, int y, string label)>
            {
                (3, 3, " "),
                (3, 4, " "),
                (4, 3, " "),
                (4, 4, " ")
            };

            var cells = new List<(int x, int y, string label)>();
            foreach (var placement in solution)
            {
                foreach (var coords in placement.Variation.Coords)
                {
                    var x = placement.Location.X + coords.X;
                    var y = placement.Location.Y + coords.Y;
                    cells.Add((x, y, placement.Piece.Label));
                }
            }

            var lines = new List<string>();
            for (int y = 0; y < 8; y++)
            {
                var row = new List<(int x, string label)>();
                for (int x = 0; x < 8; x++)
                {
                    var cell = cells.First(t => t.x == x && t.y == y);
                    row.Add((x, cell.label));
                }
                lines.Add(string.Join("", row.Select(t => t.label)));
            }

            return lines;
        }


        private static string FormatBoardOneLine(List<Placement> placements)
        {
            return string.Join("|", FormatSolution(placements));
        }


        private static Func<List<Placement>, Func<Solution, List<Placement>>> ResolveSolution =
            rows =>
                solution =>
                    solution.RowIndexes.Select(rowIndex => rows[rowIndex]).ToList();


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

        private static List<Coords> AllLocations()
        {
            var locations = new List<Coords>();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    locations.Add(new Coords(x, y));
                }
            }
            return locations;
        }

        private static List<Placement> AllPlacements()
        {
            var placements = new List<Placement>();
            foreach (var piece in Pieces.AllPieces())
            {
                foreach (var variation in piece.Variations)
                {
                    foreach (var location in AllLocations())
                    {
                        placements.Add(new Placement(piece, variation, location));
                    }
                }
            }
            return placements;
        }


        private static List<Placement> BuildRows()
        {
            var validPlacements = new List<Placement>();
            foreach (var placement in AllPlacements())
            {
                if (PlacementIsValid(placement))
                {
                    validPlacements.Add(placement);
                }
            }
            return validPlacements;
        }

        private static List<int> MakePieceColumns(Placement placement)
        {
            var allPieces = Pieces.AllPieces();
            var pieceIndex = allPieces.FindIndex(piece => piece.Equals(placement.Piece));
            var pieceColumns = new List<int>();

            for (int index = 0; index < allPieces.Count; index++)
            {
                pieceColumns.Add(index == pieceIndex ? 1 : 0);
            }

            return pieceColumns;
        }

        private static List<int> MakeLocationColumns(Placement placement)
        {
            var locationIndices = placement.Variation.Coords.Select(coords =>
            {
                var x = placement.Location.X + coords.X;
                var y = placement.Location.Y + coords.Y;
                return y * 8 + x;
            });

            var locationColumns = new List<int>();
            var excludeIndices = new[] { 27, 28, 35, 36 };

            for (int index = 0; index < 64; index++)
            {
                if (!excludeIndices.Contains(index))
                {
                    locationColumns.Add(locationIndices.Contains(index) ? 1 : 0);
                }
            }

            return locationColumns;
        }


        private static List<List<int>> BuildMatrix(List<Placement> rows)
        {
            var matrix = new List<List<int>>();

            foreach (var placement in rows)
            {
                var pieceColumns = MakePieceColumns(placement);
                var locationColumns = MakeLocationColumns(placement);
                matrix.Add(new List<int>(pieceColumns.Concat(locationColumns)));
            }

            return matrix;
        }

    }
}
