using NaughtyAttributes;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameLib.ColorScheme
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
        public Vector2Int CellMargin;
        public AtlasLayout Layout;

        [Button]
        public void GenerateAtlas()
        {
            Assert.IsNotNull(ColorScheme);
            Assert.IsTrue(TextureAspects.x > 0);
            Assert.IsTrue(TextureAspects.y > 0);
            Texture2D texture = new Texture2D(TextureAspects.x, TextureAspects.y);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            ClearTexture(texture, Background);

            // Get the directory of the ColorScheme ScriptableObject
            string colorSchemePath = AssetDatabase.GetAssetPath(ColorScheme);
            string directory = Path.GetDirectoryName(colorSchemePath);

            // Construct the path for the new texture next to the ColorScheme
            string textureName = $"{ColorScheme.name}"; // You can modify this as needed
            string path = Path.Combine(directory, $"{textureName}.png");

            if (texture.height % CellSize.y != 0f)
                Debug.LogWarning("Texture height is not a multiple of cell height");

            int row = texture.height / CellSize.y - 1;
            foreach (var item in ColorScheme.Data)
            {
                if (row < 0)
                {
                    Debug.LogWarning($"Drawing out of bounds, negative row: {row}");
                    continue;
                }

                // Draw color cells
                for (int i = 0; i < item.Palette.Length; ++i)
                {
                    if (!DrawCell(texture, i, row, item.Palette[i]))
                        Debug.LogWarning($"Drawing out of bounds xy={i},{row}");
                }

                RenderTextToTexture(texture, item.Palette.Length * CellSize.x, row * CellSize.y, " " + item.Name, Color.black);
                row--;
            }

            // Save the texture to the specified path
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

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


        // Returns false if the cell is out of texture bounds
        private bool DrawColorRect(Texture2D texture, int x, int y, Color color, int squareWidth, int squareHeight, bool useMargin = true)
        {
            var margin = useMargin ? 1 : 0;
            for (int ix = x + margin; ix < Mathf.Min(x + squareWidth - margin, texture.width); ix++)
                for (int iy = y + margin; iy < Mathf.Min(y + squareHeight - margin, texture.height); iy++)
                {
                    texture.SetPixel(ix, iy, color);
                }
            return !(x < 0 || y < 0 || x + squareWidth > texture.width || y + squareHeight > texture.height);
        }


        private void ClearTexture(Texture2D texture, Color clearColor)
        {
            for (int x = 0; x < texture.width; x++)
                for (int y = 0; y < texture.height; y++)
                    texture.SetPixel(x, y, clearColor);
        }


        Texture2D RenderTextToTexture(Texture2D texture, int x, int y, string text, Color textColor)
        {
            int charWidth = 5;
            int charHeight = 5; // Height based on the pixel size and number of rows in the font

            text = text.ToUpper();
            int caret = 0;
            int spacing = 1; // Additional space between characters

            for (int i = 0; i < text.Length; i++)
            {
                int[,] pixels;
                if (MiniPixelFontDictionary.Chars.TryGetValue(text[i], out pixels))
                {
                    for (int cy = 0; cy < pixels.GetLength(0); ++cy)
                    {
                        for (int cx = 0; cx < pixels.GetLength(1); ++cx)
                        {
                            int pixel = pixels[cy, cx];
                            Color color = (pixel == 1) ? textColor : Background;
                            texture.SetPixel(caret + x + cx, y + 5 - cy, color);
                        }
                    }
                    charWidth = pixels.GetLength(1);

                    // Add a column of empty pixels
                    for (int extraY = 0; extraY < charHeight; ++extraY)
                    {
                        texture.SetPixel(caret + charWidth + x, y + 5 - extraY, Background);
                    }

                    caret += charWidth + spacing;
                }
            }
            return texture;
        }

        #endregion
    }
}