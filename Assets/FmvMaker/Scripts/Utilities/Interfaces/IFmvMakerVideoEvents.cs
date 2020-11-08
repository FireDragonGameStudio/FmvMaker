using FmvMaker.Core.Models;

namespace FmvMaker.Utilities.Interfaces {
    public interface IFmvMakerVideoEvents {
        void OnVideoStarted(VideoModel videoModel);
        void OnVideoPaused(VideoModel videoModel, bool isPaused);
        void OnVideoSkipped(VideoModel videoModel);
        void OnVideoFinished(VideoModel videoModel);
    }
}