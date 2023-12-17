using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CastleGenerator
{
    public class CellGeneratorController : MonoBehaviour
    {
        public enum ResultStatus
        {
            Success,
            NoChunks
        }
        public byte[,] Data { get; private set; }

        public UnityEvent OnGenerate;
        private List<CellPatternChunk> _chunks;
        private List<CellPatternChunkClone> _cloneChunks;
        public Bounds Bounds { get; private set; }
        public List<Rect> Rects { get; private set; }
        public ResultStatus Status;

        [Button]
        public byte[,] Generate()
        {
            Debug.Log("CellGeneratorController.Generate");
            Status = ResultStatus.Success;
            _chunks = GetChunksInChildren();
            _cloneChunks = GetCloneChunksInChildren();

            if (_chunks.Count < 1)
            {
                Status = ResultStatus.NoChunks;
                return null;
            }


            var bounds = GetAABB(_chunks);
            Debug.Log($"Bounds size = {bounds.size}");

            Bounds = bounds;

            var rects = GetChunkRects(bounds);
            Rects = rects;

            foreach (CellPatternChunk chunk in _chunks)
                chunk.Generate();

            foreach (var cloneChunk in _cloneChunks)
            {
                cloneChunk.DoCloning();
            }

            var data = Merge(bounds, rects);

            Data = data;
            OnGenerate?.Invoke();

            return data;
        }


        private List<Rect> GetChunkRects(Bounds bounds)
        {
            var chunkRects = new List<Rect>(_chunks.Count);
            foreach (CellPatternChunk chunk in _chunks)
            {
                var curChunkOffset = chunk.transform.position - bounds.min;
                var chunkOffsetX = Mathf.RoundToInt(curChunkOffset.x - chunk.ChunkSize.x * 0.5f);
                var chunkOffsetY = Mathf.RoundToInt(curChunkOffset.y - chunk.ChunkSize.y * 0.5f);

                chunkRects.Add(new Rect(chunkOffsetX, chunkOffsetY, chunk.ChunkSize.x, chunk.ChunkSize.y));
            }

            return chunkRects;
        }

        private byte[,] Merge(Bounds bounds, List<Rect> rects)
        {
            byte[,] data = new byte[(int)bounds.size.x, (int)bounds.size.y];
            int i = 0;

            foreach (CellPatternChunk chunk in _chunks)
            {
                var rect = rects[i++];

                for (int x = 0; x < chunk.ChunkSize.x; ++x)
                    for (int y = 0; y < chunk.ChunkSize.y; ++y)
                        data[(int)(rect.xMin + x), (int)rect.yMin + y] = (byte)(chunk.Get(x, y) ? 1 : 0);
            }
            return data;
        }

        private Bounds GetAABB(List<CellPatternChunk> chunks)
        {
            var bounds = new Bounds();
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;

            foreach (CellPatternChunk chunk in chunks)
            {
                var width = chunk.ChunkSize.x;
                var height = chunk.ChunkSize.y;
                minx = Mathf.Min(minx, chunk.transform.position.x - width * 0.5f);
                miny = Mathf.Min(miny, chunk.transform.position.y - height * 0.5f);
                maxx = Mathf.Max(maxx, chunk.transform.position.x + width * 0.5f);
                maxy = Mathf.Max(maxy, chunk.transform.position.y + height * 0.5f);
            }

            // Set the values of the bounds object
            bounds.min = new Vector3(minx, miny, 0f);
            bounds.max = new Vector3(maxx, maxy, 0f);

            return bounds;
        }

        private List<CellPatternChunk> GetChunksInChildren()
        {
            _chunks = transform.GetComponentsInChildren<CellPatternChunk>().ToList();
            Debug.Log($"Found {_chunks.Count} chunks");
            return _chunks;
        }

        private List<CellPatternChunkClone> GetCloneChunksInChildren()
        {
            _cloneChunks = transform.GetComponentsInChildren<CellPatternChunkClone>().ToList();
            Debug.Log($"Found {_cloneChunks.Count} cloneChunks");
            return _cloneChunks;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Vector3 position = transform.position;
            Gizmos.color = Color.magenta; // You can choose a different color
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }

        public void MutateParameters()
        {
            var chunks = GetChunksInChildren();
            foreach (var cellPatternChunk in chunks)
            {
                cellPatternChunk.Seed = (long) (long.MaxValue * Random.value);
            }
        }
    }


}
