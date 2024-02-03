using NaughtyAttributes;
using UnityEngine;

namespace CastleGenerator
{
    public class CellPatternChunkPrefabVisualizer : MonoBehaviour
    {
        public GameObject PrefabCell;
        public CellPatternChunkBase Chunk;

        [Button]
        public void Visualize()
        {
            if (!enabled)
                return;

            var currentVisualization = transform.Find("Visualization");
            if (currentVisualization != null)
            {
                DestroyImmediate(currentVisualization.gameObject);
                currentVisualization = new GameObject("Visualization").transform;
                currentVisualization.transform.parent = transform;
            }

            var bottomLeft = Chunk.transform.position -
                             new Vector3(Chunk.GetChunkSize().x * 0.5f, Chunk.GetChunkSize().y * 0.5f, 0);

            for (int y = 0; y < Chunk.GetChunkSize().y; y++)
            for (int x = 0; x < Chunk.GetChunkSize().x; x++)
            {
                if (Chunk.Get(x, y) == 1)
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
            Vector3 halfExtents = new Vector3(Chunk.GetChunkSize().x, Chunk.GetChunkSize().y, 1);

            // Draw the AABB wire cube
            Gizmos.color = Color.magenta; // You can choose a different color
            Gizmos.DrawWireCube(position, halfExtents);
        }
    }
}