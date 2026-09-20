using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The base class for the runtime data for all visual novel nodes.
    /// </summary>
    [Serializable]
    public abstract class VisualNovelRuntimeNode
    {
        public Hash128 NextWireDestinationId;
        public Hash128 NextWireSourceId;
        
        /// <summary>
        /// The index of the next node to execute in the runtime graph.
        /// -1 indicates no next node (end of execution).
        /// For branching nodes, this represents the default path.
        /// </summary>
        public int NextNodeIndex = -1;
        
        /// <summary>
        /// The IDs of the input for this node, stored in display order.
        /// </summary>
        public List<Hash128> InputPortIDs = new();
        
        /// <summary>
        /// The names of the input ports for this node, stored in display order. Parallel to <see cref="InputPortIDs"/>.
        /// </summary>
        public List<string> InputPortNames = new();

        /// <summary>
        /// The IDs of the output for this node, stored in display order.
        /// </summary>
        public List<Hash128> OutputPortIDs = new();

        /// <summary>
        /// The names of the output ports for this node, stored in display order. Parallel to <see cref="OutputPortIDs"/>.
        /// </summary>
        public List<string> OutputPortNames = new();

        /// <summary>
        /// Gets the ID of the input port with the specified name.
        /// </summary>
        /// <param name="name">The name of the port.</param>
        /// <returns>The ID of the port.</returns>
        public Hash128 GetInputPortID(string name)
        {
            var index = InputPortNames.IndexOf(name);
            return index < 0 ? throw new ArgumentException($"No input port named '{name}' found on this node.") : InputPortIDs[index];
        }

        /// <summary>
        /// Gets the ID of the output port with the specified name.
        /// </summary>
        /// <param name="name">The name of the port.</param>
        /// <returns>The ID of the port.</returns>
        public Hash128 GetOutputPortID(string name)
        {
            var index = OutputPortNames.IndexOf(name);
            return index < 0 ? throw new ArgumentException($"No output port named '{name}' found on this node.") : OutputPortIDs[index];
        }
    }
}
