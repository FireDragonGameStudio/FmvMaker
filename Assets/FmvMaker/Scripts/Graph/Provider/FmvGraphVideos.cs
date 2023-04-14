using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Provider;
using FmvMaker.Core.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Graph {
    public class FmvGraphVideos : MonoBehaviour {

        [Header("FmvMaker Events")]
        public VideoEvent OnVideoStarted = new VideoEvent();
        public VideoBoolEvent OnVideoPaused = new VideoBoolEvent();
        public VideoEvent OnVideoSkipped = new VideoEvent();
        public VideoEvent OnVideoFinished = new VideoEvent();

        [Header("Key Bindings")]
        [SerializeField]
        private KeyCode SkipVideoKey = KeyCode.Escape;
        [SerializeField]
        private KeyCode PauseVideoKey = KeyCode.P;
        [SerializeField]
        private KeyCode SaveGameKey = KeyCode.S;
        [SerializeField]
        private KeyCode QuitGameKey = KeyCode.Q;
        [SerializeField]
        private KeyCode ShowAllAvailableClickablesKey = KeyCode.Space;

        [Header("Settings")]
        [SerializeField]
        private bool useLoadingScreen = true;

        [Header("Internal references")]
        [SerializeField]
        private FmvVideoView videoView = null;
        [SerializeField]
        private FmvData data = null;
        [SerializeField]
        private GameObject loadingSceen = null;

        private bool isCurrentlyPaused = false;

        private VideoModel currentVideoElement;

        private void Awake() {
            CheckForOnlineMappingData();
            SetupVideoEventTrigger();
        }

        private async void Start() {
            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            ConfigureLoadingScreen();
        }

        private void Update() {
            SkipVideo();
            PauseResumeVideo();
            SaveGame();
            QuitGame();
            ToggleAllAvailableClickables();
        }

        private void OnDestroy() {
            OnVideoStarted.RemoveAllListeners();
            OnVideoFinished.RemoveAllListeners();
            DisposeVideoEventTrigger();
        }

        private void CheckForOnlineMappingData() {
            if (LoadFmvConfig.Config.VideoSourceType.Equals("ONLINE")) {
                data.GenerateOnlineVideoMappingData();
            }
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

        private void StopLoadingScreen(VideoModel video) {
            loadingSceen.SetActive(false);
        }

        private void ConfigureLoadingScreen() {
            loadingSceen.SetActive(false);

            if (LoadFmvConfig.Config.VideoSourceType != "INTERNAL" && useLoadingScreen) {
                loadingSceen.SetActive(true);
            }
        }

        private void StartLoadingScreen() {
            if (useLoadingScreen) {
                loadingSceen.SetActive(true);
            }
        }

        public void PlayVideo(VideoModel video) {
            currentVideoElement = video;
            OnVideoStarted.AddListener(StopLoadingScreen);
            StartLoadingScreen();
            videoView.PrepareAndPlay(video);
        }

        private void SkipVideo() {
            if (Input.GetKeyUp(SkipVideoKey)) {
                if (!currentVideoElement.IsLooping && currentVideoElement.AlreadyWatched && videoView.ActivePlayer.IsPlaying) {
                    videoView.SkipVideoClip(currentVideoElement);
                }
            }
        }

        private void PauseResumeVideo() {
            if (Input.GetKeyUp(PauseVideoKey) && !currentVideoElement.IsLooping) {
                if (videoView.ActivePlayer.IsPlaying && !isCurrentlyPaused) {
                    PauseVideo();
                } else if (!videoView.ActivePlayer.IsPlaying && isCurrentlyPaused) {
                    ResumeVideo();
                }
            }
        }

        private void PauseVideo() {
            isCurrentlyPaused = true;
            videoView.PauseVideoClip(currentVideoElement);
        }

        private void ResumeVideo() {
            isCurrentlyPaused = false;
            videoView.ResumeVideoClip(currentVideoElement);
        }

        private void SaveGame() {
            if (Input.GetKeyUp(SaveGameKey)) {
                data.ExportGraphGameData();
            }
        }

        private void QuitGame() {
            if (Input.GetKeyUp(QuitGameKey)) {
                Application.Quit();
            }
        }

        private void ToggleAllAvailableClickables() {
            if (Input.GetKeyDown(ShowAllAvailableClickablesKey)) {
                FmvClickableFacade[] clickables = FmvSceneVariables.VideoElementsPanel.GetComponentsInChildren<FmvClickableFacade>();
                for (int i = 0; i < clickables.Length; i++) {
                    clickables[i].ChangeVisibility(1);
                }
            }

            if (Input.GetKeyUp(ShowAllAvailableClickablesKey)) {
                FmvClickableFacade[] clickables = FmvSceneVariables.VideoElementsPanel.GetComponentsInChildren<FmvClickableFacade>();
                for (int i = 0; i < clickables.Length; i++) {
                    clickables[i].ChangeVisibility(0);
                }
            }
        }
    }
}