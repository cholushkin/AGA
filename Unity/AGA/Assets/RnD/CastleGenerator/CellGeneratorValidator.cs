using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CastleGenerator
{
    public class CellGeneratorValidator : MonoBehaviour
    {
        public enum ValidateStatus
        {
            Pass,
            FailBadBasement,
            FailReachAllChunks
        }

        public bool ValidateAllRectsPass; // fill go through entrance to all rects
        public bool ValidateSaturationThreshold;
        public bool ValidateDiagonalNeighbours;

        public CellGeneratorController Controller;
        public ValidateStatus Status;
        public UnityEvent OnValidate;
        public int CellsCount;
        private HashSet<Rect> _visitedRects;


        public void Validate()
        {
            Status = ValidateStatus.Pass;
            CellsCount = 0;

            var basement = GetBasement(Controller.Data);

            if (basement.index != -1 && basement.length >= 2)
            {
                Fill(Controller.Data, basement.entranceIndex, 0, 2);
            }
            else
            {
                Status = ValidateStatus.FailBadBasement;
            }

            if (_visitedRects.Count != Controller.Rects.Count)
                Status = ValidateStatus.FailReachAllChunks;

            Debug.Log($"Visited rects {_visitedRects.Count}");

            OnValidate?.Invoke();
        }

        private void Fill(byte[,] data, int startx, int starty, byte value)
        {
            List<(int, int)> GetNeighbours(int x, int y)
            {
                int rows = data.GetLength(0);
                int cols = data.GetLength(1);

                List<(int, int)> result = new List<(int, int)>();

                // Add left neighbor
                if (x - 1 >= 0)
                {
                    result.Add((x - 1, y));
                }

                // Add top neighbor
                if (y - 1 >= 0)
                {
                    result.Add((x, y - 1));
                }

                // Add right neighbor
                if (x + 1 < rows)
                {
                    result.Add((x + 1, y));
                }

                // Add bottom neighbor
                if (y + 1 < cols)
                {
                    result.Add((x, y + 1));
                }

                return result;
            }

            void FillConnected(int x, int y)
            {
                // Check if the current element is within bounds and has the value 1
                if (x >= 0 && x < data.GetLength(0) && y >= 0 && y < data.GetLength(1) && data[x, y] == 1)
                {
                    // Fill the current element with the specified value
                    data[x, y] = value;
                    CellsCount++;

                    foreach (var rect in Controller.Rects)
                    {
                        if (rect.Contains(new Vector2(x, y)))
                        {
                            _visitedRects.Add(rect);
                            break;
                        }
                    }
                    

                    // Get the neighbors
                    List<(int, int)> neighbours = GetNeighbours(x, y);

                    // Recursively fill the connected neighbors
                    foreach (var neighbour in neighbours)
                    {
                        FillConnected(neighbour.Item1, neighbour.Item2);
                    }
                }
            }

            _visitedRects = new HashSet<Rect>(Controller.Rects.Count);

            // Fill the connected elements starting from the specified coordinates
            FillConnected(startx, starty);
        }

        private (int index, int length, int entranceIndex) GetBasement(byte[,] data)
        {
            int columns = data.GetLength(0);
            int rows = data.GetLength(1);

            int curCounter = 0;
            int max = 0;
            int maxStartIndex = -1;
            for (int x = 0; x < columns; ++x)
            {
                if (data[x, 0] == 1)
                {
                    curCounter++;
                }
                else
                {
                    // End of sequence
                    var startIndex = x - curCounter;
                    if (curCounter > max)
                    {
                        max = curCounter;
                        maxStartIndex = startIndex;
                    }

                    curCounter = 0;
                }
            }

            return (maxStartIndex, max, maxStartIndex + max / 2);
        }

    }
}
