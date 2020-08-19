using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace FmvMaker.Core.Facades {
    [RequireComponent(typeof(Button))]
    public class FmvNavigationFacade : MonoBehaviour {

        public NavigationClickEvent OnNavigationClicked = new NavigationClickEvent();

        [SerializeField]
        private Button navigationButton;
        [SerializeField]
        private Text navigationText;

        private RectTransform rectTransform;

        private void Awake() {
            navigationButton = GetComponent<Button>();
            navigationText = GetComponentInChildren<Text>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy() {
            navigationButton.onClick.RemoveAllListeners();
            OnNavigationClicked.RemoveAllListeners();
        }

        public void SetNavigationData(NavigationModel model) {
            gameObject.name = model.Name;
            navigationText.text = model.DisplayText;
            rectTransform.anchoredPosition = FmvData.GetRelativeScreenPosition(model.RelativeScreenPosition);

            navigationButton.onClick.AddListener(() => OnNavigationClicked?.Invoke(model));
        }
    }
}