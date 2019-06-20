using FmvMaker.Models;
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

        private void Awake() {
            _navigationButton = GetComponent<Button>();
            _navigationText = GetComponentInChildren<Text>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnDisable() {
            _navigationButton.onClick.RemoveAllListeners();
            OnNavigationClicked.RemoveAllListeners();
        }

        public void SetTargetData(NavigationModel model) {
            _navigationText.text = model.DisplayText;
            _rectTransform.anchoredPosition = FmvData.GetRelativeScreenPosition(model.RelativeScreenPosition);

            _navigationButton.onClick.AddListener(() => OnNavigationClicked?.Invoke(model));
        }
    }
}