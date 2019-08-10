using FmvMaker.Models;
using FmvMaker.Utils;
using FmvMaker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Presenter {
    public class FmvMakerPresenter : MonoBehaviour {

        [SerializeField]
        private VideoView _view;
        [SerializeField]
        private Canvas _canvas;

        private VideoElement _currentVideoElement;
        private List<VideoElement> _allVideoElements;
        private List<ItemElement> _allItemElements;
        private int _loopCounter = 0;

        private void Awake() {
            if (!_view) {
                Debug.LogError("No video view for presenter set. I'll try to find it automatically. Please check if view is set, before starting playmode again.", this);
                _view = GetComponentInChildren<VideoView>();
            }
            _view.OnLoopPointReached += LoopPointReached;
        }

        private async void Start() {
            _allVideoElements = FmvData.GenerateVideoDataFromLocalFile(LoadFmvConfig.Config.LocalVideoPath);
            _allItemElements = FmvData.GenerateItemMockData();

            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            // start first clip
            PlayVideo(_allVideoElements[0]);
        }

        private void Update() {
            // check for video skipping
            if (Input.GetKeyUp(KeyCode.Escape)) {
                _view.SkipVideoClip();
            }
            if (Input.GetKeyUp(KeyCode.E)) {
                FmvData.ExportVideoDataToLocalFile(_allVideoElements, LoadFmvConfig.Config.LocalVideoPath);
            }
        }

        private void OnDestroy() {
            _view.OnLoopPointReached -= LoopPointReached;
        }

        public void PlayVideo(string videoName) {
            PlayVideo(_allVideoElements.FirstOrDefault(v => v.Name == videoName));
        }

        public void PlayVideo(VideoElement video) {
            // either load from file system/online or resources
            Debug.Log($"Play video: {video.Name}");
            _loopCounter = 0;
            _currentVideoElement = video;
            _view.PrepareAndPlayVideoClip(video);
        }

        private void LoopPointReached() {
            _loopCounter++;

            // don´t show navigation elements again, when looping
            if (_loopCounter > 1) {
                return;
            }

            // check for next video to either generate navigation elements
            // or move directly to the next video
            if ((_currentVideoElement.NavigationTargets.Length < 1) || string.IsNullOrEmpty(_currentVideoElement.NavigationTargets[0].DisplayText)) {
                PlayVideo(_currentVideoElement.NavigationTargets[0].NextVideo);
            } else {
                GenerateNavigationElements();
            }
        }

        private void GenerateNavigationElements() {
            for (int i = 0; i < _currentVideoElement.NavigationTargets.Length; i++) {
                // rent new navigation targets from pool
                GameObject targetObject = ObjectPool.Instance.GetPooledTargetObject();
                targetObject.SetActive(true);
                targetObject.transform.SetParent(_canvas.transform);
                targetObject.transform.localScale = Vector3.one;
                targetObject.GetComponent<NavigationView>().SetTargetData(_currentVideoElement.NavigationTargets[i]);
                targetObject.GetComponent<NavigationView>().OnNavigationClicked.AddListener(OnNavigationClicked);
            }
        }

        private void OnNavigationClicked(NavigationModel model) {
            ObjectPool.Instance.ReturnAllTargetObjectsToPool();
            PlayVideo(model.NextVideo);
        }
    }
}