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
        public bool IsSelected = false;

        protected GUISkin nodeSkin;

        [Serializable]
        public class NodeInput {
            public bool IsOccupied = false;
            public NodeBase InputNode;
        }

        [Serializable]
        public class NodeOutput {
            public bool IsOccupied = false;
        }

        public virtual void InitNode() {

        }

        public virtual void UpdateNode(Event e, Rect viewRect) {
            ProcessEvents(e, viewRect);
        }

        private void ProcessEvents(Event e, Rect viewRect) {
            if (IsSelected) {
                if (NodeRect.Contains(e.mousePosition)) {
                    if (e.type == EventType.MouseDrag) {
                        NodeRect.x += e.delta.x;
                        NodeRect.y += e.delta.y;
                    }
                }
            }
        }

#if UNITY_EDITOR
        public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            ProcessEvents(e, viewRect);

            if (!IsSelected) {
                GUI.Box(NodeRect, NodeName, viewSkin.GetStyle("NodeDefaultStyle"));
            } else {
                GUI.Box(NodeRect, NodeName, viewSkin.GetStyle("NodeSelectedStyle"));
            }

            EditorUtility.SetDirty(this);
        }

        public virtual void DrawNodeProperties() {

        }
#endif
    }
}