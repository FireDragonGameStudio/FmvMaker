using FmvMaker.Core.Models;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [Serializable]
    public class FmvGraphElementData : IGraphElementData {
        public string VideoName { get; private set; }
        public bool IsLooping { get; private set; }
        public Vector2 RelativeScreenPosition { get; private set; } = new Vector2(0.5f, 0.5f);

        public FmvGraphElementData(string videoName, bool isLooping, Vector2 relativeScreenPosition) {
            VideoName = videoName;
            IsLooping = isLooping;
            RelativeScreenPosition = relativeScreenPosition;
        }

        public VideoModel GetVideoModel() {
            return new VideoModel() {
                Name = VideoName,
                IsLooping = IsLooping,
                RelativeScreenPosition = RelativeScreenPosition,
            };
        }
    }
}