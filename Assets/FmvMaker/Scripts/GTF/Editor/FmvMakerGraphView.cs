using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class FmvMakerGraphView : GraphView {
        public FmvMakerGraphView(GraphViewEditorWindow window, CommandDispatcher commandDispatcher, string graphViewName)
                   : base(window, commandDispatcher, graphViewName) { }
    }
}