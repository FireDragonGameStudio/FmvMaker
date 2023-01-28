using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    [GraphElementsExtensionMethodsCache(typeof(ModelInspectorView))]
    public static class ModelInspectorFactoryExtensions {
        public static IModelUI CreateClickableNodeInspector(this ElementBuilder elementBuilder, CommandDispatcher dispatcher, ClickableNodeModel model) {
            var ui = UnityEditor.GraphToolsFoundation.Overdrive.ModelInspectorFactoryExtensions.CreateNodeInspector(elementBuilder, dispatcher, model);

            (ui as ModelUI)?.PartList.AppendPart(ClickableNodeInspectorFields.Create("clickable-node-fields", model, ui, ModelInspector.ussClassName));

            ui.BuildUI();
            ui.UpdateFromModel();

            return ui;
        }
    }
}