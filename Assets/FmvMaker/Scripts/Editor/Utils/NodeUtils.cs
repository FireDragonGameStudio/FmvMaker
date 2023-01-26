using Assets.FmvMaker.Scripts.Data;
using Assets.FmvMaker.Scripts.Data.Nodes;
using Assets.FmvMaker.Scripts.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Utils {
    public static class NodeUtils {
        public static void CreateNewGraph(string graphName) {
            NodeGraph currentGraph = ScriptableObject.CreateInstance<NodeGraph>();
            if (currentGraph != null) {
                currentGraph.Name = graphName;
                currentGraph.InitGraph();

                AssetDatabase.CreateAsset(currentGraph, $"Assets/FmvMaker/NodeGraphs/{graphName}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null) {
                    currentWindow.CurrentNodeGraph = currentGraph;
                }
            } else {
                EditorUtility.DisplayDialog("Node Graph Message", "Unable to create new graph.", "OK");
            }
        }

        public static void LoadGraph() {
            NodeGraph currentGraph = null;
            string graphPath = EditorUtility.OpenFilePanel("Load Graph", $"{Application.dataPath}/FmvMaker/NodeGraphs/", "");

            int appPathLen = Application.dataPath.Length;
            string finalPath = graphPath.Substring(appPathLen - 6);

            currentGraph = AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph)) as NodeGraph;
            if (currentGraph != null) {
                NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null) {
                    currentWindow.CurrentNodeGraph = currentGraph;
                }
            } else {
                EditorUtility.DisplayDialog("Graph Load Message", "Unable to load selected graph.", "OK");
            }
        }

        public static void UnloadGraph() {
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if (currentWindow != null) {
                currentWindow.CurrentNodeGraph = null;
            }
        }

        public static void CreateNode(NodeGraph currentGraph, NodeType nodeType, Vector2 mousePosition) {
            if (currentGraph != null) {
                NodeBase currentNode = null;
                switch (nodeType) {
                    case NodeType.Float:
                        currentNode = ScriptableObject.CreateInstance<FloatNode>();
                        currentNode.NodeName = "Float Node";
                        break;
                    case NodeType.Add:
                        break;
                    default:
                        break;
                }

                if (currentNode != null) {
                    currentNode.InitNode();
                    currentNode.NodeRect.x = mousePosition.x;
                    currentNode.NodeRect.y = mousePosition.y;
                    currentNode.ParentGraph = currentGraph;
                    currentGraph.nodes.Add(currentNode);

                    AssetDatabase.AddObjectToAsset(currentNode, currentGraph);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}