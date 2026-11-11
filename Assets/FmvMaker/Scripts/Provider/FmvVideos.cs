using FmvMaker.Core;
using FmvMaker.Core.Facades;
using FmvMaker.Core.Interfaces;
using FmvMaker.Models;
using FmvMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FmvMaker.Provider {
    [RequireComponent(typeof(FmvVideoView))]
    public class FmvVideos : MonoBehaviour {

        [Header("FmvMaker Graph")]
        public FmvMakerRuntimeGraph RuntimeGraph;

        [Header("FmvMaker Events")]
        public VideoEvent OnVideoStarted = new VideoEvent();
        public VideoBoolEvent OnVideoPaused = new VideoBoolEvent();
        public VideoEvent OnVideoSkipped = new VideoEvent();
        public VideoEvent OnVideoFinished = new VideoEvent();

        [Header("Key Bindings")]
        [SerializeField] private InputActionReference playPauseVideo;
        [SerializeField] private InputActionReference skipVideo;
        [SerializeField] private InputActionReference muteUnmuteVideo;

        [Header("Internal references")]
        [SerializeField] private FmvVideoView videoView = null;
        [SerializeField] private FmvInventory inventory = null;
        [SerializeField] private RectTransform navigationElementsParent = null;
        [SerializeField] private GameObject navigationButtonPrefab;
        [SerializeField] private Image muteAudioImage;
        [SerializeField] private GameObject timedInteractionCountdownObject;

        private const int MaxRandomizersInARow = 32;

        private Dictionary<Hash128, FmvMakerNode> nodeLookup = new();
        private FmvMakerNode currentNode;
        private List<GameObject> clickableObjects = new();
        private ICountdownEvents timedInteractionCountdown;

        private Hash128 navigationLastNodeId;
        private float navigationUiAlphaValue;
        private float navigationUiLabelAlphaValue;

#if UNITY_EDITOR
        private FmvGraphVisualizer graphVisualizer;
#endif

        private void Awake() {
            if (!videoView) {
                videoView = GetComponent<FmvVideoView>();
            }

            SetupVideoEventTrigger();

            timedInteractionCountdown ??= timedInteractionCountdownObject.GetComponent<TimedInteractionCountdown>();
            timedInteractionCountdown.ResetCountdown();

#if UNITY_EDITOR
            graphVisualizer = new FmvGraphVisualizer(RuntimeGraph);
#endif
        }

        private async void Start() {

            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            OnVideoStarted.AddListener(CheckForVideoStart);
            OnVideoFinished.AddListener(CheckForNextVideo);

            foreach (var node in RuntimeGraph.AllFmvMakerNodes) {
                nodeLookup[node.NodeId] = node;
            }

            if (RuntimeGraph.EntryNodeId.isValid) {
                PlayVideo(RuntimeGraph.EntryNodeId, RuntimeGraph.EntryOutputPortId);
            } else {
                EndFmvMaker();
            }

            muteAudioImage.gameObject.SetActive(videoView.ActivePlayer.IsMute);
        }

        private void Update() {
#if UNITY_EDITOR
            graphVisualizer?.Tick();
#endif
            SkipVideo();
            PauseVideo();
            MuteVideo();
        }

        private void CheckForVideoStart(VideoClip videoclip) {
            if (currentNode.IsLooping) {
                SpawnNavigationElements();
            }

            // show countdown for timed interaction
            if (currentNode.IsTimedInteraction) {
                SpawnNavigationElements();
                timedInteractionCountdown.InitCountdown(Convert.ToSingle(currentNode.VideoClip.length));
            } else {
                timedInteractionCountdown.ResetCountdown();
            }

            // add finding item to inventory
            if (currentNode.GivingItem != null) {
                inventory.AddEntry(currentNode.GivingItem);
            }
        }

        private void CheckForNextVideo(VideoClip videoclip) {
            // end if something went wrong with decision data
            if (currentNode.HasDecisionData && currentNode.DecisionData.Count <= 0) {
                EndFmvMaker();
                return;
            }

            // autoplay next when video node or play default option when timed interaction is over
            if (!currentNode.HasDecisionData || currentNode.IsTimedInteraction) {

                // hide countdown for timed interaction
                timedInteractionCountdown.ResetCountdown();

                PlayVideo(currentNode.NextNodeId, currentNode.OutputPortId);
                return;
            }

            // spawn nav elements for not looping videos
            if (!currentNode.IsLooping && !navigationLastNodeId.Equals(currentNode.NodeId)) {
                SpawnNavigationElements();
            }
        }

        private void SpawnNavigationElements() {
            navigationLastNodeId = currentNode.NodeId;

            for (int i = 0; i < currentNode.DecisionData?.Count; i++) {

                var destinationId = currentNode.DecisionData[i].DestinationId;
                if (!IsDestinationAvailable(destinationId)) {
                    continue;
                }
                var neededItem = nodeLookup.TryGetValue(destinationId, out var nextVideo) ? nextVideo.NeededItem : null;

                var navigationButton = GameObject.Instantiate(navigationButtonPrefab);
                navigationButton.name = destinationId.ToString();
                navigationButton.transform.SetParent(navigationElementsParent, false);

                // set label text
                if (!string.IsNullOrEmpty(currentNode.DecisionData[i].LabelText)) {
                    var labelText = navigationButton.GetComponentInChildren<TextMeshProUGUI>();
                    labelText.text = currentNode.DecisionData[i].LabelText;
                    labelText.color = currentNode.DecisionData[i].LabelTextColor;
                    labelText.alpha = navigationUiLabelAlphaValue;
                }

                // set position and size
                var rectTransform = navigationButton.GetComponent<RectTransform>();
                rectTransform.transform.localScale = Vector3.one;
                rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(currentNode.DecisionData[i].RelativePosition);
                rectTransform.sizeDelta = DynamicVideoResolution.GetRelativeScreenSize(currentNode.DecisionData[i].RelativeSize);

                // set click action
                var button = navigationButton.GetComponent<Button>();
                var decisionData = currentNode.DecisionData[i];
                button.onClick.AddListener(() => {

                    // use item, if applicable
                    if (neededItem != null && inventory.ContainsItem(neededItem) && (neededItem.MultiUse || !neededItem.WasUsed)) {
                        neededItem.WasUsed = true;
                    }

                    PlayVideo(decisionData.DestinationId, decisionData.OutputPortId);
                });

                // set visibility of button image
                var image = navigationButton.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, navigationUiAlphaValue);

                clickableObjects.Add(navigationButton);
            }
        }

        /// <summary>
        /// Whether the player may go to this node, based on the inventory.
        /// Destinations that aren't playable nodes (e.g. an End FMV Node) are always available.
        /// </summary>
        private bool IsDestinationAvailable(Hash128 destinationId) {
            if (!nodeLookup.TryGetValue(destinationId, out var destination)) {
                return true;
            }

            // item is needed here and the user doesn't have it
            var neededItem = destination.NeededItem;
            if (neededItem != null && !inventory.ContainsItem(neededItem)) {
                return false;
            }
            // the item can be found here and the user already has it
            var givingItem = destination.GivingItem;
            if (givingItem != null && inventory.ContainsItem(givingItem)) {
                return false;
            }
            // item is needed, is in inventory, is not multi-use and was used already
            if (neededItem != null && !neededItem.MultiUse && neededItem.WasUsed) {
                return false;
            }
            return true;
        }

        /// <summary>Picks one outcome of a randomizer node by weight, ignoring outcomes the player can't reach.</summary>
        private FmvMakerDecisionData PickRandomOutcome(FmvMakerNode randomizer) {
            var candidates = new List<FmvMakerDecisionData>();
            var totalWeight = 0f;
            foreach (var outcome in randomizer.DecisionData) {
                if (outcome.Weight > 0f && IsDestinationAvailable(outcome.DestinationId)) {
                    candidates.Add(outcome);
                    totalWeight += outcome.Weight;
                }
            }

            if (candidates.Count == 0) {
                return null;
            }

            var roll = UnityEngine.Random.Range(0f, totalWeight);
            foreach (var outcome in candidates) {
                roll -= outcome.Weight;
                if (roll < 0f) {
                    return outcome;
                }
            }
            // floating point edge case: roll == totalWeight
            return candidates[candidates.Count - 1];
        }

        private void OnDestroy() {
            OnVideoStarted.RemoveAllListeners();
            OnVideoFinished.RemoveAllListeners();

            DisposeVideoEventTrigger();

#if UNITY_EDITOR
            graphVisualizer?.Dispose();
#endif
        }

        private void SetupVideoEventTrigger() {
            videoView.OnVideoStarted += OnVideoStarted.Invoke;
            videoView.OnVideoPaused += OnVideoPaused.Invoke;
            videoView.OnVideoSkipped += OnVideoSkipped.Invoke;
            videoView.OnVideoFinished += OnVideoFinished.Invoke;
        }

        private void DisposeVideoEventTrigger() {
            videoView.OnVideoStarted -= OnVideoStarted.Invoke;
            videoView.OnVideoPaused -= OnVideoPaused.Invoke;
            videoView.OnVideoSkipped -= OnVideoSkipped.Invoke;
            videoView.OnVideoFinished -= OnVideoFinished.Invoke;
        }

        /// <param name="viaOutputPortId">Own "out" port of the node or choice block that led here (for visualization).</param>
        /// <param name="randomizerDepth">Number of randomizer nodes passed in a row, to stop endless randomizer loops.</param>
        private void PlayVideo(Hash128 nodeId, Hash128 viaOutputPortId, int randomizerDepth = 0) {

            if (!nodeLookup.TryGetValue(nodeId, out var nextNode)) {
                EndFmvMaker();
                return;
            }

#if UNITY_EDITOR
            graphVisualizer?.ShowTransition(nextNode.NodeId, viaOutputPortId, nextNode.InputPortId);
#endif

            if (nextNode.IsRandomizer) {
                if (randomizerDepth >= MaxRandomizersInARow) {
                    Debug.LogError($"FmvMaker: more than {MaxRandomizersInARow} randomizer nodes in a row at '{nextNode.NodeName}'. Check the graph for a loop between randomizers.");
                    EndFmvMaker();
                    return;
                }

                var outcome = PickRandomOutcome(nextNode);
                if (outcome == null) {
                    Debug.LogWarning($"FmvMaker: randomizer '{nextNode.NodeName}' has no available outcome.");
                    EndFmvMaker();
                    return;
                }

                PlayVideo(outcome.DestinationId, outcome.OutputPortId, randomizerDepth + 1);
                return;
            }

            PlayVideoInternal(nextNode);
        }

        private void PlayVideoInternal(FmvMakerNode fmvMakerNode) {

            currentNode = fmvMakerNode;

            var videoModel = new VideoModel() {
                NodeId = fmvMakerNode.NodeId,
                NodeName = fmvMakerNode.NodeName,
                VideoClip = fmvMakerNode.VideoClip,
                IsLooping = fmvMakerNode.IsLooping,
            };

            // dispose all screen elements
            foreach (var clickable in clickableObjects) {
                Destroy(clickable);
            }

            videoView.PrepareAndPlay(videoModel);
        }

        private void StopVideo() {
            videoView.StopVideoClip();
        }

        private void SkipVideo() {
            if (skipVideo.action.triggered && !videoView.ActivePlayer.IsLooping && videoView.ActivePlayer.IsPlaying) {
                videoView.SkipVideoClip();
            }
        }

        private void PauseVideo() {
            if (playPauseVideo.action.triggered && !currentNode.IsTimedInteraction) {
                if (videoView.ActivePlayer.IsPlaying) {
                    videoView.PauseVideoClip();
                } else {
                    videoView.ResumeVideoClip();
                }
            }
        }

        private void MuteVideo() {
            if (muteUnmuteVideo.action.triggered) {
                var mute = !muteAudioImage.gameObject.activeSelf;
                videoView.MuteVideoClip(mute);
                muteAudioImage.gameObject.SetActive(mute);
            }
        }

        private void EndFmvMaker() {
            Debug.Log("FmvMaker ended!");
#if UNITY_EDITOR
            graphVisualizer?.StopActive();
#endif
            currentNode = null;
            StopVideo();
        }

        public void SaveProgress() {
            RuntimeGraph.EntryNodeId = currentNode.NodeId;
        }

        public void SetNavigationUiVisibility(int alphaValue) {
            navigationUiAlphaValue = alphaValue;
        }

        public void SetNavigationUiLabelVisibility(int alphaValue) {
            navigationUiLabelAlphaValue = alphaValue;
        }
    }
}