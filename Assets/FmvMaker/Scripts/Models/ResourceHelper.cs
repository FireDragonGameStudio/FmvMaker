using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Models {
    public static class ResourceInfo {

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