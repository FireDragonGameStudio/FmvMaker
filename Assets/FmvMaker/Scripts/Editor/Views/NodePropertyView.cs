using Assets.FmvMaker.Scripts.Data;
using System;
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