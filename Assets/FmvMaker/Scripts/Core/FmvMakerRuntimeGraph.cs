using FmvMaker.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Core {
    public class FmvMakerRuntimeGraph : ScriptableObject {
        public Hash128 GraphId;
        public Hash128 EntryNodeId;
        // ID of the Start node's "out" port, used to visualize the entry wire
        public Hash128 EntryOutputPortId;
        public List<FmvMakerNode> AllFmvMakerNodes = new();
    }
}