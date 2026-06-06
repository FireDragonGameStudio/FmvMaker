using FmvMaker.Core.Interfaces;
using System.Collections;
using UnityEngine;

namespace FmvMaker.Utilities {
    public class TimedInteractionCountdown : MonoBehaviour, ICountdownEvents {

        private float countdownDuration;

        public void InitCountdown(float countdownTime) {
            this.countdownDuration = countdownTime;
            this.gameObject.SetActive(true);
        }

        public void ResetCountdown() {
            this.countdownDuration = 0f;
            this.gameObject.SetActive(false);
        }

        private void OnEnable() {
            StartCoroutine(RunCountdown());
        }

        private void OnDisable() {
            StopAllCoroutines();
            ResetCountdown();
        }

        private IEnumerator RunCountdown() {
            float elapsed = 0f;

            while (elapsed < countdownDuration) {
                elapsed += Time.deltaTime;

                // calculate progress as a value between 0 and 1
                float progress = elapsed / countdownDuration;
                float scaleX = Mathf.Lerp(1f, 0f, progress);

                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

                yield return null;
            }

            // ensure it hits exactly 0 at the end
            transform.localScale = new Vector3(0f, transform.localScale.y, transform.localScale.z);
        }
    }
}