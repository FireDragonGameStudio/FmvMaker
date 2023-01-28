using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    [GraphElementsExtensionMethodsCache(typeof(FmvMakerGraphView))]
    public static class FmvMakerGraphViewFactoryExtensions {

        public static IModelUI CreateNode(this ElementBuilder elementBuilder, CommandDispatcher dispatcher, StartVideoNodeModel model) {
            IModelUI ui = new StartVideoNode();
            ui.SetupBuildAndUpdate(model, dispatcher, elementBuilder.View, elementBuilder.Context);
            return ui;
        }

        public static IModelUI CreateNode(this ElementBuilder elementBuilder, CommandDispatcher dispatcher, MixNodeModel model) {
            IModelUI ui = new VariablePortNode();
            ui.SetupBuildAndUpdate(model, dispatcher, elementBuilder.View, elementBuilder.Context);
            return ui;
        }

        public static IModelUI CreateNode(this ElementBuilder elementBuilder, CommandDispatcher dispatcher, ClickableNodeModel model) {
            IModelUI ui = new ClickableNode();
            ui.SetupBuildAndUpdate(model, dispatcher, elementBuilder.View, elementBuilder.Context);
            return ui;
        }
    }
}