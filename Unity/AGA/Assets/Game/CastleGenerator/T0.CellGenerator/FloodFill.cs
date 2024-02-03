using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CastleGenerator.Tier0
{
    public class FloodFill
    {
        public enum Status
        {
            Processing,
            Pass,
            FailBadBasement,
            FailReachAllChunks
        }

        public Status FloodFillStatus;
        public int FilledCounter { get; private set; }

        private const byte Val0 = 0;
        private const byte Val1 = 1; 
        private const byte Val2 = 2;


        public void Fill(byte[,] data, Rect basementRect, List<Rect> obligatoryRects, bool removeDiagonals)
        {
            FloodFillStatus = Status.Processing;
            FilledCounter = 0;

            var basement = GetBasement(data, basementRect);

            if (basement.index != -1 && basement.length >= 2)
            {
                Fill(data, basement.entranceIndex, 0, Val2);
                if (removeDiagonals)
                    RemoveDiagonals(data);
            }
            else
            {
                FloodFillStatus = Status.FailBadBasement;
                return;
            }

            // Check visited status
            {
                static bool IsVisited(Rect rect, byte[,] data)
                {
                    for (int y = (int) rect.yMin; y < rect.yMin + rect.height; ++y)
                    for (int x = (int) rect.xMin; x < rect.xMin + rect.width; ++x)
                        if (data[x, y] == Val2)
                            return true;
                    return false; // If not found, continue with the rest of the code
                }

                foreach (var rect in obligatoryRects)
                {
                    if (!IsVisited(rect, data))
                    {
                        FloodFillStatus = Status.FailReachAllChunks;
                        return;
                    }
                }
            }

            FloodFillStatus = Status.Pass;
        }

        private void RemoveDiagonals(byte[,] data)
        {
            int width = data.GetLength(0); // x|width|cols
            int height = data.GetLength(1); // y|height|rows

            for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                if(data[x,y] != Val2)
                    continue;
                if (x - 1 >= 0 && y - 1 >= 0)
                {
                    var leftDiag = data[x - 1, y - 1];
                    if (leftDiag == Val2 && data[x, y - 1] != Val2 && data[x - 1, y] != Val2)
                        data[x, y - 1] = Val2;
                }
                if (x + 1 < width && y - 1 >= 0 )
                {
                    var rightDiag = data[x + 1, y - 1];
                    if (rightDiag == Val2 && data[x, y - 1] != Val2 && data[x + 1, y] != Val2)
                        data[x, y - 1] = Val2;
                }
            }
        }

        private void Fill(byte[,] data, int startx, int starty, byte value)
        {
            List<(int, int)> GetNeighbours(int x, int y)
            {
                int rows = data.GetLength(0);
                int cols = data.GetLength(1);

                List<(int, int)> result = new List<(int, int)>();

                if (x - 1 >= 0) // Add left neighbor
                    result.Add((x - 1, y));

                if (y - 1 >= 0) // Add top neighbor
                    result.Add((x, y - 1));

                if (x + 1 < rows) // Add right neighbor
                    result.Add((x + 1, y));

                if (y + 1 < cols) // Add bottom neighbor
                    result.Add((x, y + 1));

                return result;
            }

            void FillConnected(int x, int y)
            {
                // Check if the current element is within bounds and has the value 1
                if (x >= 0 && x < data.GetLength(0) && y >= 0 && y < data.GetLength(1) && data[x, y] == Val1)
                {
                    // Fill the current element with the specified value
                    data[x, y] = value;
                    FilledCounter++;

                    // Get the neighbors
                    List<(int, int)> neighbours = GetNeighbours(x, y);

                    // Recursively fill the connected neighbors
                    foreach (var neighbour in neighbours)
                        FillConnected(neighbour.Item1, neighbour.Item2);
                }
            }

            // Fill the connected elements starting from the specified coordinates
            FillConnected(startx, starty);
        }

        private (int index, int length, int entranceIndex) GetBasement(byte[,] data, Rect basementRect)
        {
            int columns = data.GetLength(0);
            int rows = data.GetLength(1);

            int curCounter = 0;
            int max = 0;
            int maxStartIndex = -1;
            for (int x = 0; x < columns; ++x)
            {
                if (data[x, 0] == Val1)
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