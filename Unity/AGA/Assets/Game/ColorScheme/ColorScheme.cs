using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib.ColorScheme
{
    // Create a new ColorScheme ScriptableObject for managing color scheme
    [CreateAssetMenu(fileName = "ColorScheme", menuName = "GameLib/Color/ColorScheme", order = 1)]
    public class ColorScheme : ScriptableObject
    {
        [Serializable]
        public class ColorItem
        {
            public string Name;
            public Color[] Palette;
        }

        public string Name; // Name of the overall color scheme
        public List<ColorItem> Data; // List of ColorItems representing different named color palettes
        public Texture Atlas; // Texture atlas associated with the color scheme. Optional. 
    }
}