using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class SetNameCommand : ModelCommand<ClickableNodeModel, string> {

        const string k_UndoStringSingular = "Set Clickable Node Name";
        const string k_UndoStringPlural = "Set Clickable Nodes Names";

        public SetNameCommand(string value, params ClickableNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringPlural, value, nodes) {
        }

        public static void DefaultHandler(GraphToolState state, SetNameCommand command) {
            state.PushUndo(command);

            using (var graphUpdater = state.GraphViewState.UpdateScope) {
                foreach (var nodeModel in command.Models) {
                    nodeModel.Name = command.Value;
                    graphUpdater.MarkChanged(nodeModel);
                }
            }
        }
    }
}