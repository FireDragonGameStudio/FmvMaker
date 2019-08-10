using FmvMaker.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FmvMaker.Utils {
    public static class FmvData {

        /// <summary>
        /// Generate video mock data to test functionalities
        /// </summary>
        /// <returns></returns>
        public static List<VideoElement> GenerateVideoMockData() {
            // scan files for VideoInfo json files
            return new List<VideoElement>() {
                new VideoElement() {
                    Name = "Idle",
                    IsLooping = true,
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Up Dir",
                            RelativeScreenPosition = new Vector2(0.5f, 0.8f),
                            NextVideo = "Up"
                        }, new NavigationModel() {
                            DisplayText = "Left Dir",
                            RelativeScreenPosition = new Vector2(0.2f, 0.5f),
                            NextVideo = "Left"
                        }, new NavigationModel() {
                            DisplayText = "Right Dir",
                            RelativeScreenPosition = new Vector2(0.8f, 0.5f),
                            NextVideo = "Right"
                        }, new NavigationModel() {
                            DisplayText = "Down Dir",
                            RelativeScreenPosition = new Vector2(0.5f, 0.2f),
                            NextVideo = "Down"
                        }
                    }
                }, new VideoElement() {
                    Name = "Left",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoElement() {
                    Name = "Right",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoElement() {
                    Name = "Down",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Down Left",
                            RelativeScreenPosition = new Vector2(0.2f, 0.5f),
                            NextVideo = "DownLeft"
                        }
                    }
                }, new VideoElement() {
                    Name = "DownLeft",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "DownReturn"
                        }
                    }
                }, new VideoElement() {
                    Name = "DownReturn",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoElement() {
                    Name = "Up",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Generate video mock data to test functionalities
        /// </summary>
        /// <returns></returns>
        public static List<ItemElement> GenerateItemMockData() {
            // scan files for VideoInfo json files
            return new List<ItemElement>() {
                new ItemElement() {
                    Name = "Horn",
                    DisplayText = "BeansHorn",
                    Description = "Makes noise",
                    RelativeScreenPosition = Vector2.zero
                }, new ItemElement() {
                    Name = "Screwdriver",
                    DisplayText = "BeanDriver",
                    Description = "Helps to repair certin things",
                    RelativeScreenPosition = Vector2.zero
                }
            };
        }

        public static List<VideoElement> GenerateVideoDataFromLocalFile(string localFilePath) {
            return JsonConvert.DeserializeObject<List<VideoElement>>(File.ReadAllText(Path.Combine(localFilePath, "FmvMakerDemoVideoData.json")));
        }

        public static void ExportVideoDataToLocalFile(List<VideoElement> videoElements, string localFilePath) {
            using (StreamWriter sw = new StreamWriter(Path.Combine(localFilePath, "FmvMakerDemoVideoData.json"))) {
                sw.Write(JsonConvert.SerializeObject(videoElements));
            }
        }

        /// <summary>
        /// Gets the relative screen position for x, y values between 0 and 1. Origin (0, 0) is the left lower corner.
        /// </summary>
        /// <param name="x">Horizontal screen position</param>
        /// <param name="y">Vertical sceen position</param>
        /// <returns></returns>
        public static Vector2 GetRelativeScreenPosition(float x, float y) {
            return new Vector2(Screen.width * x, Screen.height * y);
        }

        public static Vector2 GetRelativeScreenPosition(Vector2 vector2) {
            return GetRelativeScreenPosition(vector2.x, vector2.y);
        }
    }
}