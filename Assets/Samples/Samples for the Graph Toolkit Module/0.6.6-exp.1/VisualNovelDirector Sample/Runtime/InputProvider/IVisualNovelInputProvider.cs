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
        
        /// <summary>
        /// This method creates a <see cref="Task{TResult}"/> that monitors for player selection of a dialogue option.
        /// The returned Task can be awaited to coordinate the visual novel flow with player choice interactions,
        /// enabling branching narrative based on the selected option.
        /// </summary>
        /// <returns>
        /// A Task that completes when the player selects a dialogue option. The task result is an integer
        /// representing the index of the selected option (e.g., 0 for Option 1, 1 for Option 2).
        /// </returns>
        Task<int> OptionSelectionDetected();
    }
}
