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
        public string Name { get; set; } = "New Graph";

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

            }
        }

#if UNITY_EDITOR
        public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            if (nodes.Count > 0) {
                ProcessEvents(e, viewRect);
                for (int i = 0; i < nodes.Count; i++) {
                    nodes[i].UpdateNodeGUI(e, viewRect, viewSkin);
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}