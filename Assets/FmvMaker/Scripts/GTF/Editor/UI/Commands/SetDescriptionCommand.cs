using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class SetDescriptionCommand : ModelCommand<ClickableNodeModel, string> {

        const string k_UndoStringSingular = "Set Clickable Node Description";
        const string k_UndoStringPlural = "Set Clickable Nodes Description";

        public SetDescriptionCommand(string value, params ClickableNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringPlural, value, nodes) {
        }

        public static void DefaultHandler(GraphToolState state, SetDescriptionCommand command) {
            state.PushUndo(command);

            using (var graphUpdater = state.GraphViewState.UpdateScope) {
                foreach (var nodeModel in command.Models) {
                    nodeModel.Description = command.Value;
                    graphUpdater.MarkChanged(nodeModel);
                }
            }
        }
    }
}