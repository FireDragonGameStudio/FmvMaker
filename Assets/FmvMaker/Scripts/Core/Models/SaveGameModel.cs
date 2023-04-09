using FmvMaker.Graph;
using System;
using UnityEngine;

namespace FmvMaker.Core.Models {
    [Serializable]
    public class SaveGameModel {
        public string Id;
        public FmvVideoEnum VideoTarget;
        public FmvVideoEnum UsageTarget;
        public bool IsLooping;
        public bool AlreadyWatched;
        public bool IsItem;
        public bool IsInInventory;
        public bool WasUsed;
        public Vector2 RelativeScreenPosition = new Vector2(0.5f, 0.5f);

        public SaveGameModel(VideoModel videoModel) {
            this.Id = videoModel.Name;
            this.VideoTarget = (FmvVideoEnum)Enum.Parse(typeof(FmvVideoEnum), videoModel.VideoTarget);
            this.IsLooping = videoModel.IsLooping;
            this.AlreadyWatched = videoModel.AlreadyWatched;
            this.RelativeScreenPosition = videoModel.RelativeScreenPosition;
        }

        public SaveGameModel(ClickableModel clickableModel) {
            this.Id = clickableModel.Name;
            this.VideoTarget = (FmvVideoEnum)Enum.Parse(typeof(FmvVideoEnum), clickableModel.PickUpVideo);
            this.UsageTarget = (FmvVideoEnum)Enum.Parse(typeof(FmvVideoEnum), clickableModel.UsageVideo);
            this.IsItem = true;
            this.IsInInventory = clickableModel.IsInInventory;
            this.WasUsed = clickableModel.WasUsed;
            this.RelativeScreenPosition = clickableModel.RelativeScreenPosition;
        }

        public SaveGameModel(FmvGraphElementData graphElementData) {
            this.Id = graphElementData.Id;
            this.VideoTarget = graphElementData.VideoTarget;
            this.UsageTarget = graphElementData.UsageTarget;
            this.IsLooping = graphElementData.IsLooping;
            this.AlreadyWatched = graphElementData.AlreadyWatched;
            this.IsItem = graphElementData.IsItem;
            this.IsInInventory = graphElementData.IsInInventory;
            this.WasUsed = graphElementData.WasUsed;
            this.RelativeScreenPosition = graphElementData.RelativeScreenPosition;
        }
    }
}