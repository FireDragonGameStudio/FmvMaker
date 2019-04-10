using System;
using UnityEngine;

namespace FmvMaker.Models {
    [Serializable]
    public abstract class BaseUiElementModel {
        public string DisplayText;
        public Vector2 RelativeScreenPosition;
    }
}