using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
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
        private List<CellPatternChunkBase> _chunks;
        public Bounds Bounds { get; private set; }
        public List<Rect> Rects { get; private set; }   
        public ResultStatus Status;

        [Button]
        public byte[,] Generate()
        {
            Debug.Log("CellGeneratorController.Generate");
            Status = ResultStatus.Success;
            _chunks = GetChunksInChildren();
            _chunks = _chunks.OrderBy(chunk => chunk is CellPatternChunkClone).ToList();

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

            foreach (CellPatternChunkBase chunk in _chunks)
                chunk.Generate();

 

            var data = Merge(bounds, rects);

            Data = data;
            OnGenerate?.Invoke();

            return data;
        }


        private List<Rect> GetChunkRects(Bounds bounds)
        {
            var chunkRects = new List<Rect>(_chunks.Count);
            foreach (CellPatternChunkBase chunk in _chunks)
            {
                var curChunkOffset = chunk.transform.position - bounds.min;
                var chunkOffsetX = Mathf.RoundToInt(curChunkOffset.x - chunk.GetChunkSize().x * 0.5f);
                var chunkOffsetY = Mathf.RoundToInt(curChunkOffset.y - chunk.GetChunkSize().y * 0.5f);

                chunkRects.Add(new Rect(chunkOffsetX, chunkOffsetY, chunk.GetChunkSize().x, chunk.GetChunkSize().y));
            }

            return chunkRects;
        }

        private byte[,] Merge(Bounds bounds, List<Rect> rects)
        {
            byte[,] data = new byte[(int)bounds.size.x, (int)bounds.size.y];
            int i = 0;

            foreach (CellPatternChunkBase chunk in _chunks)
            {
                var rect = rects[i++];

                for (int x = 0; x < chunk.GetChunkSize().x; ++x)
                    for (int y = 0; y < chunk.GetChunkSize().y; ++y)
                        data[(int)(rect.xMin + x), (int)rect.yMin + y] = (chunk.Get(x, y));
            }
            return data;
        }

        private Bounds GetAABB(List<CellPatternChunkBase> chunks)
        {
            var bounds = new Bounds();
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;

            foreach (CellPatternChunkBase chunk in chunks)
            {
                var width = chunk.GetChunkSize().x;
                var height = chunk.GetChunkSize().y;
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

        private List<CellPatternChunkBase> GetChunksInChildren()
        {
            _chunks = transform.GetComponentsInChildren<CellPatternChunkBase>().ToList();
            Debug.Log($"Found {_chunks.Count} chunks");
            return _chunks;
        }


        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Vector3 position = transform.position;
            Gizmos.color = Color.magenta; // You can choose a different color
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }

        public void MutateParameters(IPseudoRandomNumberGenerator rnd)
        {
            var chunks = GetChunksInChildren();
            foreach (var cellPatternChunk in chunks)
            {
                var cBase = cellPatternChunk as CellPatternChunk;
                if (cBase)
                    cBase.Seed = rnd.ValueInt();
            }
        }
    }


}
