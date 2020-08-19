using FmvMaker.Core.Models;
using System.IO;
using UnityEngine;

namespace FmvMaker.Core.Utilities {
    public static class FmvData {

        /// <summary>
        /// Generate video mock data to test functionalities
        /// </summary>
        /// <returns></returns>
        public static VideoModel[] GenerateVideoMockData() {
            // scan files for VideoInfo json files
            return new VideoModel[] {
                new VideoModel() {
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
                }, new VideoModel() {
                    Name = "Left",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoModel() {
                    Name = "Right",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoModel() {
                    Name = "Down",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "Down Left",
                            RelativeScreenPosition = new Vector2(0.2f, 0.5f),
                            NextVideo = "DownLeft"
                        }
                    }
                }, new VideoModel() {
                    Name = "DownLeft",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "DownReturn"
                        }
                    }
                }, new VideoModel() {
                    Name = "DownReturn",
                    NavigationTargets = new NavigationModel[] {
                        new NavigationModel() {
                            DisplayText = "",
                            RelativeScreenPosition = Vector2.zero,
                            NextVideo = "Idle"
                        }
                    }
                }, new VideoModel() {
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
        public static ItemModel[] GenerateItemMockData() {
            // scan files for VideoInfo json files
            return new ItemModel[] {
                new ItemModel() {
                    Name = "apple",
                    DisplayText = "Köstlicher Apfel",
                    Description = "Hilft leichten Hunger zu bekämpfen.",
                    RelativeScreenPosition = new Vector2(0.2f, 0.8f)
                }, new ItemModel() {
                    Name = "bag",
                    DisplayText = "Tasche",
                    Description = "Darin kann etwas verstaut werden.",
                    RelativeScreenPosition = new Vector2(0.2f, 0.6f)
                }, new ItemModel() {
                    Name = "coins",
                    DisplayText = "alte Münzen",
                    Description = "Um etwas zu kaufen.",
                    RelativeScreenPosition = new Vector2(0.2f, 0.4f)
                }, new ItemModel() {
                    Name = "gem",
                    DisplayText = "Saphir",
                    Description = "Ein funkelnder Edelstein.",
                    RelativeScreenPosition = new Vector2(0.2f, 0.2f)
                }, new ItemModel() {
                    Name = "Meat",
                    DisplayText = "Fleisch",
                    Description = "Gegen riesigen Hunger.",
                    RelativeScreenPosition = new Vector2(0.4f, 0.2f)
                }, new ItemModel() {
                    Name = "mp",
                    DisplayText = "Wasserflasche",
                    Description = "Ein erfrischender Durstlöscher.",
                    RelativeScreenPosition = new Vector2(0.6f, 0.2f)
                }, new ItemModel() {
                    Name = "pants",
                    DisplayText = "Stylische Hose",
                    Description = "Eine getragene Hose.",
                    RelativeScreenPosition = new Vector2(0.8f, 0.2f)
                }
            };
        }

        public static VideoModel[] GenerateVideoDataFromLocalFile() {
            return JsonUtility.FromJson<VideoModelWrapper>(ResourceVideoInfo.VideoModelData.text).VideoList;
        }

        public static ItemModel[] GenerateItemDataFromLocalFile() {
            return JsonUtility.FromJson<ItemModelWrapper>(ResourceVideoInfo.ItemModelData.text).ItemList;
        }

        public static NavigationModel[] GenerateNavigationDataFromLocalFile() {
            return JsonUtility.FromJson<NavigationModelWrapper>(ResourceVideoInfo.NavigationModelData.text).NavigationList;
        }

        public static void ExportVideoDataToLocalFile(VideoModel[] videoElements, string localFilePath) {
            using (StreamWriter sw = new StreamWriter(Path.Combine(localFilePath, "FmvMakerDemoVideoData"))) {
                sw.Write(JsonUtility.ToJson(videoElements));
            }
        }

        public static Vector2 GetRelativeScreenPosition(float x, float y) {
            return new Vector2(Screen.width * x, Screen.height * y);
        }

        public static Vector2 GetRelativeScreenPosition(Vector2 vector2) {
            return GetRelativeScreenPosition(vector2.x, vector2.y);
        }
    }
}