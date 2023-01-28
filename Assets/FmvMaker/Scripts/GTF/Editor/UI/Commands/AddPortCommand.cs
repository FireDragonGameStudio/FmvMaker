using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class AddPortCommand : ModelCommand<MixNodeModel> {

        const string k_UndoStringSingular = "Add Clickable";

        public AddPortCommand(MixNodeModel[] nodes)
            : base(k_UndoStringSingular, k_UndoStringSingular, nodes) {
        }

        public static void DefaultHandler(GraphToolState state, AddPortCommand command) {
            state.PushUndo(command);

            using (var graphUpdater = state.GraphViewState.UpdateScope) {
                foreach (var nodeModel in command.Models) {
                    nodeModel.AddClickablePort();
                    graphUpdater.MarkChanged(nodeModel);
                }
            }
        }
    }
}