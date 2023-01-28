using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class StartVideoNodeInspectorFields : FieldsInspector {
        public static StartVideoNodeInspectorFields Create(string name, IGraphElementModel model, IModelUI ownerElement, string parentClassName) {
            if (model is StartVideoNodeModel) {
                return new StartVideoNodeInspectorFields(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        StartVideoNodeInspectorFields(string name, IGraphElementModel model, IModelUI ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) { }

        protected override IEnumerable<BaseModelPropertyField> GetFields() {
            if (m_Model is StartVideoNodeModel startvideoNodeModel) {

                yield return new ModelPropertyField<string>(
                    m_OwnerElement.CommandDispatcher,
                    startvideoNodeModel,
                    nameof(StartVideoNodeModel.PickUpVideo),
                    null,
                    typeof(SetPickUpVideoCommand));
            }
        }
    }
}