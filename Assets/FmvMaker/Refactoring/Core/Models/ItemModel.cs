using System;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class ItemModel : BaseUiElementModel {
        public string Name;
        public string Description;
        public NavigationModel NavigationTarget;
        public bool IsInInventory;
        public bool WasUsed;
    }
}