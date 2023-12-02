using GameLib;
using NaughtyAttributes;
using System.Collections.Generic;
using TinyColorLib;
using UnityEngine;


[CreateAssetMenu(fileName = "ColorSchemeCreator", menuName = "GameLib/Color/ColorSchemeCreator", order = 1)]
public class ColorSchemeCreator : ScriptableObject
{
    [Tooltip("path to output scriptable object within Assets directory")]
    public string OutputDirectory;
    public string ColorSchemeScriptableObjectName;
    public string ColorSchemeName;
    public int ColorCountInARow;
    public bool GenerateBrighten;
    public bool GenerateLighten;
    public bool GenerateDarken;
    public float Step;
    public ColorScheme.ColorItem[] InputColors;

    private void Reset()
    {
        Debug.Log("Setting default values");
        OutputDirectory = "Game/ColorScheme/Palletes";
        ColorSchemeScriptableObjectName = "Basic";
        ColorSchemeName = "Basic colors";
        ColorCountInARow = 5;
        GenerateBrighten = true;
        GenerateDarken = true;
        GenerateLighten = true;
        Step = 0.1f;
        InputColors = new ColorScheme.ColorItem[3];
        InputColors[0] = new ColorScheme.ColorItem { color = new UnityEngine.Color[1] { UnityEngine.Color.red }, Name = "Red" };
        InputColors[1] = new ColorScheme.ColorItem { color = new UnityEngine.Color[1] { UnityEngine.Color.green }, Name = "Green" };
        InputColors[2] = new ColorScheme.ColorItem { color = new UnityEngine.Color[1] { UnityEngine.Color.blue }, Name = "Blue" };
    }


    [Button]
    public void CreateColorScheme()
    {
        // Create an instance of the ScriptableObject
        ColorScheme colorScheme = ScriptableObject.CreateInstance<ColorScheme>();


        colorScheme.Name = ColorSchemeName;
        colorScheme.Data = new List<ColorScheme.ColorItem>();



        foreach (var colorItem in InputColors)
        {
            var rootName = colorItem.Name;
            var baseColor = colorItem.color[0];
            var tinyColor = new TinyColor(baseColor);

            

            if (GenerateBrighten)
            {
                var colorSchemeItem = new ColorScheme.ColorItem();
                colorSchemeItem.Name = $"{rootName}Brighten";
                colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                colorSchemeItem.color[0] = baseColor;

                for (int i = 1; i < ColorCountInARow; ++i)
                    colorSchemeItem.color[i] = tinyColor.Brighten(i * Step).ToColor();

                colorScheme.Data.Add(colorSchemeItem);
            }

            if(GenerateLighten)
            {
                var colorSchemeItem = new ColorScheme.ColorItem();
                colorSchemeItem.Name = $"{rootName}Lighten";
                colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                colorSchemeItem.color[0] = baseColor;

                for (int i = 1; i < ColorCountInARow; ++i)
                    colorSchemeItem.color[i] = tinyColor.Lighten(i * Step).ToColor();

                colorScheme.Data.Add(colorSchemeItem);
            }

            if (GenerateDarken)
            {
                var colorSchemeItem = new ColorScheme.ColorItem();
                colorSchemeItem.Name = $"{rootName}Darken";
                colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
                colorSchemeItem.color[0] = baseColor;

                for (int i = 1; i < ColorCountInARow; ++i)
                    colorSchemeItem.color[i] = tinyColor.Darken(i * Step).ToColor();
                
                colorScheme.Data.Add(colorSchemeItem);
            }
        }

        // Save scriptable object
        string path = $"Assets/{OutputDirectory}/{ColorSchemeScriptableObjectName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(colorScheme, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log("ColorScheme created and saved at: " + path);
    }
}
