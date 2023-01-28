using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class FmvMakerGraphWindow : GraphViewEditorWindow {

        [InitializeOnLoadMethod]
        static void RegisterTool() {
            ShortcutHelper.RegisterDefaultShortcuts<FmvMakerGraphWindow>(FmvMakerStencil.toolName);
        }

        [MenuItem("FmvMaker/FmvMaker Editor", false)]
        public static void ShowFmvMakerGraphWindow() {
            FindOrCreateGraphWindow<FmvMakerGraphWindow>();
        }

        protected override void OnEnable() {
            EditorToolName = "FmvMaker Editor";
            base.OnEnable();
        }

        /// <inheritdoc />
        protected override GraphToolState CreateInitialState() {
            var prefs = Preferences.CreatePreferences(EditorToolName);
            return new FmvMakerState(GUID, prefs);
        }

        protected override GraphView CreateGraphView() {
            return new FmvMakerGraphView(this, CommandDispatcher, EditorToolName);
        }

        protected override BlankPage CreateBlankPage() {
            var onboardingProviders = new List<OnboardingProvider>();
            onboardingProviders.Add(new FmvMakerOnboardingProvider());

            return new BlankPage(CommandDispatcher, onboardingProviders);
        }

        /// <inheritdoc />
        protected override bool CanHandleAssetType(IGraphAssetModel asset) {
            return asset is FmvMakerGraphAssetModel;
        }
    }
}