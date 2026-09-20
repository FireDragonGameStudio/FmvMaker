using System;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The serializable data representing a runtime node in the visual novel graph 
    /// that presents two dialogue options to the player.
    /// </summary>
    [Serializable]
    public class TwoOptionRuntimeNode : VisualNovelRuntimeNode
    {
        public const string IN_PORT_OPTION1_NAME = "Option1";
        public const string IN_PORT_OPTION2_NAME = "Option2";
        public const string OUT_PORT_OPTION1_NAME = "Option1Execution";
        public const string OUT_PORT_OPTION2_NAME = "Option2Execution";

        public string Option1Text;
        public string Option2Text;
        
        /// <summary>
        /// The index of the node to execute if Option 1 is selected.
        /// -1 indicates no continuation for this branch.
        /// </summary>
        public int Option1NextNodeIndex = -1;
        
        /// <summary>
        /// The index of the node to execute if Option 2 is selected.
        /// -1 indicates no continuation for this branch.
        /// </summary>
        public int Option2NextNodeIndex = -1;
        
        /// <summary>
        /// Records which option the player selected (0 for option 1, 1 for option 2).
        /// Set during runtime execution.
        /// </summary>
        public int SelectedOption;
        
        public Hash128 NextWireOption1DestinationId;
        public Hash128 NextWireOption1SourceId;
        public Hash128 NextWireOption2DestinationId;
        public Hash128 NextWireOption2SourceId;
    }
}

