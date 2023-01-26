using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Data.Nodes {
    [Serializable]
    public class NodeBase : ScriptableObject {

        public string NodeName;
        public Rect NodeRect;
        public NodeGraph ParentGraph;
        public NodeType Type;

        protected GUISkin nodeSkin;

        public virtual void InitNode() {

        }

        public virtual void UpdateNode(Event e, Rect viewRect) {
            ProcessEvents(e, viewRect);
        }

        private void ProcessEvents(Event e, Rect viewRect) {
            if (NodeRect.Contains(e.mousePosition)) {
                if (e.type == EventType.MouseDrag) {
                    if (viewRect.Contains(e.mousePosition)) {
                        NodeRect.x += e.delta.x;
                        NodeRect.y += e.delta.y;
                    }
                }
            }
        }

#if UNITY_EDITOR
        public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            ProcessEvents(e, viewRect);

            GUI.Box(NodeRect, NodeName, viewSkin.GetStyle("NodeDefaultStyle"));

            EditorUtility.SetDirty(this);
        }
#endif
    }
}