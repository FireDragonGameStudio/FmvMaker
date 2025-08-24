using FmvMaker.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FmvMaker.Core {
    public class FmvMakerManager : MonoBehaviour {
        public FmvMakerRuntimeGraph RuntimeGraph;

        private Dictionary<string, FmvMakerNode> nodeLookup = new();
        private FmvMakerNode currentNode;

        private void Start() {
            foreach (var node in RuntimeGraph.AllFmvMakerNodes) {
                nodeLookup[node.NodeId] = node;
            }

            if (!string.IsNullOrEmpty(RuntimeGraph.EntryNodeId)) {
                ShowNode(RuntimeGraph.EntryNodeId);
            } else {
                EndFmvMaker();
            }
        }

        private void Update() {
            if (Mouse.current.leftButton.wasPressedThisFrame && currentNode?.DecisionData[0] != null) {
                if (!string.IsNullOrEmpty(currentNode.DecisionData[0].DestinationId)) {
                    ShowNode(currentNode.DecisionData[0].DestinationId);
                } else {
                    EndFmvMaker();
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame && currentNode?.DecisionData[1] != null) {
                if (!string.IsNullOrEmpty(currentNode.DecisionData[1].DestinationId)) {
                    ShowNode(currentNode.DecisionData[1].DestinationId);
                } else {
                    EndFmvMaker();
                }
            }
        }

        private void ShowNode(string nodeId) {
            if (!nodeLookup.ContainsKey(nodeId)) {
                EndFmvMaker();
                return;
            }

            currentNode = nodeLookup[nodeId];

            // play video and set ui elements for navigation
            Debug.Log(currentNode.NodeName);
        }

        private void EndFmvMaker() {
            Debug.Log("FmvMaker ended!");
            currentNode = null;
        }
    }
}