using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public class DynamicVideoResolution : MonoBehaviour {

        public static DynamicVideoResolution Instance = null;

        public delegate void ScreenSizeChangeEventHandler(float width, float height);
        public event ScreenSizeChangeEventHandler ScreenSizeChanged;

        [Header("Internal references")]
        [SerializeField]
        private RectTransform fmvVideoElementPanel = null;

        private Vector2 lastScreenSize;

        private void Awake() {
            lastScreenSize = new Vector2(fmvVideoElementPanel.rect.width, fmvVideoElementPanel.rect.height);
            Instance = this;
        }

        private void Update() {
            Vector2 screenSize = new Vector2(fmvVideoElementPanel.rect.width, fmvVideoElementPanel.rect.height);

            if (this.lastScreenSize != screenSize) {
                this.lastScreenSize = screenSize;
                OnScreenSizeChange(screenSize.x, screenSize.y);
            }
        }

        protected virtual void OnScreenSizeChange(float width, float height) {
            ScreenSizeChanged?.Invoke(width, height);
        }

        public static Vector2 GetRelativeScreenPosition(float offsetFactorX, float offsetFactorY) {
            return new Vector2(Instance.lastScreenSize.x * offsetFactorX,
                Instance.lastScreenSize.y * offsetFactorY);
        }

        public static Vector2 GetRelativeScreenPosition(Vector2 relativePosition) {
            return GetRelativeScreenPosition(relativePosition.x, relativePosition.y);
        }
    }
}