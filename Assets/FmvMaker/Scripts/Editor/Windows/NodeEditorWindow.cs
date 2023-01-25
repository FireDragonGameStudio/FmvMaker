using Assets.FmvMaker.Scripts.Editor.Views;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Windows {
    public class NodeEditorWindow : EditorWindow {

        public static NodeEditorWindow editorWindow;

        public NodePropertyView propertyView;
        public NodeWorkView workView;

        private float viewPercentage = 0.75f;

        public static void InitEditorWindow() {
            editorWindow = GetWindow<NodeEditorWindow>();
            editorWindow.titleContent = new GUIContent("FmvMaker Editor");

            CreateViews();
        }

        private void OnEnable() {

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
                e);
            propertyView.UpdateView(
                new Rect(position.width, position.y, position.width, position.height),
                new Rect(viewPercentage, 0f, 1 - viewPercentage, 1f),
                e);

            Repaint();
        }

        private static void CreateViews() {
            if (editorWindow == null) {
                editorWindow = GetWindow<NodeEditorWindow>();
            }
            editorWindow.propertyView = new NodePropertyView();
            editorWindow.workView = new NodeWorkView();
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
    }
}