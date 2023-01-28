using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace FmvMaker.Core.GTF {
    [Serializable]
    [SearcherItem(typeof(FmvMakerStencil), SearcherContext.Graph, "Clickable/Navigation")]
    public class ClickableNodeModel : NodeModel {

        [SerializeField]
        private string name;
        [SerializeField]
        private string description = "notes for this clickable";
        [SerializeField]
        private string pickUpVideo = "video to play on pick up";
        [SerializeField]
        private string useageVideo = "video to play on item useage";
        [SerializeField]
        private bool isNavigation = false;
        [SerializeField]
        private bool isInInventory = false;
        [SerializeField]
        private bool wasUsed = false;
        [SerializeField]
        private Vector2 relativePosition = Vector2.zero;

        public string Name {
            get => name;
            set => name = value;
        }

        public string Description {
            get => description;
            set => description = value;
        }

        public string PickUpVideo {
            get => pickUpVideo;
            set => pickUpVideo = value;
        }

        public string UseageVideo {
            get => useageVideo;
            set => useageVideo = value;
        }

        public bool IsNavigation {
            get => isNavigation;
            set => isNavigation = value;
        }

        public bool IsInInventory {
            get => isInInventory;
            set => isInInventory = value;
        }

        public bool WasUsed {
            get => wasUsed;
            set => wasUsed = value;
        }

        public Vector2 RelativePosition {
            get => relativePosition;
            set => relativePosition = value;
        }

        protected override void OnDefineNode() {
            base.OnDefineNode();

            AddInputPort("Clickable0", PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);
            AddInputPort("Clickable1", PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);

            AddOutputPort(PickUpVideo, PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);

            if (!string.IsNullOrEmpty(UseageVideo)) {
                AddOutputPort(UseageVideo, PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);
            }
        }
    }
}