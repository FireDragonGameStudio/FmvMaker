using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvVideoView : Unit {

        [DoNotSerialize]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        public ControlOutput OutputTrigger;


        [DoNotSerialize]
        public ValueInput MyValueA;

        [DoNotSerialize]
        public ValueOutput Result;

        private FmvFacadeType resultValue;

        protected override void Definition() {

            InputTrigger = ControlInput("inputTrigger", (flow) => {
                //Making the resultValue equal to the input value from myValueB.
                resultValue = flow.GetValue<FmvFacadeType>(MyValueA);
                return OutputTrigger;
            });
            OutputTrigger = ControlOutput("outputTrigger");

            MyValueA = ValueInput<FmvFacadeType>("VideoToPlay");

            Result = ValueOutput<FmvFacadeType>("result", (flow) => { return resultValue; });

            Requirement(MyValueA, InputTrigger); //Specifies that we need the myValueA value to be set before the node can run.
            Succession(InputTrigger, OutputTrigger);
        }
    }
}