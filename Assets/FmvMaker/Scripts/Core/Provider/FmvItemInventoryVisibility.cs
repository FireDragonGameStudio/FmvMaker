using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvItemInventoryVisibility : MonoBehaviour {

        private const int InventoryVisible = 75;
        private const int InventoryInvisible = -75;

        [Header("Key Bindings")]
        [SerializeField]
        private KeyCode InventoryToogle = KeyCode.I;
        [Header("Internal references")]
        [SerializeField]
        private RectTransform inventoryElementsPanel = null;

        private bool inventoryToggle;
        private bool externalInventoryToggle;

        private void Start() {
            ToggleInventoryVisibility();
        }

        private void Update() {
            ToggleInventory();
        }

        private void ToggleInventory() {
            if (Input.GetKeyUp(InventoryToogle) || externalInventoryToggle) {
                externalInventoryToggle = false;
                InventoryVisibility();
            }
        }

        private void InventoryVisibility() {
            float yPos = inventoryToggle ? InventoryVisible : InventoryInvisible;
            inventoryElementsPanel.position = new Vector3(
                inventoryElementsPanel.position.x, yPos, inventoryElementsPanel.position.z);
            inventoryToggle = !inventoryToggle;
        }

        public void ToggleInventoryVisibility() {
            externalInventoryToggle = true;
        }
    }
}