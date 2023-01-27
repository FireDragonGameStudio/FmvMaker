using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Data.Nodes {
    [Serializable]
    public class FloatNode : NodeBase {

        public float NodeValue;
        public NodeOutput Output;

        public FloatNode() {
            Output = new NodeOutput();
        }

        public override void InitNode() {
            base.InitNode();
            Type = NodeType.Float;
            NodeRect = new Rect(10f, 10f, 150f, 65f);
        }

        public override void UpdateNode(Event e, Rect viewRect) {
            base.UpdateNode(e, viewRect);
        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

            if (GUI.Button(new Rect(NodeRect.x + NodeRect.width, NodeRect.y + NodeRect.height / 2 - 12f, 24f, 24f), "")) {
                if (ParentGraph != null) {
                    ParentGraph.WantsConnection = true;
                    ParentGraph.ConnectionNode = this;
                }
            }
        }

        public override void DrawNodeProperties() {
            base.DrawNodeProperties();

            NodeValue = EditorGUILayout.FloatField("Float Value:", NodeValue);
        }
    }
#endif
}
