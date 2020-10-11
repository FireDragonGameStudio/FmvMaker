﻿using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public class DynamicVideoResolution : MonoBehaviour {

        public static DynamicVideoResolution Instance = null;

        public delegate void ScreenSizeChangeEventHandler(float width, float height);
        public event ScreenSizeChangeEventHandler ScreenSizeChanged;

        public Vector2 lastScreenSize { get; private set; }

        [SerializeField]
        private RectTransform fmvVideoElementPanel = null;

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
    }
}