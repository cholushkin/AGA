using System.Collections.Generic;
using System.Linq;
using CastleGenerator.Tier0;
using Cysharp.Threading.Tasks;
using GameLib.Log;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using CellGeneratorController = CastleGenerator.Tier0.CellGeneratorController;

namespace CastleGenerator.Tier1
{
    public class CastlePolyominoGenerator : MonoBehaviour
    {
        private byte[,] _data;
        private int _width;
        private int _height;
        private CellPattern _cellPattern;
        private PolyominoProvider _polyominoProvider;
        private LogChecker _log;

        private List<(Vector2Int pos, Polyomino polyomino)> _placedPolyominos;
        private int[] _pieceTypeSpawnedCounter; // number of each type of the piece being spawned 
        private List<Vector2Int> _availableCells; // Set of all empty cells available for piece spawning
        private float[] _normProbs; // target percent of each piece (normalized). Example for 3 items: 0.25 0.25 0.5

        private float[]
            _normProbsCurrent; // target percent of each piece (normalized) modified to increase a chance to reach _normProbs. Get updated each spawn

        private IPseudoRandomNumberGenerator _rnd;
        private List<Vector2Int> _perimeter;

        public void Init(CellGeneratorController cellGenerator, PolyominoProvider polyominoProvider,
            IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            _rnd = rnd;
            _polyominoProvider = polyominoProvider;
            _data = cellGenerator.Data;
            _width = _data.GetLength(0);
            _height = _data.GetLength(1);
            _cellPattern = cellGenerator.CellPattern;
            _log = log;
        }

        public async UniTask Generate()
        {
            _log.Print(LogChecker.Level.Normal, "[method]CastleChunkGenerator.Generate");

            // Initialization step
            _availableCells = GetAvailableCells();
            Assert.IsTrue(_availableCells.Count > 0);
            _normProbs = GetNormalizedProbOfEachPoly();
            _normProbsCurrent = (float[]) _normProbs.Clone();
            _pieceTypeSpawnedCounter = new int[_polyominoProvider.Polyominos.Count];
            _placedPolyominos = new List<(Vector2Int pos, Polyomino polyomino)>();
            var initialEmptyCellCount = _availableCells.Count;

            // generation loop
            var fieldCovered = false;
            int safeCounter = 100000;

            while (!fieldCovered)
            {
                // Get random position
                Vector2Int blobCenter = _rnd.FromList(_availableCells);
                _perimeter = new List<Vector2Int> {blobCenter}; // create proxy perimeter


                // Get piece using probability
                var pieceIndex = _rnd.SpawnEvent(_normProbsCurrent);
                var p = _polyominoProvider.Polyominos[pieceIndex];

                var allPieces =
                    _polyominoProvider.Polyominos.OrderBy(p => p.Probability)
                        .ToList(); // todo: check if sort by _normProbsCurrent is better

                // Blobbing
                while (true)
                {
                    var ableToSpawnCurrentPiece = false;
                    foreach (var pv in _perimeter)
                    {
                        var locations = ConvertToGlobalPieces(p, pv);
                        _rnd.ShuffleInplace(locations); // todo: try also without this

                        foreach (var pieceLocationVar in locations)
                        {
                            if (CanSpawnPiece(pieceLocationVar))
                            {
                                PlaceCells(pieceLocationVar);
                                _pieceTypeSpawnedCounter[pieceIndex]++;
                                _placedPolyominos.Add((pv, p));
                                UpdatePerimeter(_perimeter, pieceLocationVar);
                                _normProbsCurrent = UpdateProbabilities(initialEmptyCellCount);

                                Debug.Log(
                                    $"spawnedCounter:{string.Join('|', _pieceTypeSpawnedCounter)} probs({string.Join(' ', _normProbsCurrent)}); piece index: {pieceIndex}");
                                _availableCells.RemoveAll(pieceLocationVar.Contains);
                                ableToSpawnCurrentPiece = true;
                                break;
                            }
                        }

                        if (ableToSpawnCurrentPiece)
                            break;
                    }

                    if (_perimeter.Count == 0)
                        break;

                    if (ableToSpawnCurrentPiece)
                    {
                        // Get piece using probability
                        allPieces = _polyominoProvider.Polyominos.OrderBy(p => p.Probability)
                            .ToList(); // todo: check if sort by _normProbsCurrent is better
                        pieceIndex = _rnd.SpawnEvent(_normProbsCurrent);
                        p = _polyominoProvider.Polyominos[pieceIndex];
                    }
                    else
                    {
                        // Get next piece
                        allPieces.Remove(p);
                        if (allPieces.Count == 0)
                            break;
                        (p, pieceIndex) = GetNexPiece(allPieces);
                    }
                }

                _availableCells.RemoveAll(_perimeter.Contains); // nothing could be spawned on current blob perimeter

                fieldCovered = _availableCells.Count == 0;
                if (safeCounter-- < 0)
                {
                    Debug.LogError("safe counter hit");
                    break;
                }
            }

            Debug.Log("done");
        }

