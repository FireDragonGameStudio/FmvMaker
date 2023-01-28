using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class FmvMakerStencil : Stencil {

        public static string toolName = "Recipe Editor";

        public override string ToolName => toolName;

        public static readonly string graphName = "FMV Game";
        public static TypeHandle Clickable { get; } = TypeHandleHelpers.GenerateCustomTypeHandle("Clickable");

        public FmvMakerStencil() {

        }

        /// <inheritdoc />
        public override IBlackboardGraphModel CreateBlackboardGraphModel(IGraphAssetModel graphAssetModel) {
            return new FmvMakerBlackboardGraphModel(graphAssetModel);
        }
    }
}