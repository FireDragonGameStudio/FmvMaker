using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace FmvMaker.Core.GTF {
    public class FmvMakerGraphAssetModel : GraphAssetModel {
        [MenuItem("Assets/Create/FMVGame")]
        public static void CreateGraph(MenuCommand menuCommand) {
            const string path = "Assets";
            var template = new GraphTemplate<FmvMakerStencil>(FmvMakerStencil.graphName);
            CommandDispatcher commandDispatcher = null;
            if (EditorWindow.HasOpenInstances<FmvMakerGraphWindow>()) {
                var window = EditorWindow.GetWindow<FmvMakerGraphWindow>();
                if (window != null) {
                    commandDispatcher = window.CommandDispatcher;
                }
            }

            GraphAssetCreationHelpers<FmvMakerGraphAssetModel>.CreateInProjectWindow(template, commandDispatcher, path);
        }

        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceId, int line) {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is FmvMakerGraphAssetModel graphAssetModel) {
                var window = GraphViewEditorWindow.FindOrCreateGraphWindow<FmvMakerGraphWindow>();
                window.SetCurrentSelection(graphAssetModel, GraphViewEditorWindow.OpenMode.OpenAndFocus);
                return window != null;
            }

            return false;
        }

        protected override Type GraphModelType => typeof(FmvMakerGraphModel);
    }
}