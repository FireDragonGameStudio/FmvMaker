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
            runtimeAsset.ID = graph.ID;
            BuildRuntimeGraph(startNodeModel, runtimeAsset);

            // Add the runtime object to the graph asset and set it to be the main asset.
            // This allows the same asset to be used in inspectors wherever a runtime asset is expected.
            // Refer to the BasicVisualNovelCanvas.prefab for an example of this.
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        /// <summary>
        /// Builds the runtime graph by traversing all reachable nodes from the start node.
        /// Supports both linear and branching paths.
        /// </summary>
        /// <param name="startNode">The start node of the graph</param>
        /// <param name="runtimeAsset">The runtime asset to populate</param>
        static void BuildRuntimeGraph(INode startNode, VisualNovelRuntimeGraph runtimeAsset)
        {
            // Map from editor nodes to their starting index in the runtime nodes list
            var nodeToRuntimeIndex = new Dictionary<INode, int>();
            
            // Queue for breadth-first traversal
            var nodesToProcess = new Queue<INode>();
            
            // Start with the first node after the start node
            var firstNode = GetNextNode(startNode);
            if (firstNode != null)
            {
                nodesToProcess.Enqueue(firstNode);
            }

            // Process all reachable nodes
            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();
                
                // Skip if we've already processed this node
                if (nodeToRuntimeIndex.ContainsKey(currentNode))
                    continue;

                // Record the starting index for this node's runtime nodes
                var startIndex = runtimeAsset.Nodes.Count;
                nodeToRuntimeIndex[currentNode] = startIndex;

                // Convert the editor node to runtime node(s)
                var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);
                runtimeAsset.Nodes.AddRange(runtimeNodes);

                // Enqueue connected nodes based on node type
                if (currentNode is TwoOptionNode dialogueOptionNode)
                {
                    // For branching nodes, enqueue both branches
                    var option1Node = GetNextNodeFromPort(dialogueOptionNode, TwoOptionNode.OUT_PORT_OPTION1_NAME);
                    var option2Node = GetNextNodeFromPort(dialogueOptionNode, TwoOptionNode.OUT_PORT_OPTION2_NAME);
                    
                    if (option1Node != null)
                        nodesToProcess.Enqueue(option1Node);
                    if (option2Node != null)
                        nodesToProcess.Enqueue(option2Node);
                }
                else
                {
                    // For linear nodes, enqueue the next node
                    var nextNode = GetNextNode(currentNode);
                    if (nextNode != null)
                        nodesToProcess.Enqueue(nextNode);
                }
            }

            // Second pass: set up NextNodeIndex references
            SetupNodeReferences(runtimeAsset, nodeToRuntimeIndex, startNode);
        }

        /// <summary>
        /// Sets up the NextNodeIndex references for all runtime nodes.
        /// </summary>
        static void SetupNodeReferences(VisualNovelRuntimeGraph runtimeAsset, Dictionary<INode, int> nodeToRuntimeIndex, INode startNode)
        {
            var processedNodes = new HashSet<INode>();
            var nodesToProcess = new Queue<INode>();
            
            var firstNode = GetNextNode(startNode);
            if (firstNode != null)
            {
                nodesToProcess.Enqueue(firstNode);
            }

            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();
                
                if (!processedNodes.Add(currentNode))
                    continue;

                if (!nodeToRuntimeIndex.TryGetValue(currentNode, out var currentRuntimeIndex))
                    continue;

                // Handle different node types
                if (currentNode is TwoOptionNode optionNode)
                {
                    // For branching nodes, set both branch indices
                    var option1Node = GetNextNodeFromPort(optionNode, TwoOptionNode.OUT_PORT_OPTION1_NAME);
                    var option2Node = GetNextNodeFromPort(optionNode, TwoOptionNode.OUT_PORT_OPTION2_NAME);

                    var twoOptionRuntimeNode = runtimeAsset.Nodes[currentRuntimeIndex] as TwoOptionRuntimeNode;
                    if (twoOptionRuntimeNode != null)
                    {
                        twoOptionRuntimeNode.Option1NextNodeIndex = option1Node != null && nodeToRuntimeIndex.TryGetValue(option1Node, out var idx1) ? idx1 : -1;
                        twoOptionRuntimeNode.Option2NextNodeIndex = option2Node != null && nodeToRuntimeIndex.TryGetValue(option2Node, out var idx2) ? idx2 : -1;
                    }

                    // Enqueue both branches for processing
                    if (option1Node != null)
                        nodesToProcess.Enqueue(option1Node);
                    if (option2Node != null)
                        nodesToProcess.Enqueue(option2Node);
                }
                else
                {
                    // For linear nodes, set NextNodeIndex
                    var nextNode = GetNextNode(currentNode);
                    
                    // Get all runtime nodes created from this editor node
                    var runtimeNodes = TranslateNodeModelToRuntimeNodes(currentNode);
                    for (int i = 0; i < runtimeNodes.Count; i++)
                    {
                        var runtimeNodeIndex = currentRuntimeIndex + i;
                        var runtimeNode = runtimeAsset.Nodes[runtimeNodeIndex];
                        
                        // If this is the last runtime node from this editor node
                        if (i == runtimeNodes.Count - 1)
                        {
                            // Point to the next editor node's first runtime node
                            runtimeNode.NextNodeIndex = nextNode != null && nodeToRuntimeIndex.TryGetValue(nextNode, out var nextIdx) ? nextIdx : -1;
                        }
                        else
                        {
                            // Point to the next runtime node in the sequence
                            runtimeNode.NextNodeIndex = runtimeNodeIndex + 1;
                        }
                    }

                    if (nextNode != null)
                        nodesToProcess.Enqueue(nextNode);
                }
            }
        }

        /// <summary>
        /// Gets the node connected to a specific output port.
        /// </summary>
        /// <param name="currentNode">The node to get the output from</param>
        /// <param name="portName">The name of the output port</param>
        /// <returns>The connected node, or null if not connected</returns>
        static INode GetNextNodeFromPort(INode currentNode, string portName)
        {
            var outputPort = currentNode.GetOutputPortByName(portName);
            var nextNodePort = outputPort?.FirstConnectedPort;
            return nextNodePort?.GetNode();
        }

        /// <summary>
        /// Gets the node that is executed after the given node.
        /// </summary>
        /// <param name="currentNode">The current node</param>
        /// <returns>The next node in the graph</returns>
        static INode GetNextNode(INode currentNode)
        {
            var outputPort = currentNode.GetOutputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME);
            var nextNodePort = outputPort.FirstConnectedPort;
            var nextNode = nextNodePort?.GetNode();

            return nextNode;
        }

        /// <summary>
        /// Gets the wire that is executed after the given node.
        /// </summary>
        /// <param name="currentNode">The current node</param>
        /// <returns>The next wire in the graph</returns>
        static Wire GetNextWire(INode currentNode)
        {
            var nextNode = GetNextNode(currentNode);
            if (nextNode != null)
            {
                var outputPort = currentNode.GetOutputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME);
                var inputPort = nextNode.GetInputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME);
                
                if (inputPort != null && outputPort != null)
                    return currentNode.Graph.GetWire(outputPort, inputPort);
            }

            return null;
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
            Wire nextWire;
            
            switch (nodeModel)
            {
                case SetBackgroundNode setBackgroundNodeModel:
                    nextWire = GetNextWire(setBackgroundNodeModel);
                    var bgRuntimeNode = new SetBackgroundRuntimeNode
                    {
                        BackgroundSprite = GetInputPortValue<Sprite>(setBackgroundNodeModel.GetInputPortByName(SetBackgroundNode.IN_PORT_BACKGROUND_NAME)),
                        NextWireDestinationId = nextWire != null ? nextWire.InputPort.ID : default,
                        NextWireSourceId = nextWire != null ? nextWire.OutputPort.ID : default
                    };
                    
                    AddPortIdsToRuntimeNode(bgRuntimeNode, setBackgroundNodeModel);
                    returnedNodes.Add(bgRuntimeNode);

                    // Note: We deliberately don't add a WaitForInputRuntimeNode here to enable updating multiple
                    // visual novel elements (the background, music, dialogue, etc) all at once. This creates a seamless
                    // transition involving more than one element.
                    break;

                case SetDialogueNode setDialogueNodeModel:
                    var dialogueRuntimeNode = new SetDialogueRuntimeNode
                    {
                        ActorName = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_NAME_NAME)),
                        ActorSprite = GetInputPortValue<Sprite>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_ACTOR_SPRITE_NAME)),
                        LocationIndex = (int)GetInputPortValue<SetDialogueNode.Location>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_LOCATION_NAME)),
                        DialogueText = GetInputPortValue<string>(setDialogueNodeModel.GetInputPortByName(SetDialogueNode.IN_PORT_DIALOGUE_NAME)),
                        ID = setDialogueNodeModel.ID
                    };
                    
                    AddPortIdsToRuntimeNode(dialogueRuntimeNode, setDialogueNodeModel);
                    returnedNodes.Add(dialogueRuntimeNode);
                    
                    nextWire = GetNextWire(setDialogueNodeModel);
                    
                    // Insert a WaitForInputNode after dialogue to create the expected visual novel behaviour.
                    // This ensures narrative flow pauses until the player signals readiness to continue.
                    returnedNodes.Add(new WaitForInputRuntimeNode
                    {
                        NextWireDestinationId = nextWire != null ? nextWire.InputPort.ID : default,
                        NextWireSourceId = nextWire != null ? nextWire.OutputPort.ID : default
                    });
                    break;

                case WaitForInputNode _:
                    returnedNodes.Add(new WaitForInputRuntimeNode());
                    break;

                case TwoOptionNode twoOptionNodeModel:
                    var option1Node = GetNextNodeFromPort(twoOptionNodeModel, TwoOptionNode.OUT_PORT_OPTION1_NAME);
                    var option2Node = GetNextNodeFromPort(twoOptionNodeModel, TwoOptionNode.OUT_PORT_OPTION2_NAME);
                    var nextWire1 = nodeModel.Graph.GetWire(
                        twoOptionNodeModel.GetOutputPortByName(TwoOptionNode.OUT_PORT_OPTION1_NAME), 
                        option1Node.GetInputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME));
                    var nextWire2 = nodeModel.Graph.GetWire(
                        twoOptionNodeModel.GetOutputPortByName(TwoOptionNode.OUT_PORT_OPTION2_NAME), 
                        option2Node.GetInputPortByName(VisualNovelNode.EXECUTION_PORT_DEFAULT_NAME));
                    
                    var twoOptionRuntimeNode = new TwoOptionRuntimeNode
                    {
                        Option1Text = GetInputPortValue<string>(twoOptionNodeModel.GetInputPortByName(TwoOptionNode.IN_PORT_OPTION1_NAME)),
                        Option2Text = GetInputPortValue<string>(twoOptionNodeModel.GetInputPortByName(TwoOptionNode.IN_PORT_OPTION2_NAME)),
                        NextWireOption1SourceId = nextWire1.OutputPort.ID,
                        NextWireOption1DestinationId = nextWire1.InputPort.ID,
                        NextWireOption2SourceId = nextWire2.OutputPort.ID,
                        NextWireOption2DestinationId = nextWire2.InputPort.ID
                    };
                    
                    AddPortIdsToRuntimeNode(twoOptionRuntimeNode, twoOptionNodeModel);
                    returnedNodes.Add(twoOptionRuntimeNode);
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
            if (port.IsConnected)
            {
                switch (port.FirstConnectedPort.GetNode())
                {
                    case IVariableNode variableNode:
                        variableNode.Variable.TryGetDefaultValue<T>(out value);
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
        
        static void AddPortIdsToRuntimeNode(VisualNovelRuntimeNode runtimeNode, INode nodeModel)
        {
            foreach (var inputPort in nodeModel.GetInputPorts())
            {
                runtimeNode.InputPortNames.Add(inputPort.Name);
                runtimeNode.InputPortIDs.Add(inputPort.ID);
            }
            foreach (var outputPort in nodeModel.GetOutputPorts())
            {
                runtimeNode.OutputPortNames.Add(outputPort.Name);
                runtimeNode.OutputPortIDs.Add(outputPort.ID);
            }
        }
    }
}
