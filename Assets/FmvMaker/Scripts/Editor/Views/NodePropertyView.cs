using Assets.FmvMaker.Scripts.Data;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Views {
    [Serializable]
    public class NodePropertyView : ViewBase {

        public NodePropertyView(GUISkin viewSkin) : base("Property View") {
            this.viewSkin = viewSkin;
        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentNodeGraph) {
            base.UpdateView(editorRect, percentageRect, e, currentNodeGraph);

            GUI.Box(ViewRect, ViewTitle, viewSkin.GetStyle("GraphViewStyle"));

            GUILayout.BeginArea(ViewRect);
            GUILayout.Space(60);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if (currentNodeGraph != null && currentNodeGraph.SelectedNode != null && currentNodeGraph.ShowNodeProperties) {
                currentNodeGraph.SelectedNode.DrawNodeProperties();
            } else {
                EditorGUILayout.LabelField("NONE");
            }
            GUILayout.Space(30);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e) {
            base.ProcessEvents(e);

            if (ViewRect.Contains(e.mousePosition)) {

            }
        }
    }
}