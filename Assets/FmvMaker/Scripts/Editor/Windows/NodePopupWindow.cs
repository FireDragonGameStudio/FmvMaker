using Assets.FmvMaker.Scripts.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Windows {
    public class NodePopupWindow : EditorWindow {
        static NodePopupWindow currentPopup;
        string graphNameString = "Enter a name:";

        public static void InitNodePopup() {
            currentPopup = EditorWindow.GetWindow<NodePopupWindow>();
            currentPopup.titleContent = new GUIContent("Node Popup");
        }

        private void OnGUI() {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginVertical();

            EditorGUILayout.LabelField("Create new graph", EditorStyles.boldLabel);
            graphNameString = EditorGUILayout.TextField("Enter Name: ", graphNameString);

            GUILayout.Space(10);

            // buttons for graph creation
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create graph", GUILayout.Height(40))) {
                if (!string.IsNullOrEmpty(graphNameString) && graphNameString != "Enter a name:") {
                    NodeUtils.CreateNewGraph(graphNameString);
                    currentPopup.Close();
                } else {
                    EditorUtility.DisplayDialog("Graph Name Message", "Please enter a valid graph name!", "OK");
                }
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(40))) {
                currentPopup.Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
    }
}