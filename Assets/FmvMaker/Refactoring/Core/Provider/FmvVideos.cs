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
        public FmvMakerVideoEvent OnVideoPaused = new FmvMakerVideoEvent();
        public FmvMakerVideoEvent OnVideoSkipped = new FmvMakerVideoEvent();
        public FmvMakerVideoEvent OnVideoLoopPointReached = new FmvMakerVideoEvent();

        [SerializeField]
        private string nameOfStartVideo = "";
        [SerializeField]
        private FmvVideoView videoView = null;

        private VideoModel[] allVideoElements;
        private VideoModel currentVideoElement;

        private VideoModel StartVideo => GetVideoModelByName(nameOfStartVideo);

        private void Awake() {
            allVideoElements = FmvData.GenerateVideoDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);
        }

        private async void Start() {
            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            PlayVideo(StartVideo);
        }

        private void Update() {
            SkipVideo();
            PauseVideo();
            ExportVideoData();
        }

        private void ExportVideoData() {
            if (Input.GetKeyUp(ExportKeyCode)) {
                FmvData.ExportVideoDataToLocalFile(allVideoElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private void SkipVideo() {
            if (Input.GetKeyUp(SkipKeyCode)) {
                OnVideoSkipped?.Invoke(currentVideoElement);
                videoView.SkipVideoClip();
            }
        }

        private void PauseVideo() {
            if (Input.GetKeyUp(PauseKeyKode)) {
                OnVideoPaused?.Invoke(currentVideoElement);
                videoView.PauseVideoClip();
            }
        }

        private void PlayVideo(VideoModel video) {
            Debug.Log($"Play video: {video.Name}");
            currentVideoElement = video;
            OnVideoStarted?.Invoke(video);
            videoView.PrepareAndPlay(video);
        }

        private VideoModel GetVideoModelByName(string videoName) {
            return allVideoElements
                .SingleOrDefault((video) => video.Name.ToLower().Equals(videoName.ToLower()));
        }

        private NavigationModel[] GetNavigationModelsByVideoName(string videoName) {
            return GetVideoModelByName(videoName).NavigationTargets;
        }

        public ItemModel[] GetItemsToFindByVideoName(string videoName) {
            return GetVideoModelByName(videoName).ItemsToFind;
        }

        public ItemModel[] GetItemsToUseByVideoName(string videoName) {
            return GetVideoModelByName(videoName).ItemsToUse;
        }

        public bool CheckCurrentItemsForItemToUse(ItemModel itemModel) {
            return currentVideoElement.ItemsToUse
                .SingleOrDefault((item) => item.Name.ToLower().Equals(itemModel.Name.ToLower())) != null ? true : false;
        }
    }
}