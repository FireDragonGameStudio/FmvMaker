using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Utils {
    public static class ResourceInfo {

        public static VideoClip LoadVideoClipFromResources(string name) {
            return Resources.Load<VideoClip>($"Videos/{name}");
        }

        public static string LoadVideoClipFromFile(string name) {
            return new Uri($"{LoadFmvConfig.Config.LocalFilePath}\\Videos\\{name}.mp4").AbsoluteUri;
        }

        public static string LoadVideoClipFromOnlineSource(string name) {
            return new Uri($"{LoadFmvConfig.Config.OnlineVideoURL}{name}").AbsoluteUri;
        }

        public static string LoadItemImageFromFile(string name) {
            return "file:///" + Path.Combine(LoadFmvConfig.Config.LocalFilePath, "Textures", name + ".png");
        }

        public static string LoadItemImageFromOnlineSource(string name) {
            return new Uri($"{LoadFmvConfig.Config.OnlineImageURL}{name}").AbsoluteUri;
        }
    }
}