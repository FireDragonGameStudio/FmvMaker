using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvTransitionNode : Unit {

        [DoNotSerialize, PortLabelHidden]
        public ControlInput Enter { get; private set; }

        [DoNotSerialize, PortLabel("True")]
        public ControlOutput IfTrue { get; private set; }

        [DoNotSerialize, PortLabel("False")]
        public ControlOutput IfFalse { get; private set; }

        [DoNotSerialize]
        public ValueInput FmvTargetVideo { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable("")]
        public FmvVideoEnum TransitionVideo { get; set; } = FmvVideoEnum.None;

        private FmvGraphElementData triggeredNavigationTarget;

        protected override void Definition() {
            Enter = ControlInput(nameof(Enter), TriggerFmvTransition);
            IfTrue = ControlOutput(nameof(IfTrue));
            IfFalse = ControlOutput(nameof(IfFalse));

            FmvTargetVideo = ValueInput<FmvGraphElementData>(nameof(FmvTargetVideo));

            Requirement(FmvTargetVideo, Enter);
            Succession(Enter, IfTrue);
            Succession(Enter, IfFalse);
        }

        private ControlOutput TriggerFmvTransition(Flow flow) {
            triggeredNavigationTarget = flow.GetValue<FmvGraphElementData>(FmvTargetVideo);

            if (triggeredNavigationTarget.VideoTarget == TransitionVideo) {
                Variables.Scene(SceneManager.GetActiveScene()).Set("CurrentVideoTarget", triggeredNavigationTarget);
                return IfTrue;
            }
            return IfFalse;
        }
    }
}