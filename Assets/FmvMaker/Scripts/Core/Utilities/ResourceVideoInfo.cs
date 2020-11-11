using FmvMaker.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Utilities {
    public static class ResourceVideoInfo {

        private static Dictionary<string, string> onlineVideoNameMapping = new Dictionary<string, string>();

        public static VideoClip LoadVideoClipFromResources(string name) {
            return Resources.Load<VideoClip>($"FmvMakerVideos/{name}");
        }

        public static string LoadVideoClipFromFile(string name) {
            return new Uri($@"{LoadFmvConfig.Config.LocalFilePath}\FmvMakerVideos\{name}.mp4").AbsoluteUri;
        }

        public static string LoadVideoClipFromOnlineSource(string name) {
            string onlineVideoUrl = getVideoUrlByName(name);
            return new Uri($"{onlineVideoUrl}").AbsoluteUri;
        }

        private static string getVideoUrlByName(string name) {
            return onlineVideoNameMapping[name];
        }

        public static void SetOnlineVideoMappungData(VideoOnlineSource[] videoSources) {
            if (onlineVideoNameMapping.Count <= 0) {
                for (int i = 0; i < videoSources.Length; i++) {
                    onlineVideoNameMapping.Add(videoSources[i].Name, videoSources[i].Link);
                }
            }
        }

        //public static string LoadItemImageFromFile(string name) {
        //    return "file:///" + Path.Combine(LoadFmvConfig.Config.LocalFilePath, "Textures", name + ".png");
        //}

        //public static string LoadItemImageFromOnlineSource(string name) {
        //    return new Uri($"{LoadFmvConfig.Config.OnlineImageURL}{name}").AbsoluteUri;
        //}
    }
}