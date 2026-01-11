using FmvMaker.Core.Facades;
using UnityEngine;

namespace FmvMaker.Provider {
    public class FmvSettings : MonoBehaviour {
        [Header("Debug Settings")]
        [SerializeField] private bool resetInventoryOnStart;
        [SerializeField] private bool alwaysStartFromBeginning;
        [SerializeField] private bool showNavigationUiButtons;

        [Header("Internal References")]
        [SerializeField] private FmvVideos fmvVideos;
        [SerializeField] private FmvInventory inventory;

        private void Awake() {
            if (resetInventoryOnStart) {
                inventory.ResetInventory();
            }
            fmvVideos.SetNavigationUiVisibility(showNavigationUiButtons ? 1 : 0);
        }

        private void OnDestroy() {
            if (!alwaysStartFromBeginning) {
                fmvVideos.SaveProgress();
            }
        }
    }
}