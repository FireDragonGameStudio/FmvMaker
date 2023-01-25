using System;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Views {
    [Serializable]
    public class NodePropertyView : ViewBase {
        public NodePropertyView() : base("Property View") {

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

            }
        }
    }
}