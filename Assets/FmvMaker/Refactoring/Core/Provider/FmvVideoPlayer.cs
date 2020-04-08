using FmvMaker.Models;
using FmvMaker.Core.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvVideoPlayer : MonoBehaviour {

        [SerializeField]
        private VideoView videoView;

        private VideoModel currentVideoElement;
        private VideoModel[] allVideoElements;
        private int loopCounter = 0;

        private async void Start() {
            allVideoElements = FmvData.GenerateVideoDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);


            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            PlayVideo(allVideoElements[0]);
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                videoView.SkipVideoClip();
            }
            if (Input.GetKeyUp(KeyCode.X)) {
                FmvData.ExportVideoDataToLocalFile(allVideoElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private void PlayVideo(VideoModel video) {
            Debug.Log($"Play video: {video.Name}");
            loopCounter = 0;
            currentVideoElement = video;
            videoView.PrepareAndPlayVideoClip(video);
        }
    }
}