using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace FmvMaker.Core.GTF {
    public class ClickableNodeInspectorFields : FieldsInspector {
        public static ClickableNodeInspectorFields Create(string name, IGraphElementModel model, IModelUI ownerElement, string parentClassName) {
            if (model is ClickableNodeModel) {
                return new ClickableNodeInspectorFields(name, model, ownerElement, parentClassName);
            }

            return null;
        }

        ClickableNodeInspectorFields(string name, IGraphElementModel model, IModelUI ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) { }

        protected override IEnumerable<BaseModelPropertyField> GetFields() {
            if (m_Model is ClickableNodeModel clickableNodeModel) {
                yield return new ModelPropertyField<string>(
                    m_OwnerElement.CommandDispatcher,
                    clickableNodeModel,
                    nameof(ClickableNodeModel.Name),
                    nameof(ClickableNodeModel.Name) + " (C)",
                    typeof(SetNameCommand));

                yield return new ModelPropertyField<string>(
                    m_OwnerElement.CommandDispatcher,
                    clickableNodeModel,
                    nameof(ClickableNodeModel.Description),
                    null,
                    typeof(SetDescriptionCommand));

                yield return new ModelPropertyField<string>(
                    m_OwnerElement.CommandDispatcher,
                    clickableNodeModel,
                    nameof(ClickableNodeModel.PickUpVideo),
                    null,
                    typeof(SetPickUpVideoCommand));

                //yield return new ModelPropertyField<string>(
                //    m_OwnerElement.CommandDispatcher,
                //    clickableNodeModel,
                //    nameof(ClickableNodeModel.UseageVideo),
                //    null,
                //    typeof(SetDescriptionCommand));

                yield return new ModelPropertyField<bool>(
                    m_OwnerElement.CommandDispatcher,
                    clickableNodeModel,
                    nameof(ClickableNodeModel.IsNavigation),
                    null,
                    typeof(SetIsNavigationCommand));

                //yield return new ModelPropertyField<bool>(
                //    m_OwnerElement.CommandDispatcher,
                //    clickableNodeModel,
                //    nameof(ClickableNodeModel.IsInInventory),
                //    null,
                //    typeof(SetDescriptionCommand));

                //yield return new ModelPropertyField<bool>(
                //    m_OwnerElement.CommandDispatcher,
                //    clickableNodeModel,
                //    nameof(ClickableNodeModel.WasUsed),
                //    null,
                //    typeof(SetDescriptionCommand));

                //yield return new ModelPropertyField<Vector2>(
                //    m_OwnerElement.CommandDispatcher,
                //    clickableNodeModel,
                //    nameof(ClickableNodeModel.RelativePosition),
                //    null,
                //    typeof(SetDescriptionCommand));
            }
        }
    }
}