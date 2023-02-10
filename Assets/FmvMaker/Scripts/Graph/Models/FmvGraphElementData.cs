using FmvMaker.Core.Models;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [Serializable]
    public class FmvGraphElementData : IGraphElementData {
        public FmvVideoEnum VideoTarget { get; private set; }
        public FmvVideoEnum UsageTarget { get; private set; }
        public bool IsItem { get; set; } = false;
        public bool IsLooping { get; private set; } = false;
        public bool IsInInventory { get; set; } = false;
        public bool WasUsed { get; set; } = false;
        public Vector2 RelativeScreenPosition { get; private set; } = new Vector2(0.5f, 0.5f);

        public FmvGraphElementData(FmvVideoEnum targetVideo, bool isLooping, Vector2 relativeScreenPosition) {
            VideoTarget = targetVideo;
            IsLooping = isLooping;
            RelativeScreenPosition = relativeScreenPosition;
        }

        public FmvGraphElementData(FmvVideoEnum videoTarget, FmvVideoEnum usageTarget, bool isInInventory, bool wasUsed, Vector2 relativeScreenPosition) {
            VideoTarget = videoTarget;
            UsageTarget = usageTarget;
            IsItem = true;
            IsInInventory = isInInventory;
            WasUsed = wasUsed;
            RelativeScreenPosition = relativeScreenPosition;
        }

        public VideoModel GetVideoModel() {
            return new VideoModel() {
                Name = VideoTarget.ToString(),
                IsLooping = IsLooping,
                RelativeScreenPosition = RelativeScreenPosition,
            };
        }

        public ClickableModel GetItemModel() {
            return new ClickableModel() {
                Name = VideoTarget.ToString(),
                PickUpVideo = VideoTarget.ToString(),
                UseageVideo = UsageTarget.ToString(),
                IsInInventory = IsInInventory,
                WasUsed = WasUsed,
                RelativeScreenPosition = RelativeScreenPosition,
            };
        }
    }
}