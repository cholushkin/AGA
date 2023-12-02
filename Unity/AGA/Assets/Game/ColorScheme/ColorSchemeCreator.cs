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
    public bool GenerateDarken;
    public ColorScheme.ColorItem[] InputColors;


    [Button]
    public void CreateColorScheme()
    {
        // Create an instance of the ScriptableObject
        ColorScheme colorScheme = ScriptableObject.CreateInstance<ColorScheme>();


        colorScheme.Name = ColorSchemeName;
        colorScheme.Data = new List<ColorScheme.ColorItem>();



        foreach (var colorItem in InputColors)
        {
            var rowName = colorItem.Name;
            var baseColor = colorItem.color[0];
            var tinyColor = new TinyColor(baseColor);

            var colorSchemeItem = new ColorScheme.ColorItem();

            colorSchemeItem.Name = rowName;
            colorSchemeItem.color = new UnityEngine.Color[ColorCountInARow];
            colorSchemeItem.color[0] = baseColor;

            if(GenerateBrighten)
                for (int i = 1; i < ColorCountInARow; ++i)
                    colorSchemeItem.color[i] = tinyColor.Brighten(i * 0.1f).ToColor();

            

            colorScheme.Data.Add(colorSchemeItem);
        }



        //// Set values for the ScriptableObject
        //myScriptableObject.myData = "Hello, ScriptableObject!";
        //myScriptableObject.myValue = 42;

        //// Optionally, you can save the ScriptableObject as an asset
        string path = $"Assets/{OutputDirectory}/{ColorSchemeScriptableObjectName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(colorScheme, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log("ScriptableObject created and saved at: " + path);
    }
}
