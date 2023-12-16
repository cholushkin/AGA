using NaughtyAttributes;
using UnityEngine;

public class CellPatternChunkPrefabVisualizer : MonoBehaviour
{
    public GameObject PrefabCell;
    public CellPatternChunk Chunk;

    [Button]
    public void Visualize()
    {
        if(!enabled)
            return;

        var currentVisualization = transform.Find("Visualization");
        if (currentVisualization != null)
        {
            DestroyImmediate(currentVisualization.gameObject);
            currentVisualization = new GameObject("Visualization").transform;
            currentVisualization.transform.parent = transform;
        }

        var bottomLeft = Chunk.transform.position - new Vector3(Chunk.ChunkSize.x * 0.5f, Chunk.ChunkSize.y * 0.5f, 0);

        for (int y = 0; y < Chunk.ChunkSize.y; y++)
            for (int x = 0; x < Chunk.ChunkSize.x; x++)
            {
                if (Chunk.Get(x, y))
                {
                    Instantiate(
                        PrefabCell,
                        bottomLeft + new Vector3(x + 0.5f, y + 0.5f, 0),
                        Quaternion.identity,
                        currentVisualization);
                }
            }
    }

    void OnDrawGizmos()
    {
        // Assuming your script is attached to a game object, get the position
        Vector3 position = transform.position;

        // Calculate the half extents of the AABB
        Vector3 halfExtents = new Vector3(Chunk.ChunkSize.x, Chunk.ChunkSize.y, 1);

        // Draw the AABB wire cube
        Gizmos.color = Color.magenta; // You can choose a different color
        Gizmos.DrawWireCube(position, halfExtents);
    }
}