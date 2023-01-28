using System;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    [Serializable]
    [SearcherItem(typeof(FmvMakerStencil), SearcherContext.Graph, "StartVideo")]
    public class StartVideoNodeModel : ClickableNodeModel {

        public override string Title {
            get => "FMVGame Start";
            set { }
        }

        protected override void OnDefineNode() {
            //this.AddDataOutputPort<string>(PickUpVideo);
            this.AddExecutionOutputPort(PickUpVideo);
        }
    }
}