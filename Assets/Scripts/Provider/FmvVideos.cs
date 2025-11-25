using FmvMaker.Core;
using FmvMaker.Core.Facades;
using FmvMaker.Models;
using FmvMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private Dictionary<string, FmvMakerNode> nodeLookup = new();
        private FmvMakerNode currentNode;
        private List<GameObject> clickableObjects = new();

        private string navigationNotSpawnedNodeId = "";

        private void Awake() {
            if (!videoView) {
                videoView = GetComponent<FmvVideoView>();
            }
            SetupVideoEventTrigger();
        }

        private async void Start() {
            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            foreach (var node in RuntimeGraph.AllFmvMakerNodes) {
                nodeLookup[node.NodeId] = node;
            }

            if (!string.IsNullOrEmpty(RuntimeGraph.EntryNodeId)) {
                PlayVideo(RuntimeGraph.EntryNodeId);
            } else {
                EndFmvMaker();
            }

            OnVideoStarted.AddListener(CheckForVideoStart);
            OnVideoFinished.AddListener(CheckForNextVideo);

            DynamicVideoResolution.Instance.ScreenSizeChanged += OnScreenSizeChanged;
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

            // autoplay next when video node
            if (!currentNode.HasDecisionData) {
                PlayVideo(currentNode.NextNodeId);
                return;
            }

            // spawn nav elements for not looping videos
            if (!currentNode.IsLooping && !navigationNotSpawnedNodeId.Equals(currentNode.NodeId)) {
                navigationNotSpawnedNodeId = currentNode.NodeId;
                SpawnNavigationElements();
            }
        }

        private void SpawnNavigationElements() {
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
                navigationButton.name = nextVideo.NodeId;
                navigationButton.transform.SetParent(navigationElementsParent, false);

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

                clickableObjects.Add(navigationButton);
            }
        }

        private void OnDestroy() {
            OnVideoStarted.RemoveAllListeners();
            OnVideoFinished.RemoveAllListeners();

            DisposeVideoEventTrigger();

            DynamicVideoResolution.Instance.ScreenSizeChanged -= OnScreenSizeChanged;
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

        private void PlayVideo(string nodeId) {

            if (!nodeLookup.ContainsKey(nodeId)) {
                EndFmvMaker();
                return;
            }

            PlayVideoInternal(nodeLookup[nodeId]);
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
            if (skipVideo.action.WasPerformedThisFrame() && !videoView.ActivePlayer.IsLooping && videoView.ActivePlayer.IsPlaying) {
                videoView.SkipVideoClip();
            }
        }

        private void PauseVideo() {
            if (playPauseVideo.action.WasPerformedThisFrame()) {
                if (videoView.ActivePlayer.IsPlaying) {
                    videoView.PauseVideoClip();
                } else {
                    videoView.ResumeVideoClip();
                }
            }
        }

        private void MuteVideo() {
            if (muteUnmuteVideo.action.WasPerformedThisFrame()) {
                if (videoView.ActivePlayer.AudioClip.volume >= 1f) {
                    videoView.ActivePlayer.AudioClip.volume = 0f;
                } else {
                    videoView.ActivePlayer.AudioClip.volume = 1f;
                }
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
    }
}