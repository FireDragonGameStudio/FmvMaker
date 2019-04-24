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
                    Name = "Idle",
                    IsLooping = false,
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Up Dir",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.2f, 0.5f),
                            NextVideo = "Up"
                        }, new NavigationModel() {
                            DisplayText = "Left Dir",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.2f, 0.5f),
                            NextVideo = "Left"
                        }, new NavigationModel() {
                            DisplayText = "Right Dir",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.8f, 0.5f),
                            NextVideo = "Right"
                        }, new NavigationModel() {
                            DisplayText = "Down Dir",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.5f, 0.2f),
                            NextVideo = "Down"
                        }
                    }
                }, new VideoElement() {
                    Name = "Left",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = GetRelativeScreenPosition(Vector2.zero),
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoElement() {
                    Name = "Right",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = GetRelativeScreenPosition(Vector2.zero),
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoElement() {
                    Name = "Down",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Down Left",
                            RelativeScreenPosition = GetRelativeScreenPosition(0.2f, 0.5f),
                            NextVideo = "DownLeft"
                        }
                    }
                }, new VideoElement() {
                    Name = "DownLeft",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = GetRelativeScreenPosition(Vector2.zero),
                            NextVideo = "DownReturn"
                        }
                    }
                }, new VideoElement() {
                    Name = "DownReturn",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = GetRelativeScreenPosition(Vector2.zero),
                            NextVideo = "Idle"
                        }
                    }
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

        private static Vector2 GetRelativeScreenPosition(Vector2 vector2) {
            return GetRelativeScreenPosition(vector2.x, vector2.y);
        }
    }
}