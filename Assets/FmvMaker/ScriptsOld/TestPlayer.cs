using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class TestPlayer : MonoBehaviour {
    public VideoClip[] vids;

    public VideoPlayer vp1;
    public VideoPlayer vp2;

    private int ii = 0;

    // Use this for initialization
    private void Start() {
        vp1.prepareCompleted += StartVp1;
        vp1.started += DelVp2;
        vp2.prepareCompleted += StartVp2;
        vp2.started += DelVp1;
    }

    private async void DelVp1(VideoPlayer source) {
        await Task.Delay(TimeSpan.FromSeconds(0.1));
        vp1.clip = null;
    }

    private async void DelVp2(VideoPlayer source) {
        await Task.Delay(TimeSpan.FromSeconds(0.1));
        vp2.clip = null;
    }

    private void StartVp2(VideoPlayer source) {
        vp2.Play();
    }

    private void StartVp1(VideoPlayer source) {
        vp1.Play();
    }

    // Update is called once per frame
    private void Update() {

        if (Input.GetMouseButtonDown(0)) {
            ++ii;

            if (ii < 0 || ii >= vids.Length) {
                ii = 0;
            }

            if (ii % 2 == 0) {
                vp2.clip = vids[ii];
                vp2.time = 0f;
                vp2.Prepare();
            } else {
                vp1.clip = vids[ii];
                vp1.time = 0f;
                vp1.Prepare();
            }
        }
    }
}
