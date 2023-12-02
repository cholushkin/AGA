using NaughtyAttributes;
using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TextureAtlas;

namespace GameLib
{
    [CreateAssetMenu(fileName = "ColorSchemeAtlasCreator", menuName = "GameLib/Color/ColorSchemeAtlasCreator", order = 1)]
    public class ColorSchemeAtlasCreator : ScriptableObject
    {
        public enum AtlasLayout
        {
            OneItemPerRow,
            OneItemPerRowWithNames,
            PackShortItems,
            PackAll,
        }

        public ColorScheme ColorScheme;
        public Color Background;
        public Vector2Int TextureAspects;
        public Vector2Int CellSize;
        public AtlasLayout Layout;

        [Button]
        public void GenerateAtlas()
        {
            Assert.IsTrue(TextureAspects.x > 0);
            Assert.IsTrue(TextureAspects.y > 0);
            Texture2D texture = new Texture2D(TextureAspects.x, TextureAspects.y);
            ClearTexture(texture, Background);

            // Get the directory of the ColorScheme ScriptableObject
            string colorSchemePath = AssetDatabase.GetAssetPath(ColorScheme);
            string directory = Path.GetDirectoryName(colorSchemePath);

            // Construct the path for the new texture next to the ColorScheme
            string textureName = $"{ColorScheme.name}"; // You can modify this as needed
            string path = Path.Combine(directory, $"{textureName}.png");

            DrawCell(texture, 0, 0, Color.red);
            DrawCell(texture, 1, 0, Color.green);
            DrawCell(texture, 2, 0, Color.blue);
            DrawCell(texture, 3, 0, Color.yellow);

            // Save the texture to the specified path
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);

            AssetDatabase.Refresh();
            ColorScheme.Atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Debug.Log("Texture generated at: " + path);
            Debug.Log($"Texture assigned to {ColorScheme.name}.Atlas");
        }

        #region API

        private bool DrawCell(Texture2D texture, int cellX, int cellY, Color color)
        {
            // Calculate the pixel coordinates of the top-left corner of the cell
            int x = cellX * CellSize.x;
            int y = cellY * CellSize.y;

            return DrawColorRect(texture, x, y, color, CellSize.x, CellSize.y);
        }


        // Returns false if the entire rectangle is out of texture bounds
        private bool DrawColorRect(Texture2D texture, int x, int y, Color color, int squareWidth, int squareHeight)
        {
            // Calculate the clipped region to ensure it's within bounds
            int clippedX = Mathf.Clamp(x, 0, texture.width - 1);
            int clippedY = Mathf.Clamp(y, 0, texture.height - 1);
            int clippedWidth = Mathf.Clamp(x + squareWidth, 0, texture.width) - clippedX;
            int clippedHeight = Mathf.Clamp(y + squareHeight, 0, texture.height) - clippedY;

            // Check if the entire rectangle is out of bounds
            if (clippedWidth <= 0 || clippedHeight <= 0)
            {
                // The entire rectangle is out of bounds
                return false;
            }

            for (int i = clippedX; i < clippedX + clippedWidth; i++)
            {
                for (int j = clippedY; j < clippedY + clippedHeight; j++)
                {
                    texture.SetPixel(i, j, color);
                }
            }

            // At least part of the rectangle drawn successfully within bounds
            return true;
        }

        private void ClearTexture(Texture2D texture, Color clearColor)
        {
            for (int x = 0; x < texture.width; x++)
                for (int y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, clearColor);
        }
        #endregion
    }
}