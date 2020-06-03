﻿using FmvMaker.Core.Models;
using System;
using UnityEngine.Events;

namespace FmvMaker.Core.Utilities {
    [Serializable]
    public class FmvMakerVideoEvent : UnityEvent<VideoModel> {
    }
}