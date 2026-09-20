using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using Color = UnityEngine.Color;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Start Node in the Visual Novel Director tool.
    /// </summary>
    /// <remarks>
    /// The start node serves as the entry point to the visual novel graph.
    /// It is decorated with the <see cref="NodeAttribute"/> to specify its category path in the graph item library, icon and title.
    /// </remarks>
    [Serializable]
    [Node(CATEGORY_STR, VisualNovelDirectorGraph.ToolPath + "Assets/Icons/StartNode.png", TITLE_STR)]
    internal class StartNode : VisualNovelNode
    {
        const string CATEGORY_STR = "Flow Control";
        const string TITLE_STR = "Start";

        /// <summary>
        /// Initializes the node with a custom subtitle, tooltip and default color.
        /// </summary>
        public override void OnEnable()
        {
            // The subtitle is displayed below the title of the node in the graph, and provides additional context about the node (eg: category).
            Subtitle = CATEGORY_STR;
            
            // The tooltip is displayed when hovering over the node in the graph, and provides a brief description of what the node does.
            Tooltip = "The entry point of the visual novel.";
            
            // The default color is displayed as a color strip at the top of the node, and can be used to visually differentiate nodes. Here we set it to a custom green color.
            // It is also possible to manually set the color of a node instance in the graph through its right-click menu, which will overwrite this default color for that instance.
            DefaultColor = EditorGUIUtility.isProSkin ? new Color(0.525f, 0.745f, 0.619f) : new Color(0.223f, 0.459f, 0.349f);

            // The Title property dynamically overrides the node's displayed title in the graph, but not in the graph item library, which always uses the NodeAttribute.Title.
            // Here, we set it to include the graph name for clarity when multiple graphs are open.
            Title = $"{TITLE_STR} ({Graph.Name})";
        }
        
        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            // Start is a special node that has no input, so we don't call DefineCommonPorts
            context.AddOutputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
