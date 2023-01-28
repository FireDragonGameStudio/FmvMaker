using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class SetPickUpVideoCommand : ModelCommand<ClickableNodeModel, string> {

        const string k_UndoStringSingular = "Set Clickable Node PickUp Video";
        const string k_UndoStringPlural = "Set Clickable Nodes PickUp Video";

        public SetPickUpVideoCommand(string value, params ClickableNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringPlural, value, nodes) {
        }

        public static void DefaultHandler(GraphToolState state, SetPickUpVideoCommand command) {
            state.PushUndo(command);

            using (var graphUpdater = state.GraphViewState.UpdateScope) {
                foreach (var nodeModel in command.Models) {
                    nodeModel.PickUpVideo = command.Value;
                    graphUpdater.MarkChanged(nodeModel);
                }
            }
        }
    }
}
