using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [UnitCategory("Control\\FmvMaker")]
    [TypeIcon(typeof(IBranchUnit))]
    public class SwitchOnFmvVideoEnum : Unit, IBranchUnit {
        [DoNotSerialize]
        public Dictionary<string, ControlOutput> branches { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ControlInput enter { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ValueInput FmvTargetVideo { get; private set; }

        [Serialize, Inspectable, UnitHeaderInspectable("StartIndex (incl.)")]
        public int StartIndex { get; set; } = 0;

        [Serialize, Inspectable, UnitHeaderInspectable("EndIndex (incl.)")]
        public int EndIndex { get; set; } = Enum.GetNames(typeof(FmvVideoEnum)).Length - 1;

        protected override void Definition() {
            branches = new Dictionary<string, ControlOutput>();

            enter = ControlInput(nameof(enter), EnterTrigger);

            FmvTargetVideo = ValueInput<FmvGraphElementData>(nameof(FmvTargetVideo));

            Requirement(FmvTargetVideo, enter);

            string[] enumNames = Enum.GetNames(typeof(FmvVideoEnum));
            for (int i = StartIndex; i <= EndIndex; i++) {
                if (!branches.ContainsKey(enumNames[i])) {
                    var branch = ControlOutput("%" + enumNames[i]);
                    branches.Add(enumNames[i], branch);

                    Succession(enter, branch);
                }
            }
        }

        public ControlOutput EnterTrigger(Flow flow) {
            var fmvTargetValue = flow.GetValue<FmvGraphElementData>(FmvTargetVideo);

            if (branches.ContainsKey(fmvTargetValue.VideoTarget.ToString())) {
                return branches[fmvTargetValue.VideoTarget.ToString()];
            }
            return null;
        }
    }
}