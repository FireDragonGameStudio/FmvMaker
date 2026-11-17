using System;
using TMPro;
using UnityEngine;

namespace FmvMaker.Provider {
    /// <summary>
    /// Look of the choice buttons. Create one via Create > FmvMaker > Button Style, assign it as
    /// default style on the FmvVideos component, and optionally override it per choice in the graph.
    /// Changes are applied live while the game is running in the editor.
    /// </summary>
    [CreateAssetMenu(fileName = "ButtonStyle", menuName = "FmvMaker/Button Style")]
    public class FmvButtonStyle : ScriptableObject {

        public enum LabelPlacement {
            Center,
            Above,
            Below
        }

        [Header("Background")]
        [Tooltip("Optional image for the button. Leave empty for a plain rectangle. Images with borders (9-slice) are stretched nicely.")]
        public Sprite BackgroundImage;

        [Tooltip("Button color. Set the alpha (A) to 0 for an invisible hotspot.")]
        public Color NormalColor = new(1f, 1f, 1f, 0f);

        [Tooltip("Button color while the mouse is over it.")]
        public Color HoverColor = new(1f, 1f, 1f, 0.25f);

        [Tooltip("Button color while it's being clicked.")]
        public Color PressedColor = new(1f, 1f, 1f, 0.4f);

        [Tooltip("How long the color change takes, in seconds.")]
        [Range(0f, 1f)]
        public float FadeDuration = 0.1f;

        [Header("Label")]
        [Tooltip("Show the label text of the choice. The text itself is set per choice in the graph.")]
        public bool ShowLabel = true;

        [Tooltip("Where the label is placed, relative to the button.")]
        public LabelPlacement Placement = LabelPlacement.Center;

        [Tooltip("Distance between button and label, when the label is above or below the button.")]
        [Min(0f)]
        public float LabelSpacing = 10f;

        [Tooltip("Optional font. Leave empty to use the default font.")]
        public TMP_FontAsset Font;

        [Min(1f)]
        public float FontSize = 36f;

        [Tooltip("Shrinks the text to fit the button. Only used when the label is centered.")]
        public bool AutoSizeFont;

        public FontStyles FontStyle = FontStyles.Normal;

        [Tooltip("Label color. A color set on a choice in the graph replaces this color, but keeps this transparency.")]
        public Color LabelColor = Color.white;

        [Tooltip("Label color while the mouse is over the button.")]
        public Color LabelHoverColor = Color.white;

        /// <summary>Raised when the style is edited in the Inspector, so spawned buttons update live.</summary>
        public event Action Changed;

        /// <summary>True when the button background can't be seen in any state.</summary>
        public bool IsBackgroundInvisible => NormalColor.a <= 0f && HoverColor.a <= 0f && PressedColor.a <= 0f;

#if UNITY_EDITOR
        private void OnValidate() {
            Changed?.Invoke();
        }
#endif
    }
}
