using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastleGeneratorController : MonoBehaviour
{
    private List<CellPatternChunk> _chunks;
    private Bounds _bounds;


    [Button]
    byte[,] Generate()
    {
        _chunks = GetChunksInChildren();

        _bounds = GetAABB(_chunks);

        foreach (CellPatternChunk chunk in _chunks)
        {
            chunk.Generate();
        }


        // todo: move to method
        // copy to one array
        byte[,] data = new byte[(int)_bounds.size.x, (int)_bounds.size.y];
        foreach (CellPatternChunk chunk in _chunks)
        {
            var curChunkOffset = chunk.transform.position - _bounds.min;
            var chunkOffsetX = Mathf.RoundToInt(curChunkOffset.x - chunk.ChunkSize.x * 0.5f);
            var chunkOffsetY = Mathf.RoundToInt(curChunkOffset.y - chunk.ChunkSize.y * 0.5f);

            print($"{chunk.gameObject.name} BL corner offset {chunkOffsetX } {chunkOffsetY}");

            for (int x = 0; x < chunk.ChunkSize.x; ++x)
            {
                for (int y = 0; y < chunk.ChunkSize.y; ++y)
                {
                    //print($"{chunk.gameObject.name} {chunkOffsetX + x} {chunkOffsetY + y}");
                    data[chunkOffsetX + x, chunkOffsetY + y] = (byte)(chunk.Get(x, y) ? 1 : 0);


                }
            }
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
        Gizmos.DrawWireCube(position + _bounds.center, _bounds.size);
    }
}
