using FmvMaker.Core.Models;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [Serializable]
    public class FmvGraphElementData : IGraphElementData {

        public string Id { get; set; }
        public FmvVideoEnum VideoTarget { get; private set; }
        public FmvVideoEnum UsageTarget { get; private set; }
        public bool IsLooping { get; private set; } = false;
        public bool IsItem { get; set; } = false;
        public bool IsInInventory { get; set; } = false;
        public bool WasUsed { get; set; } = false;
        public Vector2 RelativeScreenPosition { get; private set; } = new Vector2(0.5f, 0.5f);

        public FmvGraphElementData(SaveGameModel saveGameData) :
            this(saveGameData.Id, saveGameData.VideoTarget, saveGameData.UsageTarget, saveGameData.IsLooping, saveGameData.IsItem, saveGameData.IsInInventory, saveGameData.WasUsed, saveGameData.RelativeScreenPosition) {
        }

        public FmvGraphElementData(string id, FmvVideoEnum targetVideo, bool isLooping, Vector2 relativeScreenPosition) :
            this(id, targetVideo, FmvVideoEnum.None, isLooping, false, false, false, relativeScreenPosition) {
        }

        public FmvGraphElementData(string id, FmvVideoEnum videoTarget, FmvVideoEnum usageTarget, bool isInInventory, bool wasUsed, Vector2 relativeScreenPosition) :
            this(id, videoTarget, usageTarget, false, true, isInInventory, wasUsed, relativeScreenPosition) {
        }

        public FmvGraphElementData(string id, FmvVideoEnum videoTarget, FmvVideoEnum usageTarget, bool isLooping, bool isItem, bool isInInventory, bool wasUsed, Vector2 relativeScreenPosition) {
            this.Id = id;
            this.VideoTarget = videoTarget;
            this.UsageTarget = usageTarget;
            this.IsLooping = isLooping;
            this.IsItem = isItem;
            this.IsInInventory = isInInventory;
            this.WasUsed = wasUsed;
            this.RelativeScreenPosition = relativeScreenPosition;
        }

        public VideoModel GetVideoModel() {
            return new VideoModel() {
                Name = this.Id,
                VideoTarget = this.VideoTarget.ToString(),
                IsLooping = this.IsLooping,
                RelativeScreenPosition = this.RelativeScreenPosition,
            };
        }

        public ClickableModel GetItemModel() {
            return new ClickableModel() {
                Name = this.Id,
                PickUpVideo = this.VideoTarget.ToString(),
                UsageVideo = this.UsageTarget.ToString(),
                IsInInventory = this.IsInInventory,
                WasUsed = this.WasUsed,
                RelativeScreenPosition = this.RelativeScreenPosition,
            };
        }

        public SaveGameModel GetSaveGameModel() {
            return new SaveGameModel() {
                Id = this.Id,
                VideoTarget = this.VideoTarget,
                UsageTarget = this.UsageTarget,
                IsItem = this.IsItem,
                IsInInventory = this.IsInInventory,
                WasUsed = this.WasUsed,
                RelativeScreenPosition = this.RelativeScreenPosition,
            };
        }
    }
}