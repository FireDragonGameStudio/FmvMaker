using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvVideos : MonoBehaviour {

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
        private KeyCode QuitGameKey = KeyCode.Q;
        [SerializeField]
        private KeyCode ShowAllAvailableClickablesKey = KeyCode.Space;

        [Header("Settings")]
        [SerializeField]
        private string nameOfStartVideo = "";

        [Header("Internal references")]
        [SerializeField]
        private FmvClickableObjects clickableObjects = null;
        [SerializeField]
        private FmvVideoView videoView = null;
        [SerializeField]
        private FmvData data = null;
        [SerializeField]
        private GameObject loadingSceen = null;

        private VideoModel[] allVideoElements;
        private VideoModel currentVideoElement;

        private bool itemsLoaded = false;
        private bool navigationsLoaded = false;
        private bool alreadyLoaded = false;
        private bool useLoadingSceen = false;

        private VideoModel startVideo => GetVideoModelByName(nameOfStartVideo);

        private void Awake() {
            allVideoElements = data.GenerateVideoDataFromConfigurationFile();
            CheckForOnlineMappingData();

            SetupVideoEventTrigger();
        }

        private async void Start() {
            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            OnVideoStarted.AddListener(DisablePreviousItems);
            OnVideoStarted.AddListener(DisablePreviousNavigationTargets);
            OnVideoStarted.AddListener(ShowItemsAndNavigationsForLooping);

            OnVideoFinished.AddListener(SetVideoToAlreadyWatched);
            OnVideoFinished.AddListener(CheckForInstantNextVideo);
            OnVideoFinished.AddListener(ShowCurrentItems);
            OnVideoFinished.AddListener(ShowCurrentNavigationTargets);

            PlayVideo(startVideo);
            ConfigureLoadingScreen();
        }

        private void Update() {
            SkipVideo();
            PauseVideo();
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

        private void DisablePreviousItems(VideoModel video) {
            if (!itemsLoaded || !video.IsLooping) {
                clickableObjects.DisableFindableItems();
            }
        }

        private void StopLoadingScreen(VideoModel video) {
            loadingSceen.SetActive(false);
            alreadyLoaded = false;
        }

        private void DisablePreviousNavigationTargets(VideoModel video) {
            if (!navigationsLoaded || !video.IsLooping) {
                clickableObjects.DisableNavigationTargets();
            }
        }

        private void ShowItemsAndNavigationsForLooping(VideoModel video) {
            if (video.IsLooping) {
                SetVideoToAlreadyWatched(video);
                ShowCurrentItems(video);
                ShowCurrentNavigationTargets(video);
            }
        }

        private void ConfigureLoadingScreen() {
            loadingSceen.SetActive(false);

            if (LoadFmvConfig.Config.VideoSourceType != "INTERNAL") {
                useLoadingSceen = true;
                loadingSceen.SetActive(true);
                OnVideoStarted.AddListener(StopLoadingScreen);
                StartLoadingScreen(startVideo);
            }
        }

        private void StartLoadingScreen(VideoModel video) {
            if (useLoadingSceen && !alreadyLoaded) {
                loadingSceen.SetActive(true);
                alreadyLoaded = true;
            }
        }

        private void SetVideoToAlreadyWatched(VideoModel video) {
            if (!video.AlreadyWatched) {
                video.AlreadyWatched = true;
            }
        }

        private void CheckForInstantNextVideo(VideoModel video) {
            if (video.NavigationTargets.Length == 1) {
                ClickableModel navigationTargetItem = clickableObjects.GetNavigationItemModelByName(video.NavigationTargets[0].Name);
                if ((navigationTargetItem != null) && string.IsNullOrEmpty(navigationTargetItem.Description)) {
                    PlayVideoFromNavigationTarget(navigationTargetItem.PickUpVideo);
                }
            }
        }

        private void ShowCurrentItems(VideoModel video) {
            if (!itemsLoaded && (video.ItemsToFind?.Length > 0)) {
                clickableObjects.EnableFindableItems(video.ItemsToFind);
                itemsLoaded = true;
            }
        }

        private void ShowCurrentNavigationTargets(VideoModel video) {
            if (!navigationsLoaded && (video.NavigationTargets?.Length > 0)) {
                ClickableModel navigationTargetItem = clickableObjects.GetNavigationItemModelByName(video.NavigationTargets[0].Name);
                if ((navigationTargetItem != null) && !string.IsNullOrEmpty(navigationTargetItem.Description)) {
                    clickableObjects.SetNavigationTargetsActive(video.NavigationTargets);
                    navigationsLoaded = true;
                }
            }
        }

        private void PlayVideo(VideoModel video) {
            currentVideoElement = video;
            itemsLoaded = false;
            navigationsLoaded = false;
            videoView.PrepareAndPlay(video);
        }

        private void SkipVideo() {
            if (Input.GetKeyUp(SkipVideoKey) && !currentVideoElement.IsLooping && currentVideoElement.AlreadyWatched && videoView.ActivePlayer.IsPlaying) {
                videoView.SkipVideoClip(currentVideoElement);
            }
        }

        private void PauseVideo() {
            if (Input.GetKeyUp(PauseVideoKey) && !currentVideoElement.IsLooping) {
                videoView.PauseVideoClip(currentVideoElement);
            }
        }

        private void QuitGame() {
            if (Input.GetKeyUp(QuitGameKey)) {
                Application.Quit();
            }
        }

        private void ToggleAllAvailableClickables() {
            if (Input.GetKeyDown(ShowAllAvailableClickablesKey)) {
                clickableObjects.ToggleFindableItems(true);
            }

            if (Input.GetKeyUp(ShowAllAvailableClickablesKey)) {
                clickableObjects.ToggleFindableItems(false);
            }
        }

        private VideoModel GetVideoModelByName(string videoName) {
            return allVideoElements
                .SingleOrDefault((video) => video.Name.ToLower().Equals(videoName.ToLower()));
        }

        public bool CheckCurrentItemsForItemToUse(ClickableModel itemModel) {
            if (currentVideoElement.ItemsToUse == null) {
                return false;
            }

            return currentVideoElement.ItemsToUse
                .Any((item) => item.Name.ToLower().Equals(itemModel.Name.ToLower()));
        }

        public void PlayVideoFromNavigationTarget(string navigationTargetName) {
            VideoModel videoModel = GetVideoModelByName(navigationTargetName);
            StartLoadingScreen(videoModel);
            PlayVideo(videoModel);
        }
    }
}