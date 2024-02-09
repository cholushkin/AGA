using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameLib.Log;


namespace CastleGenerator.Tier0
{
    // The main class responsible for generating a castle represented as a set of cells
    public class CellGeneratorController : MonoBehaviour
    {
        public enum ResultStatus
        {
            Processing,
            Success,
            Failed
        }

        public bool RemoveDiagonals;
        public byte[,] Data { get; private set; }
        public ResultStatus Status { get; private set; }
        public CellPattern CellPattern { get; private set; }

        private LogChecker _log;

        public void Init(LogChecker log)
        {
            _log = log;
        }

        public async UniTask<byte[,]> Generate(CellPattern cellPattern)
        {
            _log.Print(LogChecker.Level.Normal, "[method]CellGeneratorController.Generate");
            _log.Print(LogChecker.Level.Verbose,$"CellGeneratorController uses '{cellPattern.name}' CellPattern");
            
            Status = ResultStatus.Processing;
            CellPattern = cellPattern;

            if (cellPattern.Chunks.Count < 1)
            {
                Debug.LogError($"No chunks");
                Status = ResultStatus.Failed;
                return null;
            }

            _log.Print(LogChecker.Level.Verbose, $"Bounds size = {cellPattern.Bounds.size}");

            foreach (CellChunkBase chunk in cellPattern.Chunks)
                chunk.Generate();

            Data = Merge(cellPattern.Bounds, cellPattern.ObligatoryRects.Concat(cellPattern.OptionalRects).ToList(), cellPattern);
            var floodFill = new FloodFill();
            floodFill.Fill(Data, cellPattern.BasementRect, cellPattern.ObligatoryRects.Select(x=>x.Item1).ToList(), RemoveDiagonals);
            _log.Print($"FloodFill result: {floodFill.FloodFillStatus}");
            Status = floodFill.FloodFillStatus == FloodFill.Status.Pass ? 
                ResultStatus.Success : ResultStatus.Failed;

            return Data;
        }

        private byte[,] Merge(Bounds bounds, List<(Rect,CellChunkBase)> rects, CellPattern cellPattern)
        {
            byte[,] data = new byte[Mathf.RoundToInt(bounds.size.x), Mathf.RoundToInt(bounds.size.y)];

            foreach (var rc in rects)
            {
                var rect = rc.Item1;
                var chunk = rc.Item2;
                var chunkSize = chunk.GetSize();

                for (int x = 0; x < chunkSize.width; ++x)
                    for (int y = 0; y < chunkSize.height; ++y)
                        data[(int) (rect.xMin + x), (int) rect.yMin + y] = (chunk.GetCell(x, y));
            }
            return data;
        }
    }
}