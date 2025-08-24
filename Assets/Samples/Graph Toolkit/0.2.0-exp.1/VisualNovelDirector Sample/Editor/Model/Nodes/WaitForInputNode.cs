using System;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// Represents the Wait For Input Node in the Visual Novel Director tool.
    /// <br/><br/>
    /// Use this when you want to have an explicit pause / wait for player input after you change an element
    /// (background, etc.) of the visual novel.
    /// </summary>
    /// <remarks>
    /// Is converted to a <see cref="WaitForInputRuntimeNode"/> for the runtime.
    /// <br/><br/>
    /// The <see cref="SetDialogueNode"/> automatically waits for input after it executes because it creates
    /// a <see cref="WaitForInputRuntimeNode"/> when converted to the runtime graph. This is to match the typical
    /// expected behaviour of visual novel dialogue waiting for player input between dialogue lines.
    /// </remarks>
    [Serializable]
    internal class WaitForInputNode : VisualNovelNode
    {
        /// <summary>
        /// Defines the output for the node.
        /// </summary>
        /// <param name="context">The scope to define the node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }
    }
}
