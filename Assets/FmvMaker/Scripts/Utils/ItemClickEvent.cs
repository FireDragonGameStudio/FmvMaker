using FmvMaker.Models;
using System;
using UnityEngine.Events;

namespace FmvMaker.Utils {
    [Serializable]
    public class ItemClickEvent : UnityEvent<ItemModel> {
    }
}