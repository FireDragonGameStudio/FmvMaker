using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    /// <summary>
    /// VisualNovelDirectorImporter is a <see cref="ScriptedImporter"/> that imports the <see cref="VisualNovelDirectorGraph"/>
    /// and builds the corresponding <see cref="VisualNovelRuntimeGraph"/>.
    /// </summary>
    [ScriptedImporter(1, VisualNovelDirectorGraph.AssetExtension)]
    internal class VisualNovelDirectorImporter : ScriptedImporter
    {
        /// <summary>
        /// Unity calls this method when the editor imports the asset. This method then processes the imported <see cref="VisualNovelDirectorGraph"/>.
        /// </summary>
        /// <param name="ctx">The asset import context.</param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<VisualNovelDirectorGraph>(ctx.assetPath);

            // The `graph` may be null if the `GraphDatabase.LoadGraphForImporter` method
            // fails to load the asset from the specified `ctx.assetPath`.
            // This can occur under the following circumstances:
            // - The asset path is incorrect, or the asset does not exist at the specified location.
            // - The asset located at the specified path is not of type `VisualNovelDirectorGraph`.
            // - The asset file itself is problematic. For example, it is corrupted, or stored in an unsupported format.
            //
            // Best practice to deal with serialization is to thoroughly validate and safeguard against
            // impaired or incomplete data, to account for potential deserialization issues.
            if (graph == null)
            {
                Debug.LogError($"Failed to load Visual Novel Director graph asset: {ctx.assetPath}");
                return;
            }

            // Get the first Start Node
            // (Only using the first node is a simplification we made for this sample)
            var startNodeModel = graph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNodeModel == null)
            {
                // No need to log an error here, as the VisualNovelDirectorGraphProcessor is already logging an error in the console
                // See VisualNovelDirectorGraph.CheckGraphErrors(GraphLogger).
                return;
            }

            // Build the runtime asset by walking the graph and adding the relevant nodes.
            var runtimeAsset = ScriptableObject.CreateInstance<VisualNovelRuntimeGraph>();
            var nextNodeModel = GetNextNode(startNodeModel);
            while (nextNodeModel != null)
            {
                var runtimeNodes = TranslateNodeModelToRuntimeNodes(nextNodeModel);
                runtimeAsset.Nodes.AddRange(runtimeNodes);

                nextNodeModel = GetNextNode(nextNodeModel);
            }

            // Add the runtime object to the graph asset and set it to be the main asset.
            // This allows the same asset to be used in inspectors wherever a runtime asset is expected.
            // Refer to the BasicVisualNovelCanvas.prefab for an example of this.
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        /// <summary>
        /// Gets the node that is executed after the given node.
        /// </summary>
        /// <param name="currentNode">The current node</param>
        /// <returns>The next node in the graph</returns>
        static INode GetNextNode(INode currentNode)
        {
            var outputPort = currentNode.GetOutputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME);
            var nextNodePort = outputPort.firstConnectedPort;
            var nextNode = nextNodePort?.GetNode();

            return nextNode;
        }

        /// <summary>
        /// Converts a <see cref="VisualNovelNode"/> to a list of one or more runtime <see cref="VisualNovelRuntimeNode"/>s.
        /// </summary>
        /// <param name="nodeModel">The <see cref="VisualNovelNode"/> to convert.</param>
        /// <returns>
        /// A list of <see cref="VisualNovelRuntimeNode"/>s that represent the runtime behavior of the input node.
        /// Multiple runtime nodes may be generated from a single input <see cref="VisualNovelNode"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the <see cref="NodeModel"/> passed in is unsupported and cannot be converted.
        /// </exception>
        /// <remarks>
        /// This conversion is not always 1:1. For example: the <see cref="SetDialogueNode"/> node is converted to
        /// a <see cref="SetDialogueRuntimeNode"/> and a <see cref="WaitForInputRuntimeNode"/>. This is so that the
        /// runtime pauses execution and waits for player input after a dialogue is displayed. This approach allows
        /// more complex behaviour to be composed of multiple simpler runtime nodes.
        /// <br/><br/>
        /// </remarks>
        static List<VisualNovelRuntimeNode> TranslateNodeModelToRuntimeNodes(INode nodeModel)
        {
            var returnedNodes = new List<VisualNovelRuntimeNode>();
            switch (nodeModel)
            {
                case SetBackgroundNode setBackgroundNodeModel:
                    returnedNodes.Add(new SetBackgroundRuntimeNode
                    {
                        BackgroundSprite = GetInputPortValue<Sprite>(setBackgroundNodeModel.GetInputPortByName(SetBackgroundNode.IN_PORT_BACKGROUND_NAME))
                    });

                    // Note: We deliberately don't add a WaitForInputRuntimeNode here to enable updating multiple
                    // visual novel elements (the background, music, dialogue, etc) all at once. This creates a seamless
                    // transition involving more than one element.
                    break;

                case SetDialogueNode setDialogueNodeModel:
                    returnedNodes.Add(new SetDialogueRuntimeNode
                    {
                        ActorName = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_NAME_NAME)),
                        ActorSprite = GetInputPortValue<Sprite>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_SPRITE_NAME)),
                        LocationIndex = (int)GetInputPortValue<SetDialogueNode.Location>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_LOCATION_NAME)),
                        DialogueText = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_DIALOGUE_NAME))
                    });

                    // Insert a WaitForInputNode after dialogue to create the expected visual novel behaviour.
                    // This ensures narrative flow pauses until the player signals readiness to continue.
                    returnedNodes.Add(new WaitForInputRuntimeNode());
                    break;

                case WaitForInputNode _:
                    returnedNodes.Add(new WaitForInputRuntimeNode());
                    break;

                default:
                    throw new ArgumentException($"Unsupported node model type: {nodeModel.GetType()}");
            }

            return returnedNodes;
        }

        /// <summary>
        /// Gets the value of an input port on a node.
        /// <br/><br/>
        /// The value is obtained from (in priority order):<br/>
        /// 1. Connections to the port (variable nodes, constant nodes, wire portals)<br/>
        /// 2. Embedded value on the port<br/>
        /// 3. Default value of the port<br/>
        /// </summary>
        static T GetInputPortValue<T>(IPort port)
        {
            T value = default;

            // If port is connected to another node, get value from connection
            if (port.isConnected)
            {
                switch (port.firstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.variable.TryGetDefaultValue<T>(out value);
                        return value;
                    case IConstantNode constantNode:
                        constantNode.TryGetValue<T>(out value);
                        return value;
                    default:
                        break;
                }
            }
            else
            {
                // If port has embedded value, return it.
                // Otherwise, return the default value of the port
                port.TryGetValue(out value);
            }

            return value;
        }
    }
}
