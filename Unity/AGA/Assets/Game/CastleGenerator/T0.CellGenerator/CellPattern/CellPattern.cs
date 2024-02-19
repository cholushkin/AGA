using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Log;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace CastleGenerator.Tier0
{
    public class CellPattern : MonoBehaviour
    {
        public List<CellChunkBase> Chunks { get; private set; } // Chunks in gen order
        public List<(Rect, CellChunkBase)> ObligatoryRects { get; private set; }
        public List<(Rect, CellChunkBase)> OptionalRects { get; private set; }
        public Rect BasementRect { get; private set; }
        public Bounds Bounds { get; private set; }

        public CellChunkBase Basement;
        private LogChecker _log;

        private const int XBoundsMargin = 5;
        private const int YBoundsMargin = 5;

        public void Init(IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            Assert.IsNotNull(Basement);
            _log = log;
            Chunks = GetChunksInChildren();
            Bounds = GetAABB(Chunks);
            (ObligatoryRects, OptionalRects) = GetChunkRects(Bounds, Chunks);
            PropagateRndToChunks(Chunks, rnd);
            _log.Print(LogChecker.Level.Normal,
                $"CellPattern Init. Name: {name}. Chunks = {Chunks.Count}, Bounds = {Bounds}, Rects = {ObligatoryRects.Count + OptionalRects.Count}");
        }

        private void PropagateRndToChunks(List<CellChunkBase> chunks, IPseudoRandomNumberGenerator rnd)
        {
            foreach (var rndChunk in chunks.OfType<CellChunkRnd>())
                rndChunk.SetRnd(rnd);
        }

        // Returns list of all chunks ordered in generation order (clones go to the end) 
        private List<CellChunkBase> GetChunksInChildren()
        {
            var chunks = transform.GetComponentsInChildren<CellChunkBase>().ToList();
            return chunks.OrderBy(chunk => chunk is CellChunkClone).ToList();
        }

        private (List<(Rect, CellChunkBase)> obligatoryRect, List<(Rect, CellChunkBase)> optionalRects) GetChunkRects(
            Bounds bounds, List<CellChunkBase> chunks)
        {
            var obligatoryChunkRects = new List<(Rect, CellChunkBase)>(chunks.Count);
            var optionalChunkRects = new List<(Rect, CellChunkBase)>(chunks.Count);
            foreach (CellChunkBase chunk in chunks)
            {
                var curChunkOffset = chunk.transform.position - bounds.min;
                var chunkOffsetX = Mathf.RoundToInt(curChunkOffset.x - chunk.GetSize().width * 0.5f);
                var chunkOffsetY = Mathf.RoundToInt(curChunkOffset.y - chunk.GetSize().height * 0.5f);
                var rect = new Rect(chunkOffsetX, chunkOffsetY, chunk.GetSize().width, chunk.GetSize().height);
                if (chunk == Basement)
                    BasementRect = rect;
                if (chunk.OptionalVisit)
                    optionalChunkRects.Add((rect, chunk));
                else
                    obligatoryChunkRects.Add((rect, chunk));
            }

            return (obligatoryChunkRects, optionalChunkRects);
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
            bounds.min = new Vector3(minx - XBoundsMargin, miny, 0f);
            bounds.max = new Vector3(maxx + XBoundsMargin, maxy + YBoundsMargin, 0f);

            return bounds;
        }
    }
}