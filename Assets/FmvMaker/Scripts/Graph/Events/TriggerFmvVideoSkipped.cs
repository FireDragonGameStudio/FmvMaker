using FmvMaker.Graph;
using Unity.VisualScripting;

[UnitTitle("Trigger Fmv Video Skipped")]
[UnitCategory("Events\\FmvMaker")]
public class TriggerFmvVideoSkipped : Unit {
    [DoNotSerialize, PortLabelHidden]
    public ControlInput Enter { get; private set; }

    [DoNotSerialize, PortLabelHidden]
    public ControlOutput Exit { get; private set; }

    [DoNotSerialize, PortLabelHidden]
    public ValueInput Target { get; private set; }

    protected override void Definition() {
        Enter = ControlInput(nameof(Enter), Trigger);
        Exit = ControlOutput(nameof(Exit));

        Target = ValueInput<FmvGraphElementData>(nameof(Target));

        Requirement(Target, Enter);
        Succession(Enter, Exit);
    }

    private ControlOutput Trigger(Flow flow) {
        var target = flow.GetValue<FmvGraphElementData>(this.Target);
        OnFmvVideoSkipped.Trigger(target);

        return Exit;
    }
}
