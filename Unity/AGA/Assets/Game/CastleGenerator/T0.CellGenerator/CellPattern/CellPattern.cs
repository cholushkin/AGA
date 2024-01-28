using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CastleGenerator.Tier0
{
    public class CellPattern : MonoBehaviour
    {
        public List<CellChunkBase> Chunks { get; private set; }
        public List<Rect> Rects { get; private set; }
        public Rect BasementRect { get; private set; }
        public Bounds Bounds { get; private set; }
        
        public CellChunkBase Basement;

        public void Init(long seed)
        {
            Assert.IsNotNull(Basement);
            Chunks = GetChunksInChildren();
            Bounds = GetAABB(Chunks);
            Rects = GetChunkRects(Bounds, Chunks);
            PropagateSeed(Chunks, seed);
            Debug.Log($"CellPattern Init. Chunks = {Chunks.Count}, Bounds = {Bounds}, Rects = {Rects.Count}");
        }

        private void PropagateSeed(List<CellChunkBase> chunks, long seed)
        {
            foreach (CellChunkBase chunk in chunks)
            {
                var rndChunk = chunk as CellChunkRnd;
                if (rndChunk != null) 
                    rndChunk.SetSeed(seed);
            }
        }

        // ordered by generation queue (clones go to the end) 
        private List<CellChunkBase> GetChunksInChildren()
        {
            var chunks = transform.GetComponentsInChildren<CellChunkBase>().ToList();
            return chunks.OrderBy(chunk => chunk is CellChunkClone).ToList();
        }
        
        private List<Rect> GetChunkRects(Bounds bounds, List<CellChunkBase> chunks)
        {
            var chunkRects = new List<Rect>(chunks.Count);
            foreach (CellChunkBase chunk in chunks)
            {
                var curChunkOffset = chunk.transform.position - bounds.min;
                var chunkOffsetX = Mathf.RoundToInt(curChunkOffset.x - chunk.GetSize().width * 0.5f);
                var chunkOffsetY = Mathf.RoundToInt(curChunkOffset.y - chunk.GetSize().height * 0.5f);
                var rect = new Rect(chunkOffsetX, chunkOffsetY, chunk.GetSize().width, chunk.GetSize().height);
                if (chunk == Basement)
                    BasementRect = rect;
                chunkRects.Add(rect);
            }

            return chunkRects;
        }
        
        private Bounds GetAABB(List<CellChunkBase> chunks)
        {
            var bounds = new Bounds();
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;

            foreach (CellChunkBase chunk in chunks)
            {
                var width = chunk.GetSize().width;
                var height = chunk.GetSize().height;
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
    }
}