using FmvMaker.Models;
using FmvMaker.Utils;
using FmvMaker.Views;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FmvMaker.Presenter {
    public class FmvMakerPresenter : MonoBehaviour {

        [SerializeField]
        private VideoView _view;
        [UsedImplicitly]
        [SerializeField]
        private RectTransform _videoElementsPanel = null;
        [SerializeField]
        private RectTransform _inventoryElementsPanel = null;

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
            _allVideoElements = FmvData.GenerateVideoDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);
            _allItemElements = FmvData.GenerateItemDataFromLocalFile(LoadFmvConfig.Config.LocalFilePath);

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
                FmvData.ExportItemDataToLocalFile(_allItemElements, LoadFmvConfig.Config.LocalFilePath);
            }
        }

        private void OnDestroy() {
            _view.OnLoopPointReached -= LoopPointReached;
        }

        public void PlayVideo(string videoName) {
            PlayVideo(_allVideoElements.FirstOrDefault(v => v.Name == videoName));
        }

        public void PlayVideo(VideoElement video) {
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
            if ((_currentVideoElement.NavigationTargets.Length < 1) || string.IsNullOrEmpty(_currentVideoElement.NavigationTargets[0].DisplayText)) {
                PlayVideo(_currentVideoElement.NavigationTargets[0].NextVideo);
            } else {
                GenerateNavigationElements();
                GenerateItemElements();
            }
        }

        private void GenerateNavigationElements() {
            for (int i = 0; i < _currentVideoElement.NavigationTargets.Length; i++) {

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

        private void GenerateItemElements() {
            for (int i = 0; i < _currentVideoElement.Items?.Length; i++) {

                ItemElement currentItem = _allItemElements.FirstOrDefault(item => item.Name.Equals(_currentVideoElement.Items[i]));

                if (!currentItem.IsInInventory && !currentItem.WasUsed) {
                    GameObject itemObject = ObjectPool.Instance.GetPooledItemObject();
                    itemObject.SetActive(true);
                    itemObject.transform.SetParent(_videoElementsPanel.transform);
                    itemObject.transform.localScale = Vector3.one;

                    ItemView view = itemObject.GetComponent<ItemView>();
                    view.SetItemData(currentItem);
                    view.OnItemClicked.AddListener(() => {
                        view.AddToInventory(_inventoryElementsPanel.transform);
                        currentItem.IsInInventory = true;
                        view.OnItemClicked.RemoveAllListeners();
                        view.OnItemClicked.AddListener(() => {
                            currentItem.WasUsed = true;
                            ObjectPool.Instance.RemoveItemObjectFromPool(itemObject);
                            itemObject.transform.SetParent(_videoElementsPanel.transform);
                            OnNavigationClicked(currentItem.NavigationTarget);
                        });
                    });
                }
            }
        }
    }
}