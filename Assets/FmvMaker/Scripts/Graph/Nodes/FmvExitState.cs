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

        private FmvGraphVideos fmvGraphVideos;

        private GameObject fmvVideoElementsPanel;

        protected override void Definition() {
            InputTrigger = ControlInput(nameof(InputTrigger), CleanUpStateData);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

        private ControlOutput CleanUpStateData(Flow flow) {
            GetSceneVariables();
            DestroyGameObjectsExceptInventory();
            RemoveClickListeners();

            return OutputTrigger;
        }

        private void GetSceneVariables() {
            fmvVideoElementsPanel = FmvSceneVariables.VideoElementsPanel;
            fmvGraphVideos = FmvSceneVariables.VideoView.GetComponent<FmvGraphVideos>();
        }

        private void DestroyGameObjectsExceptInventory() {
            if (fmvVideoElementsPanel) {
                FmvClickableFacade[] fmvClickableObjects = fmvVideoElementsPanel.GetComponentsInChildren<FmvClickableFacade>();
                foreach (FmvClickableFacade clickableFacade in fmvClickableObjects) {
                    GameObject.Destroy(clickableFacade.gameObject);
                }
            }
        }

        private void RemoveClickListeners() {
            if (fmvGraphVideos) {
                fmvGraphVideos.OnVideoStarted.RemoveAllListeners();
                fmvGraphVideos.OnVideoPaused.RemoveAllListeners();
                fmvGraphVideos.OnVideoFinished.RemoveAllListeners();
                fmvGraphVideos.OnVideoSkipped.RemoveAllListeners();
            }
        }
    }
}