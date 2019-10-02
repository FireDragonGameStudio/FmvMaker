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
        private RectTransform _videoElementsPanel = null;

        private VideoModel _currentVideoElement;
        private List<VideoModel> _allVideoElements;
        private int _loopCounter = 0;

        private void Awake() {
            if (!_view) {
                Debug.LogError("No video view for presenter set. I'll try to find it automatically. Please check if view is set, before starting playmode again.", this);
                _view = GetComponentInChildren<VideoView>();
            }
            _view.OnLoopPointReached += LoopPointReached;
        }

        private async void Start() {
            _allVideoElements = FmvData.GenerateVideoDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);


            // wait a short time for Unity to get correct values for screen height and width
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            PlayVideo(_allVideoElements[0]);
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                _view.SkipVideoClip();
            }
            if (Input.GetKeyUp(KeyCode.X)) {
                FmvData.ExportVideoDataToLocalFile(_allVideoElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private void OnDestroy() {
            _view.OnLoopPointReached -= LoopPointReached;
        }

        public void PlayVideo(string videoName) {
            PlayVideo(_allVideoElements.FirstOrDefault(v => v.Name == videoName));
        }

        public void PlayVideo(VideoModel video) {
            Debug.Log($"Play video: {video.Name}");
            _loopCounter = 0;
            _currentVideoElement = video;
            _view.PrepareAndPlayVideoClip(video);
        }

        private void LoopPointReached() {
            _loopCounter++;

            // don´t load navigation elements again, when looping
            if (_loopCounter > 1) {
                return;
            }

            // check for next video to either generate navigation elements
            // or move directly to the next video
            if ((_currentVideoElement.NavigationTargets.Count < 1) || string.IsNullOrEmpty(_currentVideoElement.NavigationTargets[0].DisplayText)) {
                PlayVideo(_currentVideoElement.NavigationTargets[0].NextVideo);
            } else {
                GenerateNavigationElements();
                GenerateItemsToFind();
            }
        }

        private void GenerateNavigationElements() {
            for (int i = 0; i < _currentVideoElement.NavigationTargets.Count; i++) {

                GameObject targetObject = ObjectPool.Instance.GetPooledTargetObject();
                targetObject.SetActive(true);
                targetObject.transform.SetParent(_videoElementsPanel.transform);
                targetObject.transform.localScale = Vector3.one;

                NavigationView view = targetObject.GetComponent<NavigationView>();
                view.SetTargetData(_currentVideoElement.NavigationTargets[i]);
                view.OnNavigationClicked.AddListener(OnNavigationClicked);
            }
        }

        private void OnNavigationClicked(NavigationModel model) {
            ObjectPool.Instance.ReturnAllTargetObjectsToPool();
            ObjectPool.Instance.ReturnAllItemObjectsToPool();
            PlayVideo(model.NextVideo);
        }

        private void GenerateItemsToFind() {
            if (_currentVideoElement.ItemsToFind != null) {
                for (int i = 0; i < _currentVideoElement.ItemsToFind.Count; i++) {
                    if (!_currentVideoElement.ItemsToFind[i].IsInInventory) {
                        GameObject targetObject = ObjectPool.Instance.GetPooledItemObject();
                        targetObject.SetActive(true);
                        targetObject.transform.SetParent(_videoElementsPanel.transform);
                        targetObject.transform.localScale = Vector3.one;

                        ItemView view = targetObject.GetComponent<ItemView>();
                        view.SetItemData(_currentVideoElement.ItemsToFind[i]);
                        view.OnItemClicked.AddListener((model) => {
                            view.OnItemClicked.RemoveAllListeners();
                            model.IsInInventory = true;
                            OnNavigationClicked(model.NavigationTarget);
                            // add item to inventory
                        });
                    }
                }
            }
        }

        private void GenerateItemsToUse() {
            if (_currentVideoElement.ItemsToUse != null) {
                for (int i = 0; i < _currentVideoElement.ItemsToUse.Count; i++) {
                    if (!_currentVideoElement.ItemsToUse[i].WasUsed) {
                        // Generate Drag&Drop Drop event which checks the item that
                        // was dropped and eventually launches a video sequence.
                        // Maybe keep a default value in mind for non-usable items.
                        // Like:
                        // OnDrop.AddListener(check if dropped item is usable - launch video)
                    }
                }
            }
        }
    }
}