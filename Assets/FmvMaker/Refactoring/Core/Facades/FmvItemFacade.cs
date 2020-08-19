﻿using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FmvMaker.Core.Facades {
    public class FmvItemFacade : MonoBehaviour {

        public ItemClickEvent OnItemClicked = new ItemClickEvent();

        [SerializeField]
        private Button itemButton;
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private Text itemText;

        private RectTransform rectTransform;

        private void Awake() {
            itemButton = GetComponent<Button>();
            itemImage = GetComponent<Image>();
            itemText = GetComponentInChildren<Text>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy() {
            itemButton.onClick.RemoveAllListeners();
            OnItemClicked.RemoveAllListeners();
        }

        public void SetItemData(ItemModel model) {
            //LoadImageSprite(model.Name);
            itemImage.sprite = Resources.Load<Sprite>($"Textures/{model.Name}");
            gameObject.name = model.Name;
            itemText.text = model.DisplayText;
            rectTransform.anchoredPosition = FmvData.GetRelativeScreenPosition(model.RelativeScreenPosition);
            itemButton.onClick.AddListener(() => OnItemClicked?.Invoke(model));
        }

        //private IEnumerator LoadImageSpriteCoroutine(string spritePath) {
        //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(spritePath);

        //    yield return www.SendWebRequest();

        //    if (www.isNetworkError || www.isHttpError) {
        //        Debug.LogError(www.error);
        //    } else {
        //        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        //        itemImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        //    }
        //}

        //private void LoadImageSprite(string spriteName) {
            //if (LoadFmvConfig.Config.SourceType.Equals("LOCAL")) {
            //    StartCoroutine(LoadImageSpriteCoroutine(ResourceVideoInfo.LoadItemImageFromFile(spriteName)));
            //} else if (LoadFmvConfig.Config.SourceType.Equals("ONLINE")) {
            //    StartCoroutine(LoadImageSpriteCoroutine(ResourceVideoInfo.LoadItemImageFromOnlineSource(spriteName)));
            //} else {
            //    itemImage.sprite = Resources.Load<Sprite>($"Textures/{spriteName}");
            //}
        //}
    }
}