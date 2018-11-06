using FmvMaker.Models;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Tools {
    public static class ResourceInfo {

        private static FmvMakerConfig _config => LoadFmvConfig.LoadConfig();

        public static VideoClip LoadVideoClipFromResources(string name) {
            return Resources.Load<VideoClip>($"Videos/{name}");
        }

        public static Texture2D LoadStaticBackgroundFromResources(string name) {
            return Resources.Load<Texture2D>($"Backgrounds/Static/{name}");
        }

        public static VideoClip LoadDynamicBackgroundFromResources(string name) {
            return Resources.Load<VideoClip>($"Backgrounds/Dynamic/{name}");
        }
    }
}