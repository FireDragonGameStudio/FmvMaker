using System.Collections.Generic;
using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    [TypeIcon(typeof(IBranchUnit))]
    public class SwitchOnFmvVideoEnum : Unit, IBranchUnit {
        [DoNotSerialize]
        public Dictionary<string, ControlOutput> branches { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ControlInput enter { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ValueInput FmvTargetVideo { get; private set; }

        protected override void Definition() {
            branches = new Dictionary<string, ControlOutput>();

            enter = ControlInput(nameof(enter), EnterTrigger);

            FmvTargetVideo = ValueInput<FmvGraphElementData>(nameof(FmvTargetVideo));

            Requirement(FmvTargetVideo, enter);

            foreach (var valueByName in EnumUtility.ValuesByNames(typeof(FmvVideoEnum))) {
                var enumName = valueByName.Key;

                // Just like in C#, duplicate switch labels for the same underlying value is prohibited
                if (!branches.ContainsKey(enumName)) {
                    var branch = ControlOutput("%" + enumName);
                    branches.Add(enumName, branch);

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