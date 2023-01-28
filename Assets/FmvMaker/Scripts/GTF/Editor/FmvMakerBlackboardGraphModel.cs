using System.Collections.Generic;
using System.Linq;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace FmvMaker.Core.GTF {
    public class FmvMakerBlackboardGraphModel : BlackboardGraphModel {
        internal static readonly string[] k_Sections = { "Clickables" };

        /// <inheritdoc />
        public FmvMakerBlackboardGraphModel(IGraphAssetModel graphAssetModel)
            : base(graphAssetModel) { }

        public override string GetBlackboardTitle() {
            return AssetModel?.FriendlyScriptName == null ? "FmvMaker" : AssetModel?.FriendlyScriptName + " FmvMaker";
        }

        public override string GetBlackboardSubTitle() {
            return "FMVs for the world!";
        }

        public override IEnumerable<string> SectionNames =>
            GraphModel == null ? Enumerable.Empty<string>() : k_Sections;

        public override IEnumerable<IVariableDeclarationModel> GetSectionRows(string sectionName) {
            if (sectionName == k_Sections[0]) {
                return GraphModel?.VariableDeclarations?.Where(v => v.DataType == FmvMakerStencil.Clickable) ??
                    Enumerable.Empty<IVariableDeclarationModel>();
            }

            return Enumerable.Empty<IVariableDeclarationModel>();
        }
    }
}