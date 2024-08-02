using System.Collections.Generic;
using NaughtyAttributes;
using System.IO;
using System.Linq;
using TinyColorLib;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Color = UnityEngine.Color;

namespace GameLib.ColorScheme
{
    [CreateAssetMenu(fileName = "ColorSchemeAtlasCreator", menuName = "GameLib/Color/ColorSchemeAtlasCreator", order = 1)]
    public class ColorSchemeAtlasCreator : ScriptableObject
    {
        public enum AtlasLayout
        {
            OneItemPerRow,
            OneItemPerRowWithNames,
            Columns,
            ColumnsWithText,
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

            if (Layout == AtlasLayout.OneItemPerRow || Layout == AtlasLayout.OneItemPerRowWithNames)
            {
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

                    if (Layout == AtlasLayout.OneItemPerRowWithNames)
                        RenderTextToTexture(texture, item.Palette.Length * CellSize.x, row * CellSize.y,
                            " " + item.Name, null);
                    row--;
                }
            }
            else if (Layout == AtlasLayout.Columns || Layout == AtlasLayout.ColumnsWithText)
            {
                var xOffset = 0;
                var maxOnCurrentRow = 0;
                var index = 0;
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
                        if (!DrawCell(texture, xOffset + i, row, item.Palette[i]))
                            Debug.LogWarning($"Drawing out of bounds xy={i},{row}");
                    }

                    Debug.Log($"{index++}." + item.Name);

                    if (item.Palette.Length > maxOnCurrentRow)
                        maxOnCurrentRow = item.Palette.Length;

                    if (Layout == AtlasLayout.ColumnsWithText)
                        RenderTextToTexture(texture, xOffset * CellSize.x, row * CellSize.y, " " + item.Name, null);

                    row--;
                    if (row < 0)
                    {
                        row = texture.height / CellSize.y - 1;
                        xOffset += maxOnCurrentRow + 1; // +1  for 1 cell offset between rows
                        maxOnCurrentRow = 0;
                    }
                }
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
        private bool DrawColorRect(Texture2D texture, int x, int y, Color color, int squareWidth, int squareHeight,
            bool useMargin = true)
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


        Texture2D RenderTextToTexture(Texture2D texture, int x, int y, string text, Color? textColor)
        {
            const int charHeight = 5; // Height based on the pixel size and number of rows in the font
            List<TinyColor> textColors = new List<TinyColor> { new TinyColor(Color.white), new TinyColor(Color.black) };

            text = text.ToUpper();
            int caret = 0;
            int spacing = 1; // Additional space between characters


            for (int i = 0; i < text.Length; i++)
            {
                int[,] pixels;
                if (MiniPixelFontDictionary.Chars.TryGetValue(text[i], out pixels))
                {
                    var curCharColor = textColor;
                    if (!textColor.HasValue) // autocolor
                    {
                        // Get all bg colors using LINQ
                        var bgColors = Enumerable.Range(0, pixels.GetLength(0))
                            .SelectMany(cy => Enumerable.Range(0, pixels.GetLength(1))
                                .Select(cx => texture.GetPixel(caret + x + cx, y + charHeight - cy)))
                            .ToList();

                        // Find the dominant color
                        Color dominantColor = bgColors
                            .GroupBy(color => color)
                            .OrderByDescending(group => group.Count())
                            .First()
                            .Key;

                        // find good readability color for dominant color
                        curCharColor = ReadabilityHelpers.MostReadable(new TinyColor(dominantColor), textColors).ToColor();
                    }

                    for (int cy = 0; cy < pixels.GetLength(0); ++cy)
                    {
                        for (int cx = 0; cx < pixels.GetLength(1); ++cx)
                        {
                            int pixel = pixels[cy, cx];
                            var bgColor = texture.GetPixel(caret + x + cx, y + charHeight - cy);
                            Color color = (pixel == 1) ? curCharColor.Value : bgColor;
                            texture.SetPixel(caret + x + cx, y + charHeight - cy, color);
                        }
                    }

                    int charWidth = pixels.GetLength(1);

                    // Add a column of empty pixels
                    for (int extraY = 0; extraY < charHeight; ++extraY)
                    {
                        var bgColor = texture.GetPixel(caret + charWidth + x, y + charHeight - extraY);
                        texture.SetPixel(caret + charWidth + x, y + charHeight - extraY, bgColor);
                    }

                    caret += charWidth + spacing;
                }
            }

            return texture;
        }

        #endregion
    }
}