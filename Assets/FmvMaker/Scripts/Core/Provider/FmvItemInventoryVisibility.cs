using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvItemInventoryVisibility : MonoBehaviour {

        private const int InventoryVisible = 50;
        private const int InventoryInvisible = -50;

        [Header("Key Bindings")]
        [SerializeField]
        private KeyCode InventoryToogle = KeyCode.I;
        [Header("Internal references")]
        [SerializeField]
        private RectTransform inventoryElementsPanel = null;

        private bool inventoryToggle;

        private void Start() {
            ToggleInventoryVisibility();
        }

        private void Update() {
            ToggleInventory();
        }

        private void ToggleInventory() {
            if (Input.GetKeyUp(InventoryToogle)) {
                ToggleInventoryVisibility();
            }
        }

        private void ToggleInventoryVisibility() {
            float yPos = inventoryToggle ? InventoryVisible : InventoryInvisible;
            inventoryElementsPanel.position = new Vector3(
                inventoryElementsPanel.position.x, yPos, inventoryElementsPanel.position.z);
            inventoryToggle = !inventoryToggle;
        }
    }
}