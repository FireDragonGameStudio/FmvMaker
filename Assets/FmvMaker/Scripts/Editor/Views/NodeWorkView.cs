using System;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Views {
    [Serializable]
    public class NodeWorkView : ViewBase {
        public NodeWorkView() : base("Work View") {

        }
        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e) {
            base.UpdateView(editorRect, percentageRect, e);

            GUI.Box(ViewRect, ViewTitle);

            GUILayout.BeginArea(ViewRect);
            EditorGUILayout.LabelField("This is a label.");
            GUILayout.EndArea();

            ProcessEvents(e);
        }

        public override void ProcessEvents(Event e) {
            base.ProcessEvents(e);

            if (ViewRect.Contains(e.mousePosition)) {
                if (e.button == 0) {
                    if (e.type == EventType.MouseDown) {

                    }

                    if (e.type == EventType.MouseDrag) {

                    }

                    if (e.type == EventType.MouseUp) {

                    }
                }

                if (e.button == 1) {

                }
            }
        }
    }
}