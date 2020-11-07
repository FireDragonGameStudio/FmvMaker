using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FmvMaker.Core.Facades {
    public class FmvClickableFacade : MonoBehaviour {

        public ClickableClickEvent OnItemClicked = new ClickableClickEvent();

        [SerializeField]
        private Button itemButton;
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private Text itemText;

        private RectTransform rectTransform;
        private ClickableModel clickableModel;

        private void Awake() {
            itemButton = GetComponent<Button>();
            itemImage = GetComponent<Image>();
            itemText = GetComponentInChildren<Text>();
            rectTransform = GetComponent<RectTransform>();
            DynamicVideoResolution.Instance.ScreenSizeChanged += OnScreenSizeChanged;
        }

        private void OnDestroy() {
            itemButton.onClick.RemoveAllListeners();
            OnItemClicked.RemoveAllListeners();
        }

        public void SetItemData(ClickableModel model) {
            //LoadImageSprite(model.Name);
            clickableModel = model;
            gameObject.name = model.Name;
            itemImage.sprite = Resources.Load<Sprite>($"FmvMakerTextures/{model.Name}");
            itemText.text = model.DisplayText;
            itemText.enabled = !string.IsNullOrEmpty(model.DisplayText);

            // set default sprite if necessary
            if (!itemImage.sprite) {
                itemImage.sprite = Resources.Load<Sprite>($"FmvMakerTextures/default");
                itemText.text = model.Name;
                itemText.enabled = true;
            }

            rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(model.RelativeScreenPosition);
            itemButton.onClick.AddListener(() => OnItemClicked?.Invoke(model));
        }

        private void OnScreenSizeChanged(float width, float height) {
            rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(clickableModel.RelativeScreenPosition);
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