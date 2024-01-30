using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


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
        public byte[,] Data { get; private set; }
        public ResultStatus Status { get; private set; }

        public async UniTask<byte[,]> Generate(CellPattern cellPattern)
        {
            Debug.Log("CellGeneratorController.Generate");
            Debug.Log($"CellGeneratorController uses '{cellPattern.name}' CellPattern");
            
            Status = ResultStatus.Processing;

            if (cellPattern.Chunks.Count < 1)
            {
                Debug.LogError($"No chunks");
                Status = ResultStatus.Failed;
                return null;
            }

            Debug.Log($"Bounds size = {cellPattern.Bounds.size}");

            foreach (CellChunkBase chunk in cellPattern.Chunks)
                chunk.Generate();
            
            Data = Merge(cellPattern.Bounds, cellPattern.Rects, cellPattern);
            var floodFill = new FloodFill();
            floodFill.Fill(Data, cellPattern.BasementRect, cellPattern.Rects);
            Status = floodFill.FloodFillStatus == FloodFill.Status.Pass ? 
                ResultStatus.Success : ResultStatus.Failed;

            return Data;
        }

        private byte[,] Merge(Bounds bounds, List<Rect> rects, CellPattern cellPattern)
        {
            byte[,] data = new byte[(int)bounds.size.x, (int)bounds.size.y];
            int i = 0;

            foreach (CellChunkBase chunk in cellPattern.Chunks)
            {
                var rect = rects[i++];
                var chunkSize = chunk.GetSize();

                for (int x = 0; x < chunkSize.width; ++x)
                    for (int y = 0; y < chunkSize.height; ++y)
                        data[(int)(rect.xMin + x), (int)rect.yMin + y] = (chunk.GetCell(x, y));
            }
            return data;
        }
    }
}