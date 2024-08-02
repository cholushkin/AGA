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
                Step = 0.1f;
                GeneratePolyad = 5;
            }

            public ColorSource Randomize()
            {
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
                return this;
            }

            public string Name;
            public string ShortenedName;
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
        public bool ShortNames;

        public List<ColorSource> InputSource;

        private void Reset()
        {
            Debug.Log("Setting default values");

            OutputDirectory = "Game/ColorScheme/Palettes";
            ColorSchemeScriptableObjectName = "Basic";
            ColorSchemeName = "Basic colors";

            InputSource = null;
        }

        [Button]
        public void AddRandomColorSource()
        {
            if (InputSource == null)
                InputSource = new List<ColorSource>();
            InputSource.Add(new ColorSource().Randomize());
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
                var shortName = source.ShortenedName;
                var baseColor = source.Color;
                var step = source.Step;
                var tinyColor = new TinyColor(baseColor);

                Debug.Log(JsonUtility.ToJson(tinyColor.GetColorInfo()));


                // BRIGHTEN
                if (source.GenerateBrighten > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-brt" : $"{rootName}-brighten";
                    colorSchemeItem.Palette = new Color[source.GenerateBrighten];

                    for (int i = 0; i < source.GenerateBrighten; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Brighten(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // LIGHTEN
                if (source.GenerateLighten > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ?  $"{shortName}-lgt" : $"{rootName}-lighten";
                    colorSchemeItem.Palette = new Color[source.GenerateLighten];

                    for (int i = 0; i < source.GenerateLighten; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Lighten(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // DARKEN
                if (source.GenerateDarken > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-drk" : $"{rootName}-darken";
                    colorSchemeItem.Palette = new Color[source.GenerateDarken];

                    for (int i = 0; i < source.GenerateDarken; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Darken(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // TINT
                if (source.GenerateTint > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-tnt" : $"{rootName}-tinted";
                    colorSchemeItem.Palette = new Color[source.GenerateTint];

                    for (int i = 0; i < source.GenerateTint; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Tint(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // SHADE
                if (source.GenerateShade > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-shd" : $"{rootName}-shaded";
                    colorSchemeItem.Palette = new Color[source.GenerateShade];

                    for (int i = 0; i < source.GenerateShade; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Shade(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // SATURATE
                if (source.GenerateSaturate > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-str" : $"{rootName}-saturated";
                    colorSchemeItem.Palette = new Color[source.GenerateSaturate];

                    for (int i = 0; i < source.GenerateSaturate; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Saturate(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // DESATURATE
                if (source.GenerateDesaturate > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-dst": $"{rootName}-desaturated";
                    colorSchemeItem.Palette = new Color[source.GenerateDesaturate];

                    for (int i = 0; i < source.GenerateDesaturate; ++i)
                        colorSchemeItem.Palette[i] = tinyColor.Desaturate(i * step).ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // MONOCHROMATIC
                if (source.GenerateMonochromatic > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-mnc": $"{rootName}-monochromatic";
                    colorSchemeItem.Palette = new Color[source.GenerateMonochromatic];

                    var mData = tinyColor.Monochromatic(source.GenerateMonochromatic);
                    for (int i = 0; i < source.GenerateMonochromatic; ++i)
                        colorSchemeItem.Palette[i] = mData[i].ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // ANALOGOUS
                if (source.GenerateAnalogous > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-ang": $"{rootName}-analogous";
                    colorSchemeItem.Palette = new Color[source.GenerateAnalogous];

                    var aData = tinyColor.Analogous(source.GenerateAnalogous);
                    for (int i = 0; i < source.GenerateAnalogous; ++i)
                        colorSchemeItem.Palette[i] = aData[i].ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }

                // POLYAD
                if (source.GeneratePolyad > 0)
                {
                    var colorSchemeItem = new ColorScheme.ColorItem();
                    colorSchemeItem.Name = ShortNames ? $"{shortName}-{source.GeneratePolyad}pd": $"{rootName}-{source.GeneratePolyad}polyad";
                    colorSchemeItem.Palette = new Color[source.GeneratePolyad];

                    var pData = tinyColor.Polyad(source.GeneratePolyad);
                    for (int i = 0; i < source.GeneratePolyad; ++i)
                        colorSchemeItem.Palette[i] = pData[i].ToColor();

                    colorScheme.Data.Add(colorSchemeItem);
                }
            }

            // Save scriptable object
            if (!reusedAsset)
                UnityEditor.AssetDatabase.CreateAsset(colorScheme, path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log($"ColorScheme saved at: {path}");
        }
    }
}