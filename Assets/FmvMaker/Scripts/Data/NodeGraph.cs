using Assets.FmvMaker.Scripts.Data.Nodes;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Data {
    [Serializable]
    public class NodeGraph : ScriptableObject {

        public List<NodeBase> nodes;
        public string Name;
        public NodeBase SelectedNode;

        public bool WantsConnection = false;
        public NodeBase ConnectionNode;

        public bool ShowNodeProperties = false;

        private void OnEnable() {
            if (nodes == null) {
                nodes = new List<NodeBase>();
            }
        }

        public void InitGraph() {
            if (nodes.Count > 0) {
                for (int i = 0; i < nodes.Count; i++) {
                    nodes[i].InitNode();
                }
            }
        }

        public void UpdateGraph() {
            if (nodes.Count > 0) {

            }
        }

        private void ProcessEvents(Event e, Rect viewRect) {
            if (viewRect.Contains(e.mousePosition)) {
                if (e.button == 0) {
                    if (e.type == EventType.MouseDown) {
                        DeselectAllNodes();

                        for (int i = 0; i < nodes.Count; i++) {
                            if (nodes[i].NodeRect.Contains(e.mousePosition)) {
                                nodes[i].IsSelected = true;
                                SelectedNode = nodes[i];
                                break;
                            }
                        }

                        if (WantsConnection) {
                            WantsConnection = false;
                        }
                    }
                }
            }
        }

        private void DeselectAllNodes() {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].IsSelected = false;
            }
            SelectedNode = null;
            ShowNodeProperties = false;
        }

        private void DrawConnectionToMouse(Vector2 mousePosition) {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Handles.DrawLine(
                new Vector3(ConnectionNode.NodeRect.x + ConnectionNode.NodeRect.width + 24f,
                    ConnectionNode.NodeRect.y + ConnectionNode.NodeRect.height * 0.5f,
                    0f),
                new Vector3(mousePosition.x, mousePosition.y, 0f));

            Handles.EndGUI();
        }

#if UNITY_EDITOR
        public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            if (nodes.Count > 0) {
                ProcessEvents(e, viewRect);
                for (int i = 0; i < nodes.Count; i++) {
                    nodes[i].UpdateNodeGUI(e, viewRect, viewSkin);
                }
            }

            if (WantsConnection) {
                if (ConnectionNode != null) {
                    DrawConnectionToMouse(e.mousePosition);
                }
            }

            if (e.type == EventType.Layout) {
                if (SelectedNode != null) {
                    ShowNodeProperties = true;
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}