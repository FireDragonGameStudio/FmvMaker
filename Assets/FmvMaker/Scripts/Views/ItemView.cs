﻿using FmvMaker.Models;
using FmvMaker.Core.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FmvMaker.Views {
    public class ItemView : MonoBehaviour {

        public ItemClickEvent OnItemClicked = new ItemClickEvent();

        [SerializeField]
        private Button _itemButton;
        [SerializeField]
        private Image _itemImage;
        [SerializeField]
        private Text _itemText;

        private RectTransform _rectTransform;

        private void Awake() {
            _itemButton = GetComponent<Button>();
            _itemImage = GetComponent<Image>();
            _itemText = GetComponentInChildren<Text>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnDisable() {
            _itemButton.onClick.RemoveAllListeners();
            OnItemClicked.RemoveAllListeners();
        }

        public void SetItemData(ItemModel model) {
            LoadImageSprite(model.Name);
            _itemText.text = model.DisplayText;
            _rectTransform.anchoredPosition = FmvData.GetRelativeScreenPosition(model.RelativeScreenPosition);
            _itemButton.onClick.AddListener(() => OnItemClicked?.Invoke(model));
        }

        private IEnumerator LoadImageSpriteCoroutine(string spritePath) {
            UnityWebRequest www = UnityWebRequest.Get(spritePath);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            www.downloadHandler = texDl;

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                Texture2D texture = texDl.texture;
                _itemImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }

        private void LoadImageSprite(string spriteName) {
            if (LoadFmvConfig.Config.SourceType.Equals("LOCAL")) {
                StartCoroutine(LoadImageSpriteCoroutine(ResourceInfo.LoadItemImageFromFile(spriteName)));
            } else if (LoadFmvConfig.Config.SourceType.Equals("ONLINE")) {
                StartCoroutine(LoadImageSpriteCoroutine(ResourceInfo.LoadItemImageFromOnlineSource(spriteName)));
            } else {
                _itemImage.sprite = Resources.Load<Sprite>("Textures/spriteName");
            }
        }
    }
}