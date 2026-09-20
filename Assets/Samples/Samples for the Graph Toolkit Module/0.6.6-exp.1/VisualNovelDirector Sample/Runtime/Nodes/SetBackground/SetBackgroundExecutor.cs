using System.Threading.Tasks;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// Executor for the <see cref="SetBackgroundRuntimeNode"/> node.
    /// </summary>
    public class SetBackgroundExecutor : IVisualNovelNodeExecutor<SetBackgroundRuntimeNode>
    {
        /// <summary>
        /// Sets the background image of the visual novel director context to the specified sprite.
        /// </summary>
        public async Task ExecuteAsync(SetBackgroundRuntimeNode runtimeNode, VisualNovelDirector ctx)
        {
            ctx.BackgroundImage.sprite = runtimeNode.BackgroundSprite;
            await Task.Yield();
        }
    }
}
