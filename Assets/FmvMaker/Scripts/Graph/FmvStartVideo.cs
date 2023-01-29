using FmvMaker.Core.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvStartVideo : Unit {

        [DoNotSerialize]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput FmvVideoPlayer;
        [DoNotSerialize]
        public ValueInput FmvAudioPlayer;

        [DoNotSerialize]
        public ValueOutput StartVideo;

        private VideoPlayer videoPlayer;
        private AudioSource audioPlayer;

        private VideoModel resultValue;

        protected override void Definition() {
            InputTrigger = ControlInput("inputTrigger", (flow) => {
                // load start video
                videoPlayer = flow.GetValue<VideoPlayer>(FmvVideoPlayer);
                videoPlayer.targetTexture.Release();
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = Resources.Load<VideoClip>($"FmvMakerVideos/UniqueVideoName");
                videoPlayer.isLooping = false;
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += PreparationComplete;

                audioPlayer = flow.GetValue<AudioSource>(FmvAudioPlayer);

                resultValue = new VideoModel() {
                    Name = "UniqueVideoName",
                    IsLooping = false
                };
                return OutputTrigger;
            });
            OutputTrigger = ControlOutput("outputTrigger");

            FmvVideoPlayer = ValueInput<VideoPlayer>("VideoToPlay");
            FmvAudioPlayer = ValueInput<AudioSource>("AudioToPlay");
            StartVideo = ValueOutput<VideoModel>("result", (flow) => { return resultValue; });
        }

        private void PreparationComplete(VideoPlayer source) {
            videoPlayer.Play();
            audioPlayer.Play();
            Debug.Log("Playing?");
        }
    }
}