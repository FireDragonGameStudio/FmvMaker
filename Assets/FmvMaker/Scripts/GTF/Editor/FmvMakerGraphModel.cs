using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace FmvMaker.Core.GTF {
    public class FmvMakerGraphModel : GraphModel {
        protected override bool IsCompatiblePort(IPortModel startPortModel, IPortModel compatiblePortModel) {
            return startPortModel.DataTypeHandle == compatiblePortModel.DataTypeHandle;
        }
    }
}