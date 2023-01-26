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

        public NodeWorkView(GUISkin viewSkin) : base("Work View") {
            this.viewSkin = viewSkin;
        }

        public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph currentNodeGraph) {
            base.UpdateView(editorRect, percentageRect, e, currentNodeGraph);

            GUI.Box(ViewRect, ViewTitle, viewSkin.GetStyle("GraphViewStyle"));

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
                        ProcessContextMenu(e);
                    }
                }
            }
        }

        private void ProcessContextMenu(Event e) {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "0");
            menu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "1");

            if (currentNodeGraph != null) {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Unload Graph"), false, ContextCallback, "2");

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Float Node"), false, ContextCallback, "3");
                menu.AddItem(new GUIContent("Add Node"), false, ContextCallback, "4");
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

                    break;
                default:
                    break;
            }
        }
    }
}