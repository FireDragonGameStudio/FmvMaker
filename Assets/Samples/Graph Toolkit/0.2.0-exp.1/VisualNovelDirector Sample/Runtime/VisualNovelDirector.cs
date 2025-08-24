using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    /// using their respective <see cref="IVisualNovelNodeExecutor{TNode}"/>. For the sake of this sample,
    /// the visual novel is linear and does not include any branching logic.
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

        [Header("Settings")]
        public float GlobalFadeDuration = 0.5f;
        public float GlobalTextDelayPerCharacter = 0.03f;

        [Header("Input")]
        public MonoBehaviour InputComponent;
        public IVisualNovelInputProvider InputProvider => InputComponent as IVisualNovelInputProvider;

        private async void Start()
        {
            // Create each executor once
            var setBackgroundExecutor = new SetBackgroundExecutor();
            var setDialogueExecutor = new SetDialogueExecutor();
            var waitForInputExecutor = new WaitForInputExecutor();

            // Execute each node in the runtime graph sequentially
            foreach (var node in RuntimeGraph.Nodes)
            {
                switch (node)
                {
                    case SetBackgroundRuntimeNode bgNode:
                        await setBackgroundExecutor.ExecuteAsync(bgNode, this);
                        break;
                    case SetDialogueRuntimeNode dialogueNode:
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        break;
                    case SetDialogueRuntimeNodeWithPreviousActor dialogueNode:
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        break;
                    case WaitForInputRuntimeNode waitNode:
                        await waitForInputExecutor.ExecuteAsync(waitNode, this);
                        break;
                    default:
                        Debug.LogError($"No executor found for node type: {node.GetType()}");
                        break;
                }
            }
        }
    }
}
