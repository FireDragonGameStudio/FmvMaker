using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FadeMe : MonoBehaviour {

    [SerializeField]
    private VideoPlayer _mainPlayer;
    [SerializeField]
    private VideoPlayer _backgroundPlayer;

    void Start() {
        _mainPlayer.loopPointReached += ShowBackground;
        _mainPlayer.started += DisableBackgroundVideo;
        _backgroundPlayer.started += EnableBackground;
    }

    private void EnableBackground(VideoPlayer source) {
        SetPlayerVisible(false);
    }

    private void DisableBackgroundVideo(VideoPlayer source) {
        _backgroundPlayer.Prepare();
        SetPlayerVisible();
    }

    void OnDestroy() {
        _mainPlayer.loopPointReached -= ShowBackground;
        _mainPlayer.started -= DisableBackgroundVideo;
        _backgroundPlayer.started -= EnableBackground;
    }

    private void ShowBackground(VideoPlayer source) {
        _backgroundPlayer.Play();
    }

    private void SetPlayerVisible(bool isMainVisible = true) {
        if (isMainVisible) {
            _mainPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
            _backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
        } else {
            _mainPlayer.transform.localPosition = new Vector3(0, 0, 0.1f);
            _backgroundPlayer.transform.localPosition = new Vector3(0, 0, 0.01f);
        }
    }
}
