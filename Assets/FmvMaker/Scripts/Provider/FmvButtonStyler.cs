using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FmvMaker.Provider {
    /// <summary>
    /// Applies a <see cref="FmvButtonStyle"/> to a spawned choice button and keeps it up to date
    /// when the style is edited during play mode. Added at runtime by <see cref="FmvVideos"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class FmvButtonStyler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        // drawn on invisible hotspots when "Show Navigation UI Buttons" is enabled in FmvSettings
        private static readonly Color DebugHotspotColor = new(1f, 0f, 1f, 0.35f);

        private FmvButtonStyle style;
        private Color? labelColorOverride;
        private bool highlightInvisibleHotspot;
        private bool isHovered;

        private Button button;
        private Image image;
        private TextMeshProUGUI label;
        private Sprite defaultSprite;

        /// <param name="labelColorOverride">Color set on the choice in the graph, or null to use the style's label color.</param>
        /// <param name="highlightInvisibleHotspot">Draws invisible hotspots in a debug color, to see where they are while testing.</param>
        public void Init(FmvButtonStyle buttonStyle, Color? labelColorOverride, bool highlightInvisibleHotspot) {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            label = GetComponentInChildren<TextMeshProUGUI>(true);
            defaultSprite = image != null ? image.sprite : null;

            this.labelColorOverride = labelColorOverride;
            this.highlightInvisibleHotspot = highlightInvisibleHotspot;

            SetStyle(buttonStyle);
        }

        private void SetStyle(FmvButtonStyle buttonStyle) {
            if (style != null) style.Changed -= Apply;
            style = buttonStyle;
            if (style != null && isActiveAndEnabled) style.Changed += Apply;
            Apply();
        }

        private void OnEnable() {
            if (style != null) {
                style.Changed -= Apply;
                style.Changed += Apply;
            }
        }

        private void OnDisable() {
            if (style != null) style.Changed -= Apply;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            isHovered = true;
            ApplyLabelColor();
        }

        public void OnPointerExit(PointerEventData eventData) {
            isHovered = false;
            ApplyLabelColor();
        }

        private void Apply() {
            if (style == null || this == null) return;

            ApplyBackground();
            ApplyLabel();
        }

        private void ApplyBackground() {
            if (image != null) {
                image.sprite = style.BackgroundImage != null ? style.BackgroundImage : defaultSprite;
                image.type = image.sprite != null && image.sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;
                // the button's color tint multiplies this color, so the tint colors below are the final colors
                image.color = Color.white;
            }

            if (button != null) {
                var normal = style.NormalColor;
                if (highlightInvisibleHotspot && style.IsBackgroundInvisible) {
                    normal = DebugHotspotColor;
                }

                button.transition = Selectable.Transition.ColorTint;
                var colors = button.colors;
                colors.normalColor = normal;
                colors.highlightedColor = style.HoverColor;
                colors.pressedColor = style.PressedColor;
                colors.selectedColor = style.HoverColor;
                colors.colorMultiplier = 1f;
                colors.fadeDuration = style.FadeDuration;
                button.colors = colors;
            }
        }

        private void ApplyLabel() {
            if (label == null) return;

            label.gameObject.SetActive(style.ShowLabel && !string.IsNullOrEmpty(label.text));
            if (style.Font != null) label.font = style.Font;
            label.fontStyle = style.FontStyle;
            label.alignment = TextAlignmentOptions.Center;

            var rect = label.rectTransform;
            var lineHeight = style.FontSize * 1.4f;
            switch (style.Placement) {
                case FmvButtonStyle.LabelPlacement.Above:
                    SetLabelLayout(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 0f), new Vector2(0f, style.LabelSpacing), lineHeight);
                    break;
                case FmvButtonStyle.LabelPlacement.Below:
                    SetLabelLayout(rect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 1f), new Vector2(0f, -style.LabelSpacing), lineHeight);
                    break;
                default:
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = Vector2.zero;
                    label.textWrappingMode = TextWrappingModes.Normal;
                    label.overflowMode = TextOverflowModes.Overflow;
                    break;
            }

            // auto size only makes sense inside the button
            var autoSize = style.AutoSizeFont && style.Placement == FmvButtonStyle.LabelPlacement.Center;
            label.enableAutoSizing = autoSize;
            if (autoSize) {
                label.fontSizeMin = 1f;
                label.fontSizeMax = style.FontSize;
            } else {
                label.fontSize = style.FontSize;
            }

            ApplyLabelColor();
        }

        private void SetLabelLayout(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, float height) {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(0f, height);
            // labels outside the button may be wider than the button
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Overflow;
        }

        private void ApplyLabelColor() {
            if (label == null || style == null) return;

            var styleColor = isHovered ? style.LabelHoverColor : style.LabelColor;
            if (labelColorOverride.HasValue && !isHovered) {
                var choiceColor = labelColorOverride.Value;
                styleColor = new Color(choiceColor.r, choiceColor.g, choiceColor.b, styleColor.a);
            }
            label.color = styleColor;
        }

        private void OnDestroy() {
            if (style != null) style.Changed -= Apply;
        }
    }
}
