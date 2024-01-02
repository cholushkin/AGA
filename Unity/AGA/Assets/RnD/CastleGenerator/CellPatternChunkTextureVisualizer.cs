using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;


namespace CastleGenerator
{
    public class CellPatternChunkTextureVisualizer : MonoBehaviour
    {
        public RawImage Image;
        public CellPatternChunk Chunk;
        public Vector2Int WorldCellSize;


        [Button]
        public void Visualize()
        {
            var texture = new Texture2D(Chunk.ChunkSize.x, Chunk.ChunkSize.y);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            var colors = new Color[Chunk.ChunkSize.x * Chunk.ChunkSize.y];

            GetComponent<RectTransform>().sizeDelta = new Vector2(Chunk.ChunkSize.x * WorldCellSize.x,
                Chunk.ChunkSize.y * WorldCellSize.y);
            Image.texture = texture;

            for (int row = 0; row < texture.height; row++)
            for (int col = 0; col < texture.width; col++)
                colors[row * texture.width + col] = Chunk.Get(col, row) == 1 ? Color.black : Color.white;

            texture.SetPixels(colors);
            texture.Apply();
        }
    }
}