using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvVideos : MonoBehaviour {

        private const KeyCode ExportKeyCode = KeyCode.X;
        private const KeyCode SkipKeyCode = KeyCode.Escape;
        private const KeyCode PauseKeyKode = KeyCode.P;

        public FmvMakerVideoEvent OnVideoStarted = new FmvMakerVideoEvent();
        public FmvMakerVideoBoolEvent OnVideoPaused = new FmvMakerVideoBoolEvent();
        public FmvMakerVideoEvent OnVideoSkipped = new FmvMakerVideoEvent();
        public FmvMakerVideoEvent OnVideoFinished = new FmvMakerVideoEvent();

        [SerializeField]
        private FmvItems fmvItems = null;
        [SerializeField]
        private FmvNavigations fmvNavigations = null;
        [SerializeField]
        private string nameOfStartVideo = "";
        [SerializeField]
        private FmvVideoView videoView = null;

        private VideoModel[] allVideoElements;
        private VideoModel currentVideoElement;

        private bool itemsLoaded = false;
        private bool navigationsLoaded = false;

        private VideoModel StartVideo => GetVideoModelByName(nameOfStartVideo);

        private void Awake() {
            allVideoElements = FmvData.GenerateVideoDataFromLocalFile();

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

            PlayVideo(StartVideo);
        }

        private void Update() {
            SkipVideo();
            PauseVideo();
            ExportVideoData();
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

        private void DisablePreviousItems(VideoModel video) {
            if (!itemsLoaded) {
                fmvItems.DisableFindableItems();
            }
        }

        private void DisablePreviousNavigationTargets(VideoModel video) {
            if (!navigationsLoaded) {
                fmvNavigations.DisableNavigationTargets();
            }
        }

        private void ShowItemsAndNavigationsForLooping(VideoModel video) {
            if (video.IsLooping) {
                SetVideoToAlreadyWatched(video);
                ShowCurrentItems(video);
                ShowCurrentNavigationTargets(video);
            }
        }

        private void SetVideoToAlreadyWatched(VideoModel video) {
            if (!video.AlreadyWatched) {
                video.AlreadyWatched = true;
            }
        }

        private void CheckForInstantNextVideo(VideoModel video) {
            if ((video.NavigationTargets.Length == 1) && 
                fmvNavigations.GetNavigationModelByName(video.NavigationTargets[0].Name).DisplayText.Equals(string.Empty)) {
                PlayVideoFromNavigationTarget(video.NavigationTargets[0].Name);
            }
        }

        private void ShowCurrentItems(VideoModel video) {
            if (!itemsLoaded && (video.ItemsToFind?.Length > 0)) {
                fmvItems.EnableFindableItems(video.ItemsToFind);
                itemsLoaded = true;
            }
        }

        private void ShowCurrentNavigationTargets(VideoModel video) {
            if (!navigationsLoaded && (video.NavigationTargets?.Length > 0) &&
                !fmvNavigations.GetNavigationModelByName(video.NavigationTargets[0].Name).DisplayText.Equals(string.Empty)) {
                fmvNavigations.SetNavigationTargetsActive(video.NavigationTargets);
                navigationsLoaded = true;
            }
        }

        private void PlayVideo(VideoModel video) {
            Debug.Log($"Play video: {video.Name}");
            currentVideoElement = video;
            itemsLoaded = false;
            navigationsLoaded = false;
            videoView.PrepareAndPlay(video);
        }

        private void SkipVideo() {
            if (Input.GetKeyUp(SkipKeyCode) && !currentVideoElement.IsLooping && currentVideoElement.AlreadyWatched) {
                videoView.SkipVideoClip(currentVideoElement);
            }
        }

        private void PauseVideo() {
            if (Input.GetKeyUp(PauseKeyKode)) {
                videoView.PauseVideoClip(currentVideoElement);
            }
        }

        private void ExportVideoData() {
            if (Input.GetKeyUp(ExportKeyCode)) {
                FmvData.ExportVideoDataToLocalFile(allVideoElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private VideoModel GetVideoModelByName(string videoName) {
            return allVideoElements
                .SingleOrDefault((video) => video.Name.ToLower().Equals(videoName.ToLower()));
        }

        public bool CheckCurrentItemsForItemToUse(ItemModel itemModel) {
            if (currentVideoElement.ItemsToUse == null) {
                return false;
            }

            return currentVideoElement.ItemsToUse
                .Any((item) => item.Name.ToLower().Equals(itemModel.Name.ToLower()));
        }

        public void PlayVideoFromNavigationTarget(string navigationTargetName) {
            NavigationModel navigationModel = fmvNavigations.GetNavigationModelByName(navigationTargetName);
            PlayVideo(GetVideoModelByName(navigationModel.NextVideo));
        }
    }
}