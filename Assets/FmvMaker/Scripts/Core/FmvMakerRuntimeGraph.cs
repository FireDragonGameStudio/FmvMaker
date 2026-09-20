using FmvMaker.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Core {
    public class FmvMakerRuntimeGraph : ScriptableObject {
        public Hash128 GraphId;
        public Hash128 EntryNodeId;
        public List<FmvMakerNode> AllFmvMakerNodes = new();
    }
}