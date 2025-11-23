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

        //[Header("Key Bindings")]
        //[SerializeField]
        //private KeyCode SkipVideoKey = KeyCode.Escape;
        //[SerializeField]
        //private KeyCode PauseVideoKey = KeyCode.P;
        //[SerializeField]
        //private KeyCode QuitGameKey = KeyCode.Q;
        //[SerializeField]
        //private KeyCode ShowAllAvailableClickablesKey = KeyCode.Space;

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
            //OnVideoStarted.AddListener(DisablePreviousNavigationTargets);
            //OnVideoStarted.AddListener(ShowItemsAndNavigationsForLooping);

            OnVideoFinished.AddListener(CheckForNextVideo);
            //OnVideoFinished.AddListener(CheckForInstantNextVideo);
            //OnVideoFinished.AddListener(ShowCurrentItems);
            //OnVideoFinished.AddListener(ShowCurrentNavigationTargets);

            //PlayVideo(startVideo);

            DynamicVideoResolution.Instance.ScreenSizeChanged += OnScreenSizeChanged;
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
                // or the item can be found and the user already has it
                var nextVideo = nodeLookup[currentNode.DecisionData[i].DestinationId];
                if ((nextVideo.NeededItem != null && !inventory.ContainsItem(nextVideo.NeededItem)) ||
                    (nextVideo.GivingItem != null && inventory.ContainsItem(nextVideo.GivingItem))) {
                    continue;
                }

                var navigationButton = GameObject.Instantiate(navigationButtonPrefab);
                navigationButton.transform.SetParent(navigationElementsParent, false);

                // set position and size
                var rectTransform = navigationButton.GetComponent<RectTransform>();
                rectTransform.transform.localScale = Vector3.one;
                rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(currentNode.DecisionData[i].RelativePosition);
                rectTransform.sizeDelta = DynamicVideoResolution.GetRelativeScreenSize(currentNode.DecisionData[i].RelativeSize);

                // set click action
                var button = navigationButton.GetComponent<Button>();
                var decisionData = currentNode.DecisionData[i];
                button.onClick.AddListener(() => PlayVideo(decisionData.DestinationId));

                clickableObjects.Add(navigationButton);
            }
        }

        //private void Update() {
        //    SkipVideo();
        //    PauseVideo();
        //    QuitGame();
        //    ToggleAllAvailableClickables();
        //}

        //private void Update() {
        //if (Mouse.current.leftButton.wasPressedThisFrame && currentNode != null) {
        //    if (currentNode.VideoClip != null) {
        //        PlayVideo(currentNode);
        //    } else {
        //        PlayVideo(currentNode.DecisionData[0]);
        //    }
        //if (!string.IsNullOrEmpty(currentNode.DecisionData[0].DestinationId)) {
        //    ShowNode(currentNode.DecisionData[0].DestinationId);
        //} else {
        //    EndFmvMaker();
        //}
        //}

        //if (currentNode?.DecisionData?.Count > 1) {
        //    if (Keyboard.current.digit1Key.wasPressedThisFrame) {
        //        PlayVideoDecision(currentNode.DecisionData[0]);
        //    } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
        //        PlayVideoDecision(currentNode.DecisionData[1]);
        //    } else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
        //        PlayVideoDecision(currentNode.DecisionData[2]);
        //    } else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
        //        PlayVideoDecision(currentNode.DecisionData[3]);
        //    }
        //}
        //}

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

        //private void SkipVideo() {
        //    if (Input.GetKeyUp(SkipVideoKey) && !currentVideoElement.IsLooping && currentVideoElement.AlreadyWatched && videoView.ActivePlayer.IsPlaying) {
        //        videoView.SkipVideoClip(currentVideoElement);
        //    }
        //}

        //private void PauseVideo() {
        //    if (Input.GetKeyUp(PauseVideoKey) && !currentVideoElement.IsLooping) {
        //        videoView.PauseVideoClip(currentVideoElement);
        //    }
        //}

        //private void QuitGame() {
        //    if (Input.GetKeyUp(QuitGameKey)) {
        //        Application.Quit();
        //    }
        //}

        //private void ShowNode(string nodeId) {
        //    if (!nodeLookup.ContainsKey(nodeId)) {
        //        EndFmvMaker();
        //        return;
        //    }

        //    currentNode = nodeLookup[nodeId];

        //    // play video and set ui elements for navigation
        //    Debug.Log(currentNode.NodeName);
        //    if (currentNode.VideoClip != null) {
        //        PlayVideo(currentNode);
        //    } else {
        //        PlayVideo(currentNode.DecisionData[0]);
        //    }
        //}

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