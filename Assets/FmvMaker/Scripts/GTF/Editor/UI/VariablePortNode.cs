using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.UIElements;

namespace FmvMaker.Core.GTF {
    public class VariablePortNode : CollapsibleInOutNode {
        protected override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
            base.BuildContextualMenu(evt);

            if (!(Model is MixNodeModel mixNodeModel)) {
                return;
            }

            if (evt.menu.MenuItems().Count > 0)
                evt.menu.AppendSeparator();

            evt.menu.AppendAction($"Add Clickable", action: action => {
                CommandDispatcher.Dispatch(new AddPortCommand(new[] { mixNodeModel }));
            });

            evt.menu.AppendAction($"Remove Clickable", action: action => {
                CommandDispatcher.Dispatch(new RemovePortCommand(new[] { mixNodeModel }));
            });
        }
    }
}