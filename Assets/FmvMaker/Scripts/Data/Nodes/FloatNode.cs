using System;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Data.Nodes {
    [Serializable]
    public class FloatNode : NodeBase {

        public float NodeValue;

        public override void InitNode() {
            base.InitNode();
            Type = NodeType.Float;
            NodeRect = new Rect(10f, 10f, 150f, 65f);
        }

        public override void UpdateNode(Event e, Rect viewRect) {
            base.UpdateNode(e, viewRect);
        }

#if UNITY_EDITOR
        public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin) {
            base.UpdateNodeGUI(e, viewRect, viewSkin);
        }
    }
#endif
}
