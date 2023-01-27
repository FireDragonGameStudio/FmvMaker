using Assets.FmvMaker.Scripts.Data;
using Assets.FmvMaker.Scripts.Editor.Utils;
using Assets.FmvMaker.Scripts.Editor.Windows;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Views {
    [Serializable]
    public class NodeWorkView : ViewBase {

        private Vector2 mousePosition;
        private int deleteNodeId = 0;
        private Event e;

        public NodeWorkView(GUISkin viewSkin) : base("Work View") {
            this.viewSkin = viewSkin;
        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentNodeGraph) {
            base.UpdateView(editorRect, percentageRect, e, currentNodeGraph);

            GUI.Box(ViewRect, ViewTitle, viewSkin.GetStyle("GraphViewStyle"));
            NodeUtils.DrawGrid(ViewRect, 100f, 0.15f, Color.white);
            NodeUtils.DrawGrid(ViewRect, 20f, 0.05f, Color.white);
            GUILayout.BeginArea(ViewRect);
            if (currentNodeGraph != null) {
                currentNodeGraph.UpdateGraphGUI(e, ViewRect, viewSkin);
            }
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
                    if (e.type == EventType.MouseDown) {
                        mousePosition = e.mousePosition;
                        bool overNode = false;

                        if (currentNodeGraph != null) {
                            if (currentNodeGraph.nodes.Count > 0) {
                                for (int i = 0; i < currentNodeGraph.nodes.Count; i++) {
                                    if (currentNodeGraph.nodes[i].NodeRect.Contains(mousePosition)) {
                                        overNode = true;
                                        deleteNodeId = i;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!overNode) {
                            ProcessContextMenu(e, 0);
                        } else {
                            ProcessContextMenu(e, 1);
                        }
                    }
                }
            }
        }

        private void ProcessContextMenu(Event e, int contextId) {
            GenericMenu menu = new GenericMenu();

            if (contextId == 0) {

                menu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "0");
                menu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "1");

                if (currentNodeGraph != null) {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Unload Graph"), false, ContextCallback, "2");

                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Float Node"), false, ContextCallback, "3");
                    menu.AddItem(new GUIContent("Add Node"), false, ContextCallback, "4");
                }
            }

            if (contextId == 1) {
                if (currentNodeGraph != null) {
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "5");
                }
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object obj) {
            switch (obj.ToString()) {
                case "0":
                    NodePopupWindow.InitNodePopup();
                    break;
                case "1":
                    NodeUtils.LoadGraph();
                    break;
                case "2":
                    NodeUtils.UnloadGraph();
                    break;
                case "3":
                    NodeUtils.CreateNode(currentNodeGraph, NodeType.Float, mousePosition);
                    break;
                case "4":
                    NodeUtils.CreateNode(currentNodeGraph, NodeType.Add, mousePosition);
                    break;
                case "5":
                    NodeUtils.DeleteNode(currentNodeGraph, deleteNodeId);
                    break;
                default:
                    break;
            }
        }
    }
}