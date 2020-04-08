using FmvMaker.Models;
using System;
using UnityEngine.Events;

namespace FmvMaker.Core.Utilities {
    [Serializable]
    public class ItemClickEvent : UnityEvent<ItemModel> {
    }
}