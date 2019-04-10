using FmvMaker.Models;
using FmvMaker.Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace FmvMaker.Views {
    [RequireComponent(typeof(Button))]
    public class NavigationTargetClick : MonoBehaviour {

        //private VideoPresenter _presenter;
        //private string _videoName;

        //public void SetTargetData(TargetInfo target, VideoPresenter presenter) {
        //    _presenter = presenter;
        //    _videoName = target.VideoName;

        //    GetComponent<RectTransform>().anchoredPosition3D = 
        //        new Vector3(target.RelativeScreenPosition.x, target.RelativeScreenPosition.y, 0);

        //    GetComponent<Button>().onClick.AddListener(OnClick_NavigationTarget);
        //    GetComponentInChildren<Text>().text = target.DisplayedText;
        //}

        //private void OnClick_NavigationTarget() {
        //    _presenter.PlayVideoByName(_videoName);
        //}
    }
}