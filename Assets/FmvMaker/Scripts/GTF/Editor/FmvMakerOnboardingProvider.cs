using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine.UIElements;

namespace FmvMaker.Core.GTF {
    public class FmvMakerOnboardingProvider : OnboardingProvider {
        public override VisualElement CreateOnboardingElements(CommandDispatcher store) {
            var template = new GraphTemplate<FmvMakerStencil>(FmvMakerStencil.graphName);
            return AddNewGraphButton<FmvMakerGraphAssetModel>(template);
        }
    }
}