using FmvMaker.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Utils {
    public static class LoadFmvData {

        /// <summary>
        /// Generate video mock data to test functionalities
        /// </summary>
        /// <returns></returns>
        public static List<VideoElement> GenerateVideoMockData() {
            // scan files for VideoInfo json files
            return new List<VideoElement>() {
                new VideoElement() {
                    Name = "First",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Second Title",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.3f, 0.5f),
                            NextVideo = "Second"
                        },
                        new NavigationModel() {
                            DisplayText = "Third",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.1f, 0.1f),
                            NextVideo = ""
                        }
                    }
                }, new VideoElement() {
                    Name = "Second",
                    NavigationTargets = new NavigationModel[] { }
                }
            };
        }

        /// <summary>
        /// Gets the relative screen position for x, y values between 0 and 1. Origin (0, 0) is the left lower corner.
        /// </summary>
        /// <param name="x">Horizontal screen position</param>
        /// <param name="y">Vertical sceen position</param>
        /// <returns></returns>
        private static Vector2 GetRelativeScreenPosition(float x, float y) {
            return new Vector2(Screen.width * x, Screen.height * y);
        }
    }
}