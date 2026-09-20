using System.Threading.Tasks;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// Executor for the <see cref="TwoOptionRuntimeNode"/> node.
    /// </summary>
    /// <remarks>
    /// This executor displays a question and two dialogue options to the player,
    /// waits for their selection, and records which option was chosen.
    /// </remarks>
    public class TwoOptionExecutor : IVisualNovelNodeExecutor<TwoOptionRuntimeNode>
    {
        /// <summary>
        /// Executes the <see cref="TwoOptionRuntimeNode"/> node, displaying the dialogue options
        /// and waiting for the player to select one.
        /// </summary>
        public async Task ExecuteAsync(TwoOptionRuntimeNode runtimeNode, VisualNovelDirector ctx)
        {
            // Display the options
            ctx.OptionPanel.SetActive(true);
            ctx.Option1Text.text = runtimeNode.Option1Text;
            ctx.Option2Text.text = runtimeNode.Option2Text;

            // Wait for player to select an option
            var optionSelectionDetected = ctx.InputProvider.OptionSelectionDetected();
            while (!optionSelectionDetected.IsCompleted)
            {
                await Task.Yield();
            }
            runtimeNode.SelectedOption = optionSelectionDetected.Result;
            ctx.OptionPanel.SetActive(false);
        }
    }
}

