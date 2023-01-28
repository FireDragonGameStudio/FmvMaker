using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class SetIsNavigationCommand : ModelCommand<ClickableNodeModel, bool> {

        const string k_UndoStringSingular = "Set Clickable Node IsNavigation";
        const string k_UndoStringPlural = "Set Clickable Nodes IsNavigation";

        public SetIsNavigationCommand(bool value, params ClickableNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringPlural, value, nodes) {
        }

        public static void DefaultHandler(GraphToolState state, SetIsNavigationCommand command) {
            state.PushUndo(command);

            using (var graphUpdater = state.GraphViewState.UpdateScope) {
                foreach (var nodeModel in command.Models) {
                    nodeModel.IsNavigation = command.Value;
                    graphUpdater.MarkChanged(nodeModel);
                }
            }
        }
    }
}
