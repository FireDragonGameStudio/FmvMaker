using FmvMaker.Core.Models;
using Unity.VisualScripting;

[UnitCategory("FmvMaker")]
public class FmvClickableNode : Unit {

    [DoNotSerialize]
    [PortLabelHidden]
    public ValueInput FmvTargetVideo { get; private set; }

    [DoNotSerialize]
    public ValueInput IsNavigation { get; private set; }

    [DoNotSerialize]
    [PortLabelHidden]
    public ValueOutput ClickableItem { get; private set; }

    protected override void Definition() {
        FmvTargetVideo = ValueInput<FmvVideoEnum>("FmvTargetVideo", FmvVideoEnum.UniqueVideoName);
        IsNavigation = ValueInput("IsNavigation", false);

        ClickableItem = ValueOutput<object>("Data", (flow) => {
            string videoName = flow.GetValue<FmvVideoEnum>(FmvTargetVideo).ToString();
            bool isNavigation = flow.GetValue<bool>(IsNavigation);
            return new ClickableModel() {
                Name = videoName,
                PickUpVideo = videoName,
                IsNavigation = true,
            };
        });
    }
}
