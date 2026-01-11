using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace FmvMaker.Core {
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    [Serializable]
    public class FmvMakerGraph : Graph {
        public const string AssetExtension = "fmvmkr";

        [MenuItem("Assets/Create/FmvMaker/FmvMaker Graph", false)]
        static void CreateAssetFile() {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<FmvMakerGraph>();
        }
    }
}