using System;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using TinyColorLib;
using UnityEngine;
using UnityEngine.Assertions;
using Color = UnityEngine.Color;

namespace GameLib.ColorScheme
{

    [CreateAssetMenu(fileName = "ColorSchemeCreator", menuName = "GameLib/Color/ColorSchemeCreator", order = 1)]
    public class ColorSchemeCreator : ScriptableObject
    {
	    [Serializable]
	    public class ColorSource
	    {
            // Set default values. Supposed to be redefined by user manually in the inspector
		    public ColorSource()
		    {
			    Name = null;

                // todo: move to tinycolor package as a random helper
                var randomColor = TinyColorLib.Color.Colors
	                .ElementAt(UnityEngine.Random.Range(0, TinyColorLib.Color.Colors.Count)).Value;
                Name = randomColor.ToName();
                Assert.IsNotNull(Name);

                float mutation = UnityEngine.Random.Range(0f, 0.4f);
                if (mutation > 0.05f)
                {
	                if (randomColor.IsDark())
	                {
		                randomColor = randomColor.Brighten(mutation);
		                Name = $"Brighten{Name}{mutation:F1}";
	                }
	                else
	                {
		                randomColor = randomColor.Darken(mutation);
		                Name = $"Darken{Name}{mutation:F1}";
                    }
                }

                Color = randomColor.ToColor();
                Step = 0.1f;
                GeneratePolyad = 5;
		    }

		    public string Name;
		    public Color Color;
		    public float Step;

		    public int GenerateBrighten; // 0 - don't generate, n - generate n grades of brighten colors with the step == Step
            public int GenerateLighten; // 0 - don't generate, n - generate n grades of lighten colors with the step == Step
            public int GenerateDarken; // 0 - don't generate, n - generate n grades of darken colors with the step == Step
            public int GenerateTint; // 0 - don't generate, n - generate n grades of tint colors with the step == Step
            public int GenerateShade; // 0 - don't generate, n - generate n grades of shaded colors with the step == Step
            public int GenerateSaturate; // 0 - don't generate, n - generate n grades of saturated colors with the step == Step
            public int GenerateDesaturate; // 0 - don't generate, n - generate n grades of desaturated colors with the step == Step
            public int GenerateMonochromatic; // 0 - don't generate, n - generate n grades of monochromatic colors with the step == Step
            public int GenerateAnalogous; // 0 - don't generate, n - generate n grades of analogous colors with the step == Step
            public int GeneratePolyad; // 0 - don't generate, n - generate n-level polyad (5 will generate pentad)
	    }


        [Tooltip("path to output scriptable object within Assets directory")]
        public string OutputDirectory;
        public string ColorSchemeScriptableObjectName;
        public string ColorSchemeName;
        
        public ColorSource[] InputSource;

        private void Reset()
        {
            Debug.Log("Setting default values");
            Debug.Log("Setting 3 random sources");

            OutputDirectory = "Game/ColorScheme/Palettes";
            ColorSchemeScriptableObjectName = "Basic";
            ColorSchemeName = "Basic colors";

            InputSource = new ColorSource[]
            {
                new ColorSource(),
                new ColorSource(),
                new ColorSource()
            };
        }


        [Button]
        public void CreateColorScheme()
        {
            // Create an instance of the ScriptableObject
            string path = $"Assets/{OutputDirectory}/{ColorSchemeScriptableObjectName}.asset";

            // create or reuse existing
            var reusedAsset = true;
            ColorScheme colorScheme = UnityEditor.AssetDatabase.LoadAssetAtPath<ColorScheme>(path);
            if (colorScheme == null)
            {
                Debug.Log($"Creating new asset ColorScheme");
                colorScheme = ScriptableObject.CreateInstance<ColorScheme>();
                reusedAsset = false;
            }
            else
                Debug.Log($"Reusing existing asset {path}");


            colorScheme.Name = ColorSchemeName;
            colorScheme.Data = new List<ColorScheme.ColorItem>();


            foreach (var source in InputSource)
            {
                var rootName = source.Name;
                var baseColor = source.Color;
                var tinyColor = new TinyColor(baseColor);

                Debug.Log(tinyColor.GetColorInfo());
                



                //if (GenerateBrighten)
                //{
                //    var colorSchemeItem = new ColorScheme.ColorItem();
                //    colorSchemeItem.Name = $"{rootName}Brighten";
                //    colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                //    colorSchemeItem.color[0] = baseColor;

                //    for (int i = 1; i < ColorCountInARow; ++i)
                //        colorSchemeItem.color[i] = tinyColor.Brighten(i * Step).ToColor();

                //    colorScheme.Data.Add(colorSchemeItem);
                //}

                //if (GenerateLighten)
                //{
                //    var colorSchemeItem = new ColorScheme.ColorItem();
                //    colorSchemeItem.Name = $"{rootName}Lighten";
                //    colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                //    colorSchemeItem.color[0] = baseColor;

                //    for (int i = 1; i < ColorCountInARow; ++i)
                //        colorSchemeItem.color[i] = tinyColor.Lighten(i * Step).ToColor();

                //    colorScheme.Data.Add(colorSchemeItem);
                //}

                //if (GenerateDarken)
                //{
                //    var colorSchemeItem = new ColorScheme.ColorItem();
                //    colorSchemeItem.Name = $"{rootName}Darken";
                //    colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                //    colorSchemeItem.color[0] = baseColor;

                //    for (int i = 1; i < ColorCountInARow; ++i)
                //        colorSchemeItem.color[i] = tinyColor.Darken(i * Step).ToColor();

                //    colorScheme.Data.Add(colorSchemeItem);
                //}
            }

            // Save scriptable object
            if(!reusedAsset)
                UnityEditor.AssetDatabase.CreateAsset(colorScheme, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log($"ColorScheme saved at: {path}");
        }
    }
}