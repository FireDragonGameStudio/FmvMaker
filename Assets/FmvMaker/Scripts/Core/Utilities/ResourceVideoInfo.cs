﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace FmvMaker.Core.Utilities {
    public static class ResourceVideoInfo {

        public static VideoClip LoadVideoClipFromResources(string name) {
            return Resources.Load<VideoClip>($"FmvMakerVideos/{name}");
        }

        public static string LoadVideoClipFromFile(string name) {
            return new Uri($@"{LoadFmvConfig.Config.LocalFilePath}\FmvMakerVideos\{name}.mp4").AbsoluteUri;
        }

        public static string LoadVideoClipFromOnlineSource(string name) {
            return new Uri($"{LoadFmvConfig.Config.OnlineVideoURL}{name}").AbsoluteUri;
        }

        //public static string LoadItemImageFromFile(string name) {
        //    return "file:///" + Path.Combine(LoadFmvConfig.Config.LocalFilePath, "Textures", name + ".png");
        //}

        //public static string LoadItemImageFromOnlineSource(string name) {
        //    return new Uri($"{LoadFmvConfig.Config.OnlineImageURL}{name}").AbsoluteUri;
        //}
    }
}