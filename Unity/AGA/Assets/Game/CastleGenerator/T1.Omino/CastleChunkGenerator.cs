using System.Collections.Generic;
using System.Threading.Tasks;
using CastleGenerator.Tier0;
using GameLib.Log;
using UnityEngine;
using CellGeneratorController = CastleGenerator.Tier0.CellGeneratorController;

namespace CastleGenerator.Tier1
{
    public class CastleChunkGenerator : MonoBehaviour
    {
        private byte[,] _data;
        private int _width;
        private int _height;
        private CellPattern _cellPattern;
        private PieceDescriptions _pieceDescriptions;
        private LogChecker _log;

        private List<Vector2Int> _availableCells; // Set of all empty cells available for piece spawning

        public void Init(CellGeneratorController cellGenerator, PieceDescriptions pieceDescripions, LogChecker log)
        {
            _pieceDescriptions = pieceDescripions;
            _data = cellGenerator.Data;
            _width = _data.GetLength(0);
            _height = _data.GetLength(1);
            _cellPattern = cellGenerator.CellPattern;
            _log = log;
        }

        public async Task Generate()
        {
            // Initialization step
            _availableCells = GetAvailableCells();
            // _normProbs = GetPercentageOfAllPieces();
            // _normProbsCurrent = (float[]) _normProbs.Clone();
            // _pieceTypeSpawnedCounter = new int[PieceDescriptions.AllPieceDescriptions.Length];
            var initialEmptyCellCount = _availableCells.Count;

            _log.Print(LogChecker.Level.Normal, "[method]CastleChunkGenerator.Generate");
        }


        private List<Vector2Int> GetAvailableCells()
        {
            var res = new List<Vector2Int>();

            for (int x = 0; x < _width; ++x)
            for (int y = 0; y < _height; ++y)
            {
                if (IsEmptyCell(x, y))
                    res.Add(new Vector2Int(x, y));
            }

            return res;
        }

        // private float[] GetPercentageOfAllPieces()
        // {
        //     float[] probsNorm = new float[PieceDescriptions.AllPieceDescriptions.Count()];
        //     float sum = PieceDescriptions.AllPieceDescriptions.Sum(allPieceDescription =>
        //         allPieceDescription.Probability);
        //     int i = 0;
        //     foreach (var dsc in PieceDescriptions.AllPieceDescriptions)
        //         probsNorm[i++] = dsc.Probability / sum;
        //     return probsNorm;
        // }

        private bool IsEmptyCell(int x, int y)
        {
            if (IsInsideGrid(x, y))
                return false;
            return _data[x, y] == 0;
        }

        private bool IsInsideGrid(int x, int y)
        {
            // Check if the new position is within the grid bounds
            return (x >= 0 && x < _width && y >= 0 && y < _height);
        }

    }
}