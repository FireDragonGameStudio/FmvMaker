using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The runtime representation of a visual novel graph.
    /// </summary>
    /// <remarks>
    /// For the sake of this sample, the visual novel graph is represented as a linear list of nodes with no branching
    /// or loops.
    /// </remarks>
    public class VisualNovelRuntimeGraph : ScriptableObject
    {
        [SerializeReference]
        public List<VisualNovelRuntimeNode> Nodes = new();
    }
}
