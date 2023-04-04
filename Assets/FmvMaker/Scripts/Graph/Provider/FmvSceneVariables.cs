using FmvMaker.Core.Provider;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    public static class FmvSceneVariables {
        public static GameObject VideoView => GetSceneVariableByName<GameObject>("FmvVideoView");
        public static FmvData FmvData => GetSceneVariableByName<GameObject>("FmvData").GetComponent<FmvData>();
        public static GameObject ClickablePrefab => GetSceneVariableByName<GameObject>("ClickableObjectPrefab");
        public static GameObject VideoElementsPanel => GetSceneVariableByName<GameObject>("VideoElementsPanel");
        public static GameObject InventoryElementsPanel => GetSceneVariableByName<GameObject>("InventoryElementsPanel");

        private static bool CheckSceneVariable(string variableName) {
            return Variables.ActiveScene.IsDefined(variableName);
        }

        private static T GetSceneVariableByName<T>(string variableName) {
            if (CheckSceneVariable(variableName)) {
                return (T)Variables.ActiveScene.Get(variableName);
            }
            return default;
        }
    }
}