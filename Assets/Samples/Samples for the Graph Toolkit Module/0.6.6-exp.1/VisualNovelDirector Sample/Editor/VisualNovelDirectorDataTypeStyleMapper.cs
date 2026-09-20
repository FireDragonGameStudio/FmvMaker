using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Maps data types to specific icons and colors for the Visual Novel Director graph.
    /// </summary>
    /// <remarks>
    /// This class is decorated with the <see cref="DataTypeStyleMapperAttribute"/> to specify that it applies specifically to the <see cref="VisualNovelDirectorGraph"/>.
    /// </remarks>
    [DataTypeStyleMapper(typeof(VisualNovelDirectorGraph))]
    public class VisualNovelDirectorDataTypeStyleMapper : DataTypeStyleMapper
    {
        const string SPRITE_ICON_PATH = VisualNovelDirectorGraph.ToolPath + "Assets/Icons/Sprite.png";
        const string LOCATION_ICON_PATH = VisualNovelDirectorGraph.ToolPath + "Assets/Icons/Location.png";

        /// <summary>
        /// Initializes the data type style mapper by registering icons and colors for specific data types used in the Visual Novel Director graph.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="DataTypeStyleMapper.Register"/> method to associate data types with their corresponding icons and colors.
        /// In this example, we register styles for the <see cref="Sprite"/> type (which is not included in the built-in type styles) and a custom <see cref="SetDialogueNode.Location"/> type.
        /// These types are particularly relevant in the context of the Visual Novel Director tool and benefit from having their own custom styles.
        /// It is not possible to register a style for a data type that already has a built-in type style (see: <see cref="DataTypeStyleMapper"/> for more information).
        /// </remarks>
        public VisualNovelDirectorDataTypeStyleMapper()
        {
            // Register a style for the Sprite type.
            var spriteIcon = EditorGUIUtility.IconContent(SPRITE_ICON_PATH).image as Texture2D;
            if (spriteIcon == null)
                Debug.LogWarning($"Sprite icon not found at path: {SPRITE_ICON_PATH}");
            
            var spriteColor = EditorGUIUtility.isProSkin ? Color.cornflowerBlue : Color.midnightBlue;
            Register(typeof(Sprite), spriteIcon, spriteColor);

            // Register a style for the custom Location type.
            var locationIcon = EditorGUIUtility.IconContent(LOCATION_ICON_PATH).image as Texture2D;
            if (locationIcon == null)
                Debug.LogWarning($"Location icon not found at path: {SPRITE_ICON_PATH}");
            
            var locationColor = EditorGUIUtility.isProSkin ? Color.yellowGreen : Color.darkOliveGreen;
            Register(typeof(SetDialogueNode.Location), locationIcon, locationColor);
        }
    }
}