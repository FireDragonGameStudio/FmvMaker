using System.Threading.Tasks;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// An interface that abstracts the input required by the visual novel system.
    /// </summary>
    public interface IVisualNovelInputProvider
    {
        /// <summary>
        /// This method creates a <see cref="Task"/> that monitors for input to advance the visual novel.
        /// The returned Task can be awaited to coordinate the visual novel flow with
        /// user interactions, allowing for proper sequencing with other async operations like
        /// the typewriter effect in <see cref="SetDialogueExecutor"/>.
        /// </summary>
        /// <returns>
        /// A Task that completes when the user provides the necessary input (such as clicking,
        /// pressing space/enter, etc.) to progress to the next step in the visual novel.
        /// </returns>
        Task InputDetected();
    }
}
