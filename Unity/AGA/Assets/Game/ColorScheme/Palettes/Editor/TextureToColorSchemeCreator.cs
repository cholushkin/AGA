using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameLib.ColorScheme
{
	[CreateAssetMenu(fileName = "TextureToColorSchemeCreator", menuName = "GameLib/Color/TextureToColorSchemeCreator", order = 1)]
	public class TextureToColorSchemeCreator : ScriptableObject
	{
		public TextureSource[] Sources;
		public string OutputDirectory;
		public int ColorsNumber;

		[Serializable]
		public class TextureSource
		{
			[Tooltip("Keep empty to use texture name as a base")]
			public string ColorSchemeCreatorName;
			public Texture2D SourceTexture;
		}


		[Button()]
		public void CreateColorSchemeCreators()
		{
			foreach (var textureSource in Sources)
			{
				// Cook name
				var schemeCreatorName = textureSource.ColorSchemeCreatorName;
				if (string.IsNullOrEmpty(schemeCreatorName))
				{
					Assert.IsNotNull(textureSource.SourceTexture);
					schemeCreatorName = $"{textureSource.SourceTexture.name}SchemeCreator";
				}
                CreateSchemeCreator(OutputDirectory, schemeCreatorName, textureSource.SourceTexture);
            }
		}

		void CreateSchemeCreator(string outputDirectory, string schemeCreatorName, Texture2D texture)
		{
			// Create an instance of the ScriptableObject
			ColorSchemeCreator colorSchemeCreator = ScriptableObject.CreateInstance<ColorSchemeCreator>();

			colorSchemeCreator.ColorSchemeName = texture.name;


			var palette = new ColorThief.ColorThief();
			List<ColorThief.QuantizedColor> colors = palette.GetPalette(texture, ColorsNumber);


			colorSchemeCreator.InputSource = new List<ColorSchemeCreator.ColorSource>(ColorsNumber);

			Assert.IsTrue(colors.Count == ColorsNumber, $"colors.Count == ColorsNumber{colors.Count} {ColorsNumber}");

			for (int i = 0; i < colors.Count; i++)
            {
                var src = new ColorSchemeCreator.ColorSource();
                src.Name = $"Color {i}";
                src.Color = colors[i].UnityColor;

                colorSchemeCreator.InputSource.Add( src );
				
			}

			string path = $"Assets/{OutputDirectory}/{schemeCreatorName}.asset";
			UnityEditor.AssetDatabase.CreateAsset(colorSchemeCreator, path);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
	}
}
