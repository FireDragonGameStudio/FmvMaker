using System.Threading.Tasks;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The executor for the <see cref="WaitForInputRuntimeNode"/> node.
    /// </summary>
    public class WaitForInputExecutor : IVisualNovelNodeExecutor<WaitForInputRuntimeNode>
    {
        /// <summary>
        /// Asynchronously waits for user input to be detected before proceeding with the execution of the visual novel graph.
        /// </summary>
        public async Task ExecuteAsync(WaitForInputRuntimeNode _, VisualNovelDirector ctx)
        {
            await ctx.InputProvider.InputDetected();
        }
    }
}
