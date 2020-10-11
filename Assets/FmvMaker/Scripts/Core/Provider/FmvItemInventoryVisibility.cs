using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvItemInventoryVisibility : MonoBehaviour {

        private const KeyCode InventoryToogle = KeyCode.I;
        private const int InventoryVisible = 50;
        private const int InventoryInvisible = -50;

        [SerializeField]
        private RectTransform inventoryElementsPanel = null;

        private bool inventoryToggle;

        private void Update() {
            ToggleInventory();
        }

        private void ToggleInventory() {
            if (Input.GetKeyUp(InventoryToogle)) {
                float yPos = inventoryToggle ? InventoryVisible : InventoryInvisible;
                inventoryElementsPanel.position = new Vector3(
                    inventoryElementsPanel.position.x, yPos, inventoryElementsPanel.position.z);
                inventoryToggle = !inventoryToggle;
            }
        }
    }
}