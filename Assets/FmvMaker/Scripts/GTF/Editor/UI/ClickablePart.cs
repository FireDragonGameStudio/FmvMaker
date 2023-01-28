using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.UIElements;

namespace FmvMaker.Core.GTF {
    public class ClickablePart : BaseModelUIPart {

        public static readonly string ussClassName = "ge-clickable-node-part";
        public static readonly string nameLabelName = "name";
        public static readonly string descriptionLabelName = "description";
        public static readonly string pickUpVideoLabelName = "pickupvideo";
        public static readonly string isNavigationLabelName = "navigation";

        public static ClickablePart Create(string name, IGraphElementModel model, IModelUI modelUI, string parentClassName) {
            if (model is INodeModel) {
                return new ClickablePart(name, model, modelUI, parentClassName);
            }

            return null;
        }

        VisualElement NameAndDescriptionContainer { get; set; }
        EditableLabel NameLabel { get; set; }
        EditableLabel DescriptionLabel { get; set; }
        EditableLabel PickUpVideoLabel { get; set; }
        Toggle IsNavigationToggle { get; set; }

        public override VisualElement Root => NameAndDescriptionContainer;

        ClickablePart(string name, IGraphElementModel model, IModelUI ownerElement, string parentClassName)
            : base(name, model, ownerElement, parentClassName) {
        }

        protected override void BuildPartUI(VisualElement container) {
            if (!(m_Model is ClickableNodeModel))
                return;

            NameAndDescriptionContainer = new VisualElement { name = PartName };
            NameAndDescriptionContainer.AddToClassList(ussClassName);
            NameAndDescriptionContainer.AddToClassList(m_ParentClassName.WithUssElement(PartName));

            NameLabel = new EditableLabel { name = nameLabelName };
            NameLabel.RegisterCallback<ChangeEvent<string>>(OnChangeName);
            NameLabel.AddToClassList(ussClassName.WithUssElement("description"));
            NameLabel.AddToClassList(m_ParentClassName.WithUssElement("description"));
            NameAndDescriptionContainer.Add(NameLabel);

            DescriptionLabel = new EditableLabel { name = descriptionLabelName };
            DescriptionLabel.RegisterCallback<ChangeEvent<string>>(OnChangeDescription);
            DescriptionLabel.AddToClassList(ussClassName.WithUssElement("description"));
            DescriptionLabel.AddToClassList(m_ParentClassName.WithUssElement("description"));
            NameAndDescriptionContainer.Add(DescriptionLabel);

            PickUpVideoLabel = new EditableLabel { name = pickUpVideoLabelName };
            PickUpVideoLabel.RegisterCallback<ChangeEvent<string>>(OnChangePickUpVideo);
            PickUpVideoLabel.AddToClassList(ussClassName.WithUssElement("description"));
            PickUpVideoLabel.AddToClassList(m_ParentClassName.WithUssElement("description"));
            NameAndDescriptionContainer.Add(PickUpVideoLabel);

            IsNavigationToggle = new Toggle { name = isNavigationLabelName, label = isNavigationLabelName };
            IsNavigationToggle.RegisterCallback<ChangeEvent<bool>>(OnChangeIsNavigation);
            IsNavigationToggle.AddToClassList(ussClassName.WithUssElement("description"));
            IsNavigationToggle.AddToClassList(m_ParentClassName.WithUssElement("description"));
            NameAndDescriptionContainer.Add(IsNavigationToggle);

            container.Add(NameAndDescriptionContainer);
        }

        void OnChangeName(ChangeEvent<string> evt) {
            if (!(m_Model is ClickableNodeModel clickableNodeModel))
                return;

            m_OwnerElement.CommandDispatcher.Dispatch(new SetNameCommand(evt.newValue, nodes: clickableNodeModel));
        }

        void OnChangeDescription(ChangeEvent<string> evt) {
            if (!(m_Model is ClickableNodeModel clickableNodeModel))
                return;

            m_OwnerElement.CommandDispatcher.Dispatch(new SetDescriptionCommand(evt.newValue, nodes: clickableNodeModel));
        }

        void OnChangePickUpVideo(ChangeEvent<string> evt) {
            if (!(m_Model is ClickableNodeModel clickableNodeModel))
                return;

            m_OwnerElement.CommandDispatcher.Dispatch(new SetPickUpVideoCommand(evt.newValue, nodes: clickableNodeModel));
        }

        void OnChangeIsNavigation(ChangeEvent<bool> evt) {
            if (!(m_Model is ClickableNodeModel clickableNodeModel))
                return;

            m_OwnerElement.CommandDispatcher.Dispatch(new SetIsNavigationCommand(evt.newValue, nodes: clickableNodeModel));
        }

        protected override void PostBuildPartUI() {
            base.PostBuildPartUI();

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FmvMaker/Scripts/GTF/Editor/UI/ClickableNodePart.uss");
            if (stylesheet != null) {
                NameAndDescriptionContainer.styleSheets.Add(stylesheet);
            }
        }

        protected override void UpdatePartFromModel() {
            if (!(m_Model is ClickableNodeModel clickableNodeModel))
                return;

            NameLabel.SetValueWithoutNotify($"{clickableNodeModel.Name}");
            DescriptionLabel.SetValueWithoutNotify($"{clickableNodeModel.Description}");
            PickUpVideoLabel.SetValueWithoutNotify($"{clickableNodeModel.PickUpVideo}");
            IsNavigationToggle.SetValueWithoutNotify(clickableNodeModel.IsNavigation);
        }
    }
}