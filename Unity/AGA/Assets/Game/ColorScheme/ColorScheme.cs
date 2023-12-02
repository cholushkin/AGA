using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLib
{
    [CreateAssetMenu(fileName = "ColorScheme", menuName = "GameLib/Color/ColorScheme", order = 1)]
    public class ColorScheme : ScriptableObject
    {
        [Serializable]
        public class ColorItem
        {
            public string Name;
            public Color[] color;
        }

        public string Name;
        public List<ColorItem> Data;

        public Texture Atlas;
        



        
    }
}