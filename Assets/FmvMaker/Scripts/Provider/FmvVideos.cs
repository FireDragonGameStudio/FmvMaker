using FmvMaker.Core;
using FmvMaker.Core.Facades;
using FmvMaker.Core.Interfaces;
using FmvMaker.Models;
using FmvMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.GraphToolkit.Editor;
using Unity.GraphToolkit.Editor.GraphVisualization;
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

        private Dictionary<Hash128, FmvMakerNode> nodeLookup = new();
        private FmvMakerNode currentNode;
        private List<GameObject> clickableObjects = new();
        private ICountdownEvents timedInteractionCountdown;

        private Hash128 navigationLastNodeId;
        private float navigationUiAlphaValue;
        private float navigationUiLabelAlphaValue;

#if UNITY_EDITOR
        private Context context;
#endif

        private void Awake() {
            if (!videoView) {
                videoView = GetComponent<FmvVideoView>();
            }

            SetupVideoEventTrigger();

            timedInteractionCountdown ??= timedInteractionCountdownObject.GetComponent<TimedInteractionCountdown>();
            timedInteractionCountdown.ResetCountdown();

#if UNITY_EDITOR
            context = Registry.CreateVisualizationContext(RuntimeGraph.GraphId);
#endif
        }

        private async void Start() {

            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            OnVideoStarted.AddListener(CheckForVideoStart);
            OnVideoFinished.AddListener(CheckForNextVideo);

            DynamicVideoResolution.Instance.ScreenSizeChanged += OnScreenSizeChanged;

            foreach (var node in RuntimeGraph.AllFmvMakerNodes) {
                nodeLookup[node.NodeId] = node;
            }

            if (RuntimeGraph.EntryNodeId != null) {
                PlayVideo(RuntimeGraph.EntryNodeId);
            } else {
                EndFmvMaker();
            }

            muteAudioImage.gameObject.SetActive(videoView.ActivePlayer.IsMute);
        }

        private void Update() {
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

                PlayVideo(currentNode.NextNodeId);
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

                // check if item is needed here and if user has the item in his inventory
                var nextVideo = nodeLookup[currentNode.DecisionData[i].DestinationId];
                var neededItem = nextVideo.NeededItem;
                if (neededItem != null && !inventory.ContainsItem(neededItem)) {
                    continue;
                }
                // the item can be found and the user already has it
                var givingItem = nextVideo.GivingItem;
                if (givingItem != null && inventory.ContainsItem(givingItem)) {
                    continue;
                }
                // if item is needed, is in inventory, is not multi-use and was used alread
                if (neededItem != null && inventory.ContainsItem(neededItem) && !neededItem.MultiUse && neededItem.WasUsed) {
                    continue;
                }

                var navigationButton = GameObject.Instantiate(navigationButtonPrefab);
                navigationButton.name = nextVideo.NodeId.ToString();
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

                    PlayVideo(decisionData.DestinationId);
                });

                // set visibility of button image
                var image = navigationButton.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, navigationUiAlphaValue);

                clickableObjects.Add(navigationButton);
            }
        }

        private void OnDestroy() {
            OnVideoStarted.RemoveAllListeners();
            OnVideoFinished.RemoveAllListeners();

            DisposeVideoEventTrigger();

            DynamicVideoResolution.Instance.ScreenSizeChanged -= OnScreenSizeChanged;

#if UNITY_EDITOR
            context.Dispose();
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

        private void PlayVideo(Hash128 nodeId) {

            if (!nodeLookup.ContainsKey(nodeId)) {
                EndFmvMaker();
                return;
            }

            PlayVideoInternal(nodeLookup[nodeId]);
        }

        private void PlayVideoInternal(FmvMakerNode fmvMakerNode) {

#if UNITY_EDITOR
            if (currentNode != null) {
                var previousNodeRef = context.GetNodeReference(currentNode.NodeId);
                context.Motion.Stop(previousNodeRef);

                //var connectedWireRef = context.GetWireReference(currentNode.OutputPortId, fmvMakerNode.InputPortId);
                //context.Motion.Play(connectedWireRef);
                //connectedWireRef.WidthOverride = 10f;
                //connectedWireRef.Opacity = 0.5f;
                //connectedWireRef.IsDashed = true;
                //connectedWireRef.ClearCustomization();
            }

            var currentNodeRef = context.GetNodeReference(fmvMakerNode.NodeId);
            currentNodeRef.FillAmount = 100f;
            context.Motion.Play(currentNodeRef);

            var connectedWireRef = context.GetWireReference(Hash128.Parse("ac6574633d958692adb54c5549f22d55"), Hash128.Parse("d6a97c78c9f8b8eaf0f87cbdf2bfe8e7"));
            connectedWireRef.WidthOverride = 10f;
            connectedWireRef.Opacity = 0.5f;
            connectedWireRef.IsDashed = true;
            context.Motion.Play(connectedWireRef);
#endif

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

        private void OnScreenSizeChanged(float width, float height) {
            //rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(clickableModel.RelativeScreenPosition);
        }

        private void EndFmvMaker() {
            Debug.Log("FmvMaker ended!");
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