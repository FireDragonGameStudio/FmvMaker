using FmvMaker.Core.Facades;
using FmvMaker.Core.Models;
using FmvMaker.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FmvMaker.Core.Provider {
    public class FmvNavigations : MonoBehaviour {

        [SerializeField]
        private FmvVideos fmvVideos = null;
        [SerializeField]
        private RectTransform videoElementsPanel = null;
        [SerializeField]
        private GameObject navigationObjectPrefab = null;

        private List<NavigationModel> allNavigations = new List<NavigationModel>();
        private List<FmvNavigationFacade> allNavigationTargets = new List<FmvNavigationFacade>();

        private void Awake() {
            LoadNavigationTargets();
            GenerateAllNavigationTargets();
        }

        private void LoadNavigationTargets() {
            allNavigations.AddRange(FmvData.GenerateNavigationDataFromLocalFile());
        }

        private void GenerateAllNavigationTargets() {
            allNavigationTargets.AddRange(GenerateNavigationTargets(allNavigations, videoElementsPanel));

            ConfigureNavigationTagets();
        }

        private void ConfigureNavigationTagets() {
            for (int i = 0; i < allNavigationTargets.Count; i++) {
                SetEventsForNavigationTarget(allNavigationTargets[i]);
                SetNavigationTargetInactive(allNavigationTargets[i]);
            }
        }

        private void SetEventsForNavigationTarget(FmvNavigationFacade navigationFacade) {
            navigationFacade.OnNavigationClicked.RemoveAllListeners();
            navigationFacade.OnNavigationClicked.AddListener(TriggerNavigationTarget);
        }

        private void SetNavigationTargetInactive(FmvNavigationFacade navigationFacade) {
            navigationFacade.gameObject.SetActive(false);
        }

        private void TriggerNavigationTarget(NavigationModel model) {
            fmvVideos.PlayVideoFromNavigationTarget(model.Name);
        }

        private List<FmvNavigationFacade> GenerateNavigationTargets(IEnumerable<NavigationModel> navigations, RectTransform parent) {
            List<FmvNavigationFacade> createdNavigations = new List<FmvNavigationFacade>();
            foreach (NavigationModel navigation in navigations) {
                createdNavigations.Add(CreateNavigation(navigation, parent));
            }
            return createdNavigations;
        }

        private FmvNavigationFacade CreateNavigation(NavigationModel navigationModel, RectTransform parent) {
            FmvNavigationFacade navigationFacade = CreateNavigationFacade(parent);
            navigationFacade.SetNavigationData(navigationModel);
            return navigationFacade;
        }

        private FmvNavigationFacade CreateNavigationFacade(Transform parentTransform) {
            GameObject targetObject = Instantiate(navigationObjectPrefab);
            targetObject.SetActive(true);
            targetObject.transform.SetParent(parentTransform);
            targetObject.transform.localScale = Vector3.one;

            return targetObject.GetComponent<FmvNavigationFacade>();
        }

        public void SetNavigationTargetsActive(NavigationModel[] navigationModels) {
            for (int i = 0; i < navigationModels.Length; i++) {
                allNavigationTargets
                    .SingleOrDefault(navigation => navigation.name.ToLower().Equals(navigationModels[i].Name.ToLower()))
                    ?.gameObject.SetActive(true);
            }
        }

        public NavigationModel GetNavigationModelByName(string navigationName) {
            return allNavigations.Single(navigation => navigation.Name.ToLower().Equals(navigationName.ToLower()));
        }

        public void DisableNavigationTargets() {
            allNavigationTargets.ForEach(navigation => navigation.gameObject.SetActive(false));
        }
    }
}