        public List<(Vector2Int pos, Polyomino polyomino)> GetPlacedPolyominos() => _placedPolyominos;

        private List<List<Vector2Int>> ConvertToGlobalPieces(Polyomino omino, Vector2Int pointer)
        {
            var pieces = new List<List<Vector2Int>>();
            foreach (var coord in omino.ShapeCells)
            {
                var piece = new List<Vector2Int>();
                foreach (var offset in omino.ShapeCells)
                {
                    var localCoord = offset - coord;
                    piece.Add(localCoord + pointer);
                }

                pieces.Add(piece);
            }

            return pieces;
        }

        private bool CanSpawnPiece(List<Vector2Int> piece)
        {
            return piece.All(cell => IsVal(cell.x, cell.y, CastleGenerator.Val2));
        }

        private void PlaceCells(List<Vector2Int> piece)
        {
            foreach (var cell in piece)
                _data[cell.x, cell.y] = CastleGenerator.Val3;
        }

        private (Polyomino p, int pieceID) GetNexPiece(List<Polyomino> allPieces)
        {
            var p = allPieces.First();
            allPieces.RemoveAt(0);
            return (p, _polyominoProvider.Polyominos.IndexOf(p));
        }

        private List<Vector2Int> GetAvailableCells()
        {
            var res = new List<Vector2Int>();
            for (int x = 0; x < _width; ++x)
                for (int y = 0; y < _height; ++y)
                    if (IsVal(x, y, CastleGenerator.Val2))
                        res.Add(new Vector2Int(x, y));
            return res;
        }

        private void UpdatePerimeter(List<Vector2Int> perimeter, List<Vector2Int> piece)
        {
            // remove all cells of the piece from the perimeter
            _perimeter.RemoveAll(piece.Contains);

            // add neighbours of the piece (only orthogonal ones) to the perimeter
            foreach (var nCell in piece)
            {
                var c = new Vector2Int(nCell.x - 1, nCell.y);
                if (IsVal(c.x, c.y, CastleGenerator.Val2) && !_perimeter.Contains(c))
                    _perimeter.Add(c);
                c = new Vector2Int(nCell.x + 1, nCell.y);
                if (IsVal(c.x, c.y, CastleGenerator.Val2) && !_perimeter.Contains(c))
                    _perimeter.Add(c);
                c = new Vector2Int(nCell.x, nCell.y - 1);
                if (IsVal(c.x, c.y, CastleGenerator.Val2) && !_perimeter.Contains(c))
                    _perimeter.Add(c);
                c = new Vector2Int(nCell.x, nCell.y + 1);
                if (IsVal(c.x, c.y, CastleGenerator.Val2) && !_perimeter.Contains(c))
                    _perimeter.Add(c);
            }
        }


        // Updates the probability distribution for each piece type based on the spawn count. If the spawn count
        // is zero, the original probability is retained. Otherwise, the probability is adjusted based on the
        // expected and real spawn percentages to maintain a balanced distribution.
        private float[] UpdateProbabilities(int initialEmptyCellCount)
        {
            return _pieceTypeSpawnedCounter
                .Select((cnt, i) => cnt == 0
                    ? _normProbs[i]
                    : _normProbs[i] * (_normProbs[i] * initialEmptyCellCount /
                                       _polyominoProvider.Polyominos[i].ShapeCells.Count) /
                      (cnt / (initialEmptyCellCount * 1f / _polyominoProvider.Polyominos[i].ShapeCells.Count)))
                .ToArray();
        }

        private float[] GetNormalizedProbOfEachPoly()
        {
            float sum = _polyominoProvider.Polyominos.Sum(x => x.Probability);
            return _polyominoProvider.Polyominos.Select(dsc => dsc.Probability / sum).ToArray();
        }

        private bool IsVal(int x, int y, byte val)
        {
            if (!IsInsideGrid(x, y))
                return false;
            return _data[x, y] == val;
        }

        private bool IsInsideGrid(int x, int y)
        {
            // Check if the new position is within the grid bounds
            return (x >= 0 && x < _width && y >= 0 && y < _height);
        }
    }
}