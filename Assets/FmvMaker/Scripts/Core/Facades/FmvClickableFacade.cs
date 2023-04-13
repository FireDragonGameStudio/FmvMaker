using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FmvMaker.Core.Facades {
    public class FmvClickableFacade : MonoBehaviour {

        public ClickableClickEvent OnItemClicked = new ClickableClickEvent();

        public bool IsButtonTransparent {
            get => isButtonTransparent;
            set => isButtonTransparent = value;
        }

        [SerializeField]
        private bool isButtonTransparent = false;
        [SerializeField]
        private Button itemButton;
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TextMeshProUGUI itemText;

        private RectTransform rectTransform;
        private ClickableModel clickableModel;

        private void Awake() {
            if (!itemButton || !itemImage || !itemText) {
                itemButton = GetComponent<Button>();
                itemImage = GetComponent<Image>();
                itemText = GetComponentInChildren<TextMeshProUGUI>();
            }

            rectTransform = GetComponent<RectTransform>();
            DynamicVideoResolution.Instance.ScreenSizeChanged += OnScreenSizeChanged;
        }

        private void OnDestroy() {
            itemButton.onClick.RemoveAllListeners();
            OnItemClicked.RemoveAllListeners();
            DynamicVideoResolution.Instance.ScreenSizeChanged -= OnScreenSizeChanged;
        }

        private void OnScreenSizeChanged(float width, float height) {
            rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(clickableModel.RelativeScreenPosition);
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

            // set to transparent if enabled
            if (isButtonTransparent) {
                itemImage.color = new Color(1, 1, 1, 0);
                itemText.color = new Color(itemText.color.r, itemText.color.g, itemText.color.b, 0);
            }

            rectTransform.anchoredPosition = DynamicVideoResolution.GetRelativeScreenPosition(model.RelativeScreenPosition);
            itemButton.onClick.AddListener(() => OnItemClicked?.Invoke(model));
        }

        public void ChangeVisibility(int alphaValue) {
            if (isButtonTransparent) {
                itemImage.color = new Color(1, 1, 1, alphaValue);
                itemText.color = new Color(itemText.color.r, itemText.color.g, itemText.color.b, alphaValue);
            }
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