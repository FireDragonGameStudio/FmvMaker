﻿using FmvMaker.Models;
using FmvMaker.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace FmvMaker.Views {
    [RequireComponent(typeof(Button))]
    public class NavigationView : MonoBehaviour {

        public NavigationClickEvent OnNavigationClicked = new NavigationClickEvent();

        [SerializeField]
        private Button _navigationButton;
        [SerializeField]
        private Text _navigationText;

        private RectTransform _rectTransform;

        void Awake() {
            _navigationButton = GetComponent<Button>();
            _navigationText = GetComponentInChildren<Text>();
            _rectTransform = GetComponent<RectTransform>();
        }

        void OnDisable() {
            _navigationButton.onClick.RemoveAllListeners();
            OnNavigationClicked.RemoveAllListeners();
        }

        public void SetTargetData(NavigationModel model) {
            _navigationText.text = model.DisplayText;
            _rectTransform.anchoredPosition = new Vector2(model.RelativeScreenPosition.x, model.RelativeScreenPosition.y);

            _navigationButton.onClick.AddListener(() => OnNavigationClicked?.Invoke(model));

            // preload next target data
        }
    }
}