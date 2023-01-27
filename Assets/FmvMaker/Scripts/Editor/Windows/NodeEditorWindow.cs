using Assets.FmvMaker.Scripts.Data;
using Assets.FmvMaker.Scripts.Editor.Views;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Windows {
    public class NodeEditorWindow : EditorWindow {

        public static NodeEditorWindow editorWindow;

        public NodePropertyView propertyView;
        public NodeWorkView workView;

        public NodeGraph CurrentNodeGraph = null;

        private float viewPercentage = 0.75f;

        public static void InitEditorWindow() {
            editorWindow = GetWindow<NodeEditorWindow>();
            editorWindow.titleContent = new GUIContent("FmvMaker Editor");
        }

        private void OnEnable() {
            if (workView == null || propertyView == null) {
                GUISkin viewSkin = Resources.Load("GUISkins/NodeEditorSkin") as GUISkin;
                CreateViews(viewSkin);
                return;
            }
        }

        private void OnDestroy() {

        }

        private void Update() {

        }

        private void OnGUI() {
            Event e = Event.current;
            CheckForKeyboardInputs(e);

            workView.UpdateView(
                position,
                new Rect(0f, 0f, viewPercentage, 1f),
                e, CurrentNodeGraph);

            propertyView.UpdateView(
                new Rect(position.width, position.y, position.width, position.height),
                new Rect(viewPercentage, 0f, 1 - viewPercentage, 1f),
                e, CurrentNodeGraph);

            Repaint();
        }

        private static void CreateViews(GUISkin viewSkin) {
            if (editorWindow == null) {
                editorWindow = GetWindow<NodeEditorWindow>();
            }
            editorWindow.propertyView = new NodePropertyView(viewSkin);
            editorWindow.workView = new NodeWorkView(viewSkin);
        }

        private void CheckForKeyboardInputs(Event e) {
            if (e.type == EventType.KeyDown) {
                if (e.keyCode == KeyCode.LeftArrow) {
                    viewPercentage -= 0.01f;
                }
                if (e.keyCode == KeyCode.RightArrow) {
                    viewPercentage += 0.01f;
                }
            }
        }

        private GUISkin GetEditorSkin() {
            return Resources.Load("GUISkins/NodeEditorSkin") as GUISkin;
        }
    }
}