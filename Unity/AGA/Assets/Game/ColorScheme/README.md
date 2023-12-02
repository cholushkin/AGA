# ColorSchemes Library

ColorSchemes library provides a set of classes for managing color schemes in your projects. The library includes the following classes:

1. **ColorSchemeCreator** (Editor-only ScriptableObject)
2. **ColorScheme** (Runtime ScriptableObject)
3. **ColorSchemeAtlasCreator** (Editor-only ScriptableObject)

## ColorSchemeCreator

ColorSchemeCreator is an editor-only ScriptableObject designed for creating ColorScheme objects. These objects contain color palettes with names that serve as references for easily accessing colors. If you only need Unity.Color values, using ColorSchemes may be sufficient. ColorSchemes act as color containers and can be utilized for various purposes, such as:

- Creating different GUI color schemes
- Applying diverse coloring in SVG images
- Establishing different color settings for a set of materials

## ColorScheme

ColorScheme is a runtime ScriptableObject that contains color palettes with names, providing an easy way to reference colors within your project. By using ColorSchemes, you can streamline the process of managing and updating color themes for various elements.

## ColorSchemeAtlasCreator

ColorSchemeAtlasCreator is an editor-only ScriptableObject created for generating color texture atlases based on a ColorScheme. If you are working with textured 3D objects that require a color atlas, this tool is particularly useful. To utilize this feature, create a ColorSchemeAtlasCreator and assign a ColorScheme as a reference. The ColorSchemeAtlasCreator will then generate a color texture atlas based on the provided color scheme. Additionally, a reference to the generated atlas will be added to the ColorScheme ScriptableObject.


### How to Use

1. **ColorSchemeCreator:**
   - Use the editor-only ColorSchemeCreator to create ColorScheme assets.
   - Define color palettes and assign names for easy referencing.

2. **ColorScheme:**
   - Use the ColorScheme ScriptableObject in your runtime code.
   - Reference color palettes by name.
   - You can also create ColorScheme directly, ignoring ColorSchemeCreator

3. **ColorSchemeAtlasCreator:**
   - Create a ColorSchemeAtlasCreator for generating texture atlases.
   - Assign a ColorScheme as a reference.
   - The generated atlas will be added to the ColorScheme for runtime use.
   - The generated atlas could be used for 3D object texturing.
