using FmvMaker.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FmvMaker.Core {
    public class FmvMakerRuntimeGraph : ScriptableObject {
        public string EntryNodeId;
        public List<FmvMakerNode> AllFmvMakerNodes = new();
    }
}