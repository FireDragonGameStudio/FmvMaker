using System;
using UnityEngine;

namespace FmvMaker.Core.Models {
    [Serializable]
    public abstract class BaseUiElementModel {
        public string DisplayText = "";
        public Vector2 RelativeScreenPosition = new Vector2(0.5f, 0.5f);
    }
}