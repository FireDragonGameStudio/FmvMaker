using System;

namespace FmvMaker.Models {
    [Serializable]
    public class ItemElement : BaseUiElementModel {
        public string Name;
        public string Description;
        public NavigationModel NavigationTarget;
        public bool IsInInventory;
        public bool WasUsed;
    }
}