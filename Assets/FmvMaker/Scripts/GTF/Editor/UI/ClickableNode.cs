using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class ClickableNode : CollapsibleInOutNode {

        public static readonly string paramContainerPartName = "parameter-container";

        protected override void BuildPartList() {
            base.BuildPartList();

            PartList.InsertPartAfter(titleIconContainerPartName, ClickablePart.Create(paramContainerPartName, Model, this, ussClassName));
        }
    }
}