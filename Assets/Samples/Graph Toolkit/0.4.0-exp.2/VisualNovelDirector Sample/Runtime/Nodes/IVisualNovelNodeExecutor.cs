using System.Threading.Tasks;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// The interface for an executor of a visual novel node.
    /// <br/><br/>
    /// An executor defines the runtime behaviour for the data stored in a <see cref="VisualNovelRuntimeNode"/>. Each
    /// <see cref="VisualNovelRuntimeNode"/> will have its own implementation of an executor.
    /// </summary>
    /// <remarks>
    /// If we want to share runtime behaviour between nodes, a single class can implement multiple node executors and have
    /// functions for shared behaviour. This is useful for nodes that have similar behaviour but different data.
    /// We can see an example of this in the <see cref="SetDialogueExecutor"/> class, which implements
    /// <see cref="IVisualNovelNodeExecutor{TNode}"/> for both <see cref="SetDialogueRuntimeNode"/> and <see cref="SetDialogueRuntimeNodeWithPreviousActor"/>
    /// </remarks>
    /// <typeparam name="TNode">The type of <see cref="VisualNovelRuntimeNode"/> to execute</typeparam>
    public interface IVisualNovelNodeExecutor<in TNode> where TNode : VisualNovelRuntimeNode
    {
        Task ExecuteAsync(TNode node, VisualNovelDirector ctx);
    }
}
