using FmvMaker.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Tools {
    public static class LoadFmvData {

        /// <summary>
        /// Generate video mock data to test functionalities
        /// </summary>
        /// <returns></returns>
        public static List<VideoInfo> GenerateVideoMockData() {
            // scan files for VideoInfo json files
            return new List<VideoInfo>() {
                new VideoInfo() {
                    Name = "Entrance",
                    Background = new BackgroundInfo() {
                        Name = "HallwayBackground",
                        Type = BackgroundType.Static
                    },
                    Targets = new TargetInfo[] {
                        new TargetInfo() {
                            DisplayedText = "Nav to Toilet",
                            VideoName = "Toilet",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.3f, 0.5f)
                        },
                        new TargetInfo() {
                            DisplayedText = "Nav To Bathroom",
                            VideoName = "Bathroom",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.5f, 0.4f)
                        }
                    }
                }, new VideoInfo() {
                    Name = "Toilet",
                    Background = new BackgroundInfo() {
                        Name = "ToiletBackground",
                        Type = BackgroundType.Dynamic
                    },
                    Targets = new TargetInfo[] { }
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