using System;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The serializable data representing a runtime node in the visual novel graph that sets the background image.
    /// </summary>
    [Serializable]
    public class SetBackgroundRuntimeNode : VisualNovelRuntimeNode
    {
        public Sprite BackgroundSprite;
    }
}
