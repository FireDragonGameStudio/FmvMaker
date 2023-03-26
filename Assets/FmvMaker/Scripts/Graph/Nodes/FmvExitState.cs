using FmvMaker.Core.Facades;
using Unity.VisualScripting;
using UnityEngine;

namespace FmvMaker.Graph {
    [UnitCategory("FmvMaker")]
    public class FmvExitState : Unit {

        [DoNotSerialize, PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }

        [DoNotSerialize, PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        private GameObject fmvVideoElementsPanel;

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), CleanUpStateData);
        }

        private ControlOutput CleanUpStateData(Flow flow) {
            GetSceneVariables();
            DestroyGameObjectsExceptInventory();

            return OutputTrigger;
        }

        private void GetSceneVariables() {
            if (Variables.ExistInActiveScene && Variables.ActiveScene.IsDefined("VideoElementsPanel")) {
                fmvVideoElementsPanel = Variables.ActiveScene.Get("VideoElementsPanel") as GameObject;
            }
        }

        private void DestroyGameObjectsExceptInventory() {
            if (fmvVideoElementsPanel) {
                foreach (FmvClickableFacade clickableNavOrItemFacade in fmvVideoElementsPanel.GetComponentsInChildren<FmvClickableFacade>()) {
                    GameObject.Destroy(clickableNavOrItemFacade.gameObject);

                }
            }
        }
    }
}