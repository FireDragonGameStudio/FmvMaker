using FmvMaker.Models;
using FmvMaker.Utils;
using FmvMaker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FmvMaker.Presenter {
    public class FmvMakerPresenter : MonoBehaviour {

        [SerializeField]
        private VideoView _view;
        [SerializeField]
        private Canvas _canvas;

        private VideoElement _currentVideoElement;
        private List<VideoElement> _allVideoElements;

        void Awake() {
            if (!_view) {
                Debug.LogError("No video player for presenter set. I'll try to find it automatically. Please check if view is set, before starting playmode again.", this);
                _view = GetComponentInChildren<VideoView>();
            }
            _view.OnLoopPointReached += LoopPointReached;
        }

        void Start() {
            FmvMakerConfig config = LoadFmvConfig.LoadConfig();

            _allVideoElements = LoadFmvData.GenerateVideoMockData();

            // start first clip
            _currentVideoElement = _allVideoElements[0];
            PlayVideo(_currentVideoElement.Name);
        }

        void OnDestroy() {
            _view.OnLoopPointReached -= LoopPointReached;
        }

        public void PlayVideo(string videoName) {
            // either load from file system/online or resources
            _view.PlayVideoClip(_allVideoElements.FirstOrDefault(v => v.Name == videoName));
        }

        private void LoopPointReached() {
            GenerateNavigationElements();
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