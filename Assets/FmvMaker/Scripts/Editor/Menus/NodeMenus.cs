using Assets.FmvMaker.Scripts.Editor.Windows;
using UnityEditor;

namespace Assets.FmvMaker.Scripts.Editor.Menus {
    public static class NodeMenus {
        [MenuItem("FmvMaker/Launch Graph Editor")]
        public static void InitNodeEditor() {
            NodeEditorWindow.InitEditorWindow();
        }
    }
}