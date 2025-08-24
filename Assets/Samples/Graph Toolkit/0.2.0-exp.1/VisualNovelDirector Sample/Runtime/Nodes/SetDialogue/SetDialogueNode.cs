using System;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The serializable data representing a runtime node in the visual novel graph that sets the dialogue text and actor information.
    /// </summary>
    [Serializable]
    public class SetDialogueRuntimeNode : VisualNovelRuntimeNode
    {
        public string ActorName;
        public Sprite ActorSprite;
        public int LocationIndex;
        public string DialogueText;
    }
}
