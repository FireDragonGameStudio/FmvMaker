using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace FmvMaker.Core.GTF {
    [Serializable]
    [SearcherItem(typeof(FmvMakerStencil), SearcherContext.Graph, "Clickable/Multiple")]
    public class MixNodeModel : NodeModel {

        [SerializeField, HideInInspector]
        private int clickableCount = 2;

        protected override void OnDefineNode() {
            base.OnDefineNode();

            //AddInputPort("Clickable", PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);

            for (var i = 0; i < clickableCount; i++) {
                AddInputPort("Clickable " + (i + 1), PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);
            }

            AddOutputPort("NavigationTarget", PortType.Data, FmvMakerStencil.Clickable, options: PortModelOptions.NoEmbeddedConstant);
        }

        public void AddClickablePort() {
            clickableCount++;
            DefineNode();
        }

        public void RemoveClickablePort() {
            clickableCount--;
            if (clickableCount < 2)
                clickableCount = 2;

            DefineNode();
        }
    }
}