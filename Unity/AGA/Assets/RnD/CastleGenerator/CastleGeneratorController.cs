using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CastleGeneratorController : MonoBehaviour
{
    public byte[,] Data { get; private set; }

    public UnityEvent OnGenerate;
    private List<CellPatternChunk> _chunks;
    public Bounds Bounds { get; private set; }

    [Button]
    byte[,] Generate()
    {
        _chunks = GetChunksInChildren();

        var bounds = GetAABB(_chunks);
        Debug.Log($"Bounds size = {bounds.size}");

        Bounds = bounds;

        var rects = GetChunkRects(bounds);

        foreach (CellPatternChunk chunk in _chunks)
            chunk.Generate();


        var data = Merge(bounds, rects);

        var basement = GetBasement(data);

        Fill(data, basement.entranceIndex, 0, 2);

        Data = data;
        OnGenerate?.Invoke();

        return data;
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

                // Get the neighbors
                List<(int, int)> neighbours = GetNeighbours(x, y);

                // Recursively fill the connected neighbors
                foreach (var neighbour in neighbours)
                {
                    FillConnected(neighbour.Item1, neighbour.Item2);
                }
            }
        }

        // Fill the connected elements starting from the specified coordinates
        FillConnected(startx, starty);
    }

    private (int index, int length, int entranceIndex) GetBasement(byte[,] data)
    {
        int columns = data.GetLength(0);
        int rows = data.GetLength(1);

        int curCounter = 0;
        int max = 0;
        int maxStartIndex = 0;
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
                    data[(int) (rect.xMin + x), (int) rect.yMin + y] = (byte) (chunk.Get(x, y) ? 1 : 0);
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vector3 position = transform.position;
        Gizmos.color = Color.magenta; // You can choose a different color
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}
