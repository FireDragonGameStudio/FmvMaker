using FmvMaker.Core;
using FmvMaker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private Dictionary<string, FmvMakerNode> nodeLookup = new();
        private FmvMakerNode currentNode;
        private FmvMakerDecisionData currentDecisionData;

        private FmvVideoView videoView = null;

        private void Awake() {
            videoView = GetComponent<FmvVideoView>();
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

            //OnVideoStarted.AddListener(DisablePreviousItems);
            //OnVideoStarted.AddListener(DisablePreviousNavigationTargets);
            //OnVideoStarted.AddListener(ShowItemsAndNavigationsForLooping);

            OnVideoFinished.AddListener(CheckForNextVideo);
            //OnVideoFinished.AddListener(CheckForInstantNextVideo);
            //OnVideoFinished.AddListener(ShowCurrentItems);
            //OnVideoFinished.AddListener(ShowCurrentNavigationTargets);

            //PlayVideo(startVideo);
        }

        private void CheckForNextVideo(VideoClip videoclip) {
            if (currentNode.DecisionData?.Count <= 0) {
                EndFmvMaker();
            }

            if (currentNode.DecisionData?.Count == 1 && currentNode.DecisionData[0] != null) {
                PlayVideo(currentNode.DecisionData[0].DestinationId);
            } else if (currentDecisionData != null) {
                PlayVideo(currentDecisionData.DestinationId);
            }
        }

        //private void Update() {
        //    SkipVideo();
        //    PauseVideo();
        //    QuitGame();
        //    ToggleAllAvailableClickables();
        //}

        private void Update() {
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

            if (currentNode?.DecisionData?.Count > 1) {
                if (Keyboard.current.digit1Key.wasPressedThisFrame) {
                    PlayVideoDecision(currentNode.DecisionData[0]);
                } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
                    PlayVideoDecision(currentNode.DecisionData[1]);
                } else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
                    PlayVideoDecision(currentNode.DecisionData[2]);
                } else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
                    PlayVideoDecision(currentNode.DecisionData[3]);
                }
            }
        }

        private void OnDestroy() {
            OnVideoStarted.RemoveAllListeners();
            OnVideoFinished.RemoveAllListeners();

            DisposeVideoEventTrigger();
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

            currentNode = nodeLookup[nodeId];

            // play video and set ui elements for navigation
            //Debug.Log(currentNode.NodeName);
            if (currentNode.VideoClip != null) {
                PlayVideoInternal(currentNode);
            } else {
                PlayVideoDecision(currentNode.DecisionData[0]);
            }
        }

        private void PlayVideoInternal(FmvMakerNode fmvMakerNode) {
            var videoModel = new VideoModel() {
                NodeId = fmvMakerNode.NodeId,
                NodeName = fmvMakerNode.NodeName,
                VideoClip = fmvMakerNode.VideoClip,
                IsLooping = fmvMakerNode.IsLooping,
            };
            videoView.PrepareAndPlay(videoModel);
        }

        private void PlayVideoDecision(FmvMakerDecisionData decisionDataNode) {
            currentDecisionData = decisionDataNode;

            var videoModel = new VideoModel() {
                NodeId = decisionDataNode.DestinationId,
                NodeName = decisionDataNode.DecisionText,
                VideoClip = decisionDataNode.VideoClip,
                IsLooping = false,
            };
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

        private void EndFmvMaker() {
            Debug.Log("FmvMaker ended!");
            currentNode = null;
            currentDecisionData = null;
            StopVideo();
        }
    }
}