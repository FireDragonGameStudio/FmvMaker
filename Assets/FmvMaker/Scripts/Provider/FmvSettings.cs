using FmvMaker.Core.Facades;
using UnityEngine;

namespace FmvMaker.Provider {
    public class FmvSettings : MonoBehaviour {
        [Header("Debug Settings")]
        [SerializeField] private bool resetInventoryOnStart;
        [SerializeField] private bool alwaysStartFromBeginning;
        [Tooltip("Without a Button Style: shows or hides the choice buttons. With a Button Style: draws invisible hotspots in pink, so you can see where they are while testing.")]
        [SerializeField] private bool showNavigationUiButtons;
        [Tooltip("Without a Button Style: shows or hides the choice labels. With a Button Style this is ignored; the style decides.")]
        [SerializeField] private bool showNavigationLabelTexts;

        [Header("Internal References")]
        [SerializeField] private FmvVideos fmvVideos;
        [SerializeField] private FmvInventory inventory;

        private void Awake() {
            if (resetInventoryOnStart) {
                inventory.ResetInventory();
            }
            fmvVideos.SetNavigationUiVisibility(showNavigationUiButtons ? 1 : 0);
            fmvVideos.SetNavigationUiLabelVisibility(showNavigationLabelTexts ? 1 : 0);
        }

        private void OnDestroy() {
            if (!alwaysStartFromBeginning) {
                fmvVideos.SaveProgress();
            }
        }
    }
}