using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Data.Nodes {
    public class AddNode : NodeBase {

        public float NodeValue;
        public float NodeSum;
        public NodeOutput Output;
        public NodeInput InputA;
        public NodeInput InputB;

        public AddNode() {
            Output = new NodeOutput();
            InputA = new NodeInput();
            InputB = new NodeInput();
        }

        public override void InitNode() {
            base.InitNode();
            Type = NodeType.Add;
            NodeRect = new Rect(10f, 10f, 200f, 65f);
        }

        public override void UpdateNode(Event e, Rect viewRect) {
            base.UpdateNode(e, viewRect);
        }

        private void DrawInputLines() {
            if (InputA.IsOccupied && InputA.InputNode != null) {
                DrawLine(InputA, 1f);
            } else {
                InputA.IsOccupied = false;
            }
            if (InputB.IsOccupied && InputB.InputNode != null) {
                DrawLine(InputB, 3f);
            } else {
                InputB.IsOccupied = false;
            }
        }

        private void DrawLine(NodeInput input, float inputId) {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawLine(new Vector3(input.InputNode.NodeRect.x + input.InputNode.NodeRect.width + 12f,
                input.InputNode.NodeRect.y + input.InputNode.NodeRect.height / 2,
                0f),
                new Vector3(NodeRect.x - 12f,
                NodeRect.y + NodeRect.height * inputId * 0.33f - 12f,
                0f));

            Handles.EndGUI();
        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            base.UpdateNodeGUI(e, viewRect, viewSkin);

            // output
            if (GUI.Button(new Rect(NodeRect.x + NodeRect.width, NodeRect.y + NodeRect.height / 2 - 12f, 24f, 24f), "")) {
                if (ParentGraph != null) {
                    ParentGraph.WantsConnection = true;
                    ParentGraph.ConnectionNode = this;
                }
            }

            // input a
            if (GUI.Button(new Rect(NodeRect.x - 24f, NodeRect.y + NodeRect.height / 2 - 36f, 24f, 24f), "")) {
                if (ParentGraph != null) {
                    InputA.InputNode = ParentGraph.ConnectionNode;
                    InputA.IsOccupied = InputA.InputNode != null ? true : false;

                    ParentGraph.WantsConnection = false;
                    ParentGraph.ConnectionNode = null;
                }
            }

            // input b
            if (GUI.Button(new Rect(NodeRect.x - 24f, NodeRect.y + NodeRect.height / 2 + 12f, 24f, 24f), "")) {
                if (ParentGraph != null) {
                    InputB.InputNode = ParentGraph.ConnectionNode;
                    InputB.IsOccupied = InputB.InputNode != null ? true : false;

                    ParentGraph.WantsConnection = false;
                    ParentGraph.ConnectionNode = null;
                }
            }

            if (InputA.IsOccupied && InputB.IsOccupied) {
                FloatNode nodeA = (FloatNode)InputA.InputNode;
                FloatNode nodeB = (FloatNode)InputB.InputNode;

                if (nodeA != null && nodeB != null) {
                    NodeSum = nodeA.NodeValue + nodeB.NodeValue;
                } else {
                    NodeSum = 0f;
                }
            }

            DrawInputLines();
        }

        public override void DrawNodeProperties() {
            base.DrawNodeProperties();

            EditorGUILayout.FloatField("Sum: ", NodeSum);
        }
    }
#endif
}