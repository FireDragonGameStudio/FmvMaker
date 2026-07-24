using FmvMaker.Core.Interfaces;
using FmvMaker.Models;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Facades {
    [RequireComponent(typeof(AudioSource), typeof(VideoPlayer))]
    public class FmvVideoFacade : MonoBehaviour, IVideoEvents {

        public event Action OnPreparationCompleted;
        public event Action<VideoClip> OnPlayerStarted;
        public event Action<VideoClip> OnLoopPointReached;

        private VideoPlayer videoPlayer;
        private AudioSource audioSource;

        public bool IsLooping => videoPlayer.isLooping;
        public bool IsPlaying => videoPlayer.isPlaying;
        public VideoClip VideoClip => videoPlayer.clip;

        public bool IsMute {
            get => isMuted;
            set {
                isMuted = value;
                MuteAudio(isMuted);
            }
        }
        private bool isMuted = false;

        private void Awake() {
            videoPlayer = GetComponent<VideoPlayer>();
            audioSource = GetComponent<AudioSource>();

            ReleaseRenderTexture();
            SetupUnityVideoEvents();
        }

        private void OnDestroy() {
            DisposeUnityVideoEvents();
            ReleaseRenderTexture();
        }

        private void ReleaseRenderTexture() {
            videoPlayer.targetTexture.Release();
        }

        private void SetupUnityVideoEvents() {
            videoPlayer.started += PlayerStarted;
            videoPlayer.prepareCompleted += PreparationComplete;
            videoPlayer.loopPointReached += LoopPointReached;
        }

        private void DisposeUnityVideoEvents() {
            videoPlayer.started -= PlayerStarted;
            videoPlayer.prepareCompleted -= PreparationComplete;
            videoPlayer.loopPointReached -= LoopPointReached;
        }

        private void PlayerStarted(VideoPlayer source) {
            // don't fire started event on resume
            // to prevent multiple button spawning
            if (source.frame != -1) {
                return;
            }

            OnPlayerStarted?.Invoke(source.clip);
        }

        private void PreparationComplete(VideoPlayer sourc) {
            MuteAudio(IsMute);

            OnPreparationCompleted?.Invoke();
        }

        private void LoopPointReached(VideoPlayer source) {
            OnLoopPointReached?.Invoke(source.clip);
        }

        private void MuteAudio(bool mute) {
            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.None) {
                return;
            }

            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct && videoPlayer.canSetDirectAudioVolume) {
                for (ushort i = 0; i < videoPlayer.audioTrackCount; i++) {
                    videoPlayer.SetDirectAudioMute(i, mute);
                }
                return;
            }

            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
                audioSource.mute = mute;
            }

            // API not necessary (yet?)
        }

        public void Play() {
            videoPlayer.Play();
            audioSource.Play();
        }

        public void Pause() {
            videoPlayer.Pause();
            audioSource.Pause();
        }

        public void Stop() {
            videoPlayer.Stop();
            audioSource.Stop();
            videoPlayer.isLooping = false;
        }

        public void Prepare(VideoModel videoModel) {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoModel.VideoClip;
            videoPlayer.isLooping = videoModel.IsLooping;

            // add audio source manually
            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
                videoPlayer.SetTargetAudioSource(0, audioSource);
            }

            videoPlayer.Prepare();
        }

        public void Skip() {
            if (!videoPlayer.isLooping && videoPlayer.isPlaying) {
                videoPlayer.frame = (long)videoPlayer.frameCount - 5;
            } else {
                LoopPointReached(videoPlayer);
            }
        }
    }
}