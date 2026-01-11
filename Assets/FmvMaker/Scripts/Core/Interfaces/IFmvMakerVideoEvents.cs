using UnityEngine.Video;

namespace FmvMaker.Interfaces {
    public interface IFmvMakerVideoEvents {
        void OnVideoStarted(VideoClip videoClip);
        void OnVideoPaused(VideoClip videoClip, bool isPaused);
        void OnVideoSkipped(VideoClip videoClip);
        void OnVideoFinished(VideoClip videoClip);
    }
}