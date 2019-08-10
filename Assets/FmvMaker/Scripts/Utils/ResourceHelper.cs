using System;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Utils {
    public static class ResourceInfo {

        public static VideoClip LoadVideoClipFromResources(string name) {
            return Resources.Load<VideoClip>($"Videos/{name}");
        }

        public static string LoadVideoClipFromFile(string name) {
            return new Uri($"{LoadFmvConfig.Config.LocalVideoPath}{name}.mp4").AbsoluteUri;
        }

        public static string LoadVideoClipFromOnlineSource(string name) {
            return new Uri($"{LoadFmvConfig.Config.OnlineVideoURL}{name}").AbsoluteUri;
        }
    }
}