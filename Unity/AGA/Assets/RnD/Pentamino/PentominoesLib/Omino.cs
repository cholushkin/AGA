using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DlxLib;

namespace PentominoesLib
{
    public class Omino
    {
        private int _solutionCounter;
        public IEnumerable<Solution> Solve(Action<IEnumerable<Placement>, Solution, int> onSolutionFound, int maxSolutions)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            List<Placement> rows = BuildRows();
            List<List<int>> matrix = BuildMatrix(rows);
            Dlx dlx = new Dlx(cancellationTokenSource.Token);
            dlx.SolutionFound += (_, e) =>
            {
                if (onSolutionFound != null)
                    onSolutionFound(rows, e.Solution, e.SolutionIndex);
                _solutionCounter++;
                if (_solutionCounter >= maxSolutions)
                    cancellationTokenSource.Cancel();

            };
            return dlx.Solve(matrix, d => d, r => r);
        }

        private bool PlacementIsValid(Placement placement)
        {
            foreach (var coords in placement.Variation.Coords)
            {
                var x = placement.Location.X + coords.X;
                var y = placement.Location.Y + coords.Y;
                if (x >= 64 || y >= 64) return false;
            }
            return true;
        }

        private IEnumerable<Coords> AllLocations()
        {
            List<Coords> result = new List<Coords>();
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    result.Add(new Coords(x, y));
                }
            }
            return result;
        }

        private List<Placement> AllPlacements()
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

        private List<Placement> BuildRows()
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

        private IEnumerable<int> MakePieceColumns(Placement placement)
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

        private IEnumerable<int> MakeLocationColumns(Placement placement)
        {
            List<int> result = new List<int>();
            List<int> locationIndices = new List<int>();
            foreach (var coords in placement.Variation.Coords)
            {
                var x = placement.Location.X + coords.X;
                var y = placement.Location.Y + coords.Y;
                locationIndices.Add(y * 8 + x);
            }

            //int[] excludeIndices = { 27, 28, 35, 36 };
            for (int index = 0; index < 64; index++)
            {
              //  if (!Array.Exists(excludeIndices, element => element == index))
                //{
                    result.Add(locationIndices.Contains(index) ? 1 : 0);
                //}
            }

            return result;
        }

        private List<List<int>> BuildMatrix(IEnumerable<Placement> rows)
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
