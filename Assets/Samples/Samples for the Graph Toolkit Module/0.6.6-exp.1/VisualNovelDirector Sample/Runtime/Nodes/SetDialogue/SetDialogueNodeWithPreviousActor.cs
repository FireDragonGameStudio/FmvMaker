using System;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The serializable data that represents a runtime node in the visual novel graph that sets the dialogue text only.
    /// </summary>
    [Serializable]
    public class SetDialogueRuntimeNodeWithPreviousActor : VisualNovelRuntimeNode
    {
        public string DialogueText;
    }
}
