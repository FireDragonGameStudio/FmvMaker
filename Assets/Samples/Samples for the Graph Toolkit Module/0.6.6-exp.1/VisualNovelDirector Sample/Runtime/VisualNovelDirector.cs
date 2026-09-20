using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using Unity.GraphToolkit.Editor.GraphVisualization;
#endif

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The main class that controls the visual novel direction.
    /// <br/><br/>
    /// This class is a <see cref="MonoBehaviour"/> intended to be attached to a specific hierarchy of GameObjects.
    /// In this sample we can see it being used in the `BasicVisualNovelCanvas` prefab. This is how it gets all the
    /// necessary components to run the visual novel, including the scene references, global settings, input handling
    /// and the runtime graph to execute.
    /// <br/><br/>
    /// When running (in PlayMode or in a build) this class executes the runtime visual novel graph, one node at a time,
    /// using their respective <see cref="IVisualNovelNodeExecutor{TNode}"/>. The execution supports both linear
    /// narrative flow and branching paths.
    /// </summary>
    public class VisualNovelDirector : MonoBehaviour
    {
        [Header("Graph")]
        // The runtime graph to execute. Note that the runtime graph asset is the same as the authoring graph asset
        // because we export the runtime asset object into the same asset and set it as the 'main' asset in our importer.
        // This allows us to edit the authoring graph in the editor and drag-drop the same asset into inspector fields
        // that expect the runtime graph.
        public VisualNovelRuntimeGraph RuntimeGraph;

        [Header("Scene References")]
        public Image BackgroundImage;
        public List<Image> ActorLocationList;
        public GameObject DialoguePanel;
        public TextMeshProUGUI DialogueText;
        public TextMeshProUGUI ActorNameText;
        public GameObject OptionPanel;
        public TextMeshProUGUI Option1Text;
        public TextMeshProUGUI Option2Text;

        [Header("Settings")]
        public float GlobalFadeDuration = 0.5f;
        public float GlobalTextDelayPerCharacter = 0.03f;

        [Header("Input")]
        public MonoBehaviour InputComponent;
        public IVisualNovelInputProvider InputProvider => InputComponent as IVisualNovelInputProvider;
        
        private async void Start()
        {
#if UNITY_EDITOR
            using var debugContext = Registry.CreateVisualizationContext(RuntimeGraph.ID);
            
            // Set opacity of all wires
            foreach (var node in RuntimeGraph.Nodes)
            {
                if (node is TwoOptionRuntimeNode twoOptionNode)
                {
                    var firstWire = debugContext.GetWireReference(twoOptionNode.NextWireOption1SourceId, twoOptionNode.NextWireOption1DestinationId);
                    firstWire.Opacity = 0.5f;
                    firstWire.IsDashed = true;
                    
                    var secondWire = debugContext.GetWireReference(twoOptionNode.NextWireOption2SourceId, twoOptionNode.NextWireOption2DestinationId);
                    secondWire.Opacity = 0.5f;
                    secondWire.IsDashed = true;

                }
                else
                {
                    if (node.NextWireSourceId.isValid && node.NextWireDestinationId.isValid)
                    {
                        var wire = debugContext.GetWireReference(node.NextWireSourceId, node.NextWireDestinationId);
                        wire.Opacity = 0.5f;
                        wire.IsDashed = true;
                    }
                }
            }
#endif
            
            // Create each executor once
            var setBackgroundExecutor = new SetBackgroundExecutor();
            var setDialogueExecutor = new SetDialogueExecutor();
            var waitForInputExecutor = new WaitForInputExecutor();
            var twoOptionExecutor = new TwoOptionExecutor();

            // Execute nodes following the graph structure, supporting branching paths
            int currentNodeIndex = 0;
            VisualNovelRuntimeNode previousNode = null;
            var nodesBeforeWaitForInput = new List<VisualNovelRuntimeNode>();
#if UNITY_EDITOR
            WireReference nextWire = default;
            WireReference prevWire = default;
#endif

            while (currentNodeIndex >= 0 && currentNodeIndex < RuntimeGraph.Nodes.Count)
            {
                var node = RuntimeGraph.Nodes[currentNodeIndex];
                int nextNodeIndex = -1;
                
                switch (node)
                {
                    case SetBackgroundRuntimeNode bgNode:
#if UNITY_EDITOR
                        // Debug View: Show the background sprite name as a port preview
                        debugContext?.GetPortReference(bgNode.GetInputPortID(SetBackgroundRuntimeNode.IN_PORT_BACKGROUND_NAME)).SetPreview(BackgroundImage.sprite != null ? BackgroundImage.sprite.name : "(none)");
                        if (bgNode.NextWireSourceId.isValid && bgNode.NextWireDestinationId.isValid)
                            nextWire = debugContext.GetWireReference(bgNode.NextWireSourceId, bgNode.NextWireDestinationId);
#endif
                        await setBackgroundExecutor.ExecuteAsync(bgNode, this);
                        nextNodeIndex = bgNode.NextNodeIndex;
                        break;
                        
                    case SetDialogueRuntimeNode dialogueNode:
#if UNITY_EDITOR
                        if (debugContext != null)
                        {
                            // Debug View: Play an animation on the dialogue node while it's being executed
                            debugContext.Motion.Play(debugContext.GetNodeReference(dialogueNode.ID), 1f);
                            
                            // Debug View: Show the dialogue details as port previews
                            debugContext.GetPortReference(dialogueNode.GetInputPortID(SetDialogueRuntimeNode.IN_PORT_ACTOR_NAME_NAME)).SetPreview(dialogueNode.ActorName);
                            debugContext.GetPortReference(dialogueNode.GetInputPortID(SetDialogueRuntimeNode.IN_PORT_ACTOR_SPRITE_NAME)).SetPreview(dialogueNode.ActorSprite != null ? dialogueNode.ActorSprite.name : "(none)");
                            debugContext.GetPortReference(dialogueNode.GetInputPortID(SetDialogueRuntimeNode.IN_PORT_LOCATION_NAME)).SetPreview(dialogueNode.LocationIndex == 0 ? "Left" : "Right");
                            const int maxPreviewLength = 40; // truncated to 40 characters if it's too long
                            var dialogueText = dialogueNode.DialogueText.Length > maxPreviewLength ? dialogueNode.DialogueText[..maxPreviewLength] + "..." : dialogueNode.DialogueText;
                            debugContext.GetPortReference(dialogueNode.GetInputPortID(SetDialogueRuntimeNode.IN_PORT_DIALOGUE_NAME)).SetPreview(dialogueText);
                        }
#endif
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        nextNodeIndex = dialogueNode.NextNodeIndex;
                        break;
                        
                    case SetDialogueRuntimeNodeWithPreviousActor dialogueNode:
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        nextNodeIndex = dialogueNode.NextNodeIndex;
                        break;
                        
                    case WaitForInputRuntimeNode waitNode:
                        await waitForInputExecutor.ExecuteAsync(waitNode, this);
#if UNITY_EDITOR
                        if (debugContext != null)
                        {
                            // Debug View: Stop the motion on the previous dialogue node
                            if (previousNode is SetDialogueRuntimeNode prevDialogueNode)
                                debugContext.Motion.Stop(debugContext.GetNodeReference(prevDialogueNode.ID));

                            // Debug View: Clear all port previews on the nodes leading up to the wait for input node
                            foreach (var nodeBeforeWaitForInput in nodesBeforeWaitForInput)
                            {
                                foreach (var inputPortID in nodeBeforeWaitForInput.InputPortIDs)
                                    debugContext.GetPortReference(inputPortID).ClearPreview();
                                foreach (var outputPortID in nodeBeforeWaitForInput.OutputPortIDs)
                                    debugContext.GetPortReference(outputPortID).ClearPreview();
                            }
                            
                            if (waitNode.NextWireSourceId.isValid && waitNode.NextWireDestinationId.isValid)
                                nextWire = debugContext.GetWireReference(waitNode.NextWireSourceId, waitNode.NextWireDestinationId);
                        }
#endif
                        nodesBeforeWaitForInput.Clear();
                        nextNodeIndex = waitNode.NextNodeIndex;
                        break;
                        
                    case TwoOptionRuntimeNode twoOptionNode:
#if UNITY_EDITOR
                        // Debug View: Show the options as port previews while waiting for player input
                        var option1PortRef = debugContext?.GetPortReference(twoOptionNode.GetOutputPortID(TwoOptionRuntimeNode.OUT_PORT_OPTION1_NAME));
                        var option2PortRef = debugContext?.GetPortReference(twoOptionNode.GetOutputPortID(TwoOptionRuntimeNode.OUT_PORT_OPTION2_NAME));
                        if (debugContext != null)
                        {
                            option1PortRef.Value.SetPreview(twoOptionNode.Option1Text);
                            option2PortRef.Value.SetPreview(twoOptionNode.Option2Text);
                        }
#endif
                        await twoOptionExecutor.ExecuteAsync(twoOptionNode, this);
                        
                        // Follow the branch based on the player's choice
                        nextNodeIndex = twoOptionNode.SelectedOption == 0 
                            ? twoOptionNode.Option1NextNodeIndex 
                            : twoOptionNode.Option2NextNodeIndex;
#if UNITY_EDITOR
                        if (debugContext != null)
                        {
                            // Debug View: Cross out the option in the port preview to indicate it was not selected
                            if (nextNodeIndex == twoOptionNode.Option1NextNodeIndex)
                            {
                                var currentString = option2PortRef.Value.TryGetPreview(out var previewValue) ? previewValue : twoOptionNode.Option2Text;
                                option2PortRef.Value.SetPreview($"<s>{currentString}</s>");
                            }
                            else
                            {
                                var currentString = option1PortRef.Value.TryGetPreview(out var previewValue) ? previewValue : twoOptionNode.Option1Text;
                                option1PortRef.Value.SetPreview($"<s>{currentString}</s>");
                            }
                        }
                        
                        nextWire = twoOptionNode.SelectedOption == 0 
                            ? debugContext.GetWireReference(twoOptionNode.NextWireOption1SourceId, twoOptionNode.NextWireOption1DestinationId)
                            : debugContext.GetWireReference(twoOptionNode.NextWireOption2SourceId, twoOptionNode.NextWireOption2DestinationId);
#endif
                        break;
                        
                    default:
                        Debug.LogError($"No executor found for node type: {node.GetType()}");
                        return;
                }

                nodesBeforeWaitForInput.Add(node);
                previousNode = node;
                currentNodeIndex = nextNodeIndex;
                
#if UNITY_EDITOR
                // Only transition the wire animation when the active wire actually changed.
                // Some node cases (e.g. SetDialogueRuntimeNode) don't update nextWire, so without
                // this guard the bottom-of-loop code would re-target the same wire and the
                // prevWire branch would clear the animation we just (re)started.
                if (nextWire != prevWire)
                {
                    if (nextWire.Context != null)
                    {
                        nextWire.ClearCustomization();
                        nextWire.WidthOverride = 10f;
                        debugContext.Motion.Play(nextWire, 1f);
                    }

                    if (prevWire != default && prevWire.Context != null)
                    {
                        prevWire.ClearCustomization();
                        prevWire.WidthOverride = 10f;
                    }

                    prevWire = nextWire;
                }
#endif
            }
        }
    }
}
