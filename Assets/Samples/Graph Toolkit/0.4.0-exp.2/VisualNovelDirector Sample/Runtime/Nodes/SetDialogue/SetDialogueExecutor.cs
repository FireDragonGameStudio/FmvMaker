using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// Executor for the <see cref="SetDialogueRuntimeNode"/> and <see cref="SetDialogueRuntimeNodeWithPreviousActor"/> nodes.
    /// </summary>
    public class SetDialogueExecutor :
        IVisualNovelNodeExecutor<SetDialogueRuntimeNode>,
        IVisualNovelNodeExecutor<SetDialogueRuntimeNodeWithPreviousActor>
    {
        /// <summary>
        /// Executes the <see cref="SetDialogueRuntimeNode"/> node, setting the dialogue text and actor sprite settings.
        /// </summary>
        public async Task ExecuteAsync(SetDialogueRuntimeNode runtimeNode, VisualNovelDirector ctx)
        {
            if (string.IsNullOrEmpty(runtimeNode.DialogueText))
            {
                ctx.DialoguePanel.SetActive(false);
                return;
            }

            ctx.DialoguePanel.SetActive(true);
            ctx.ActorNameText.text = runtimeNode.ActorName;

            foreach (var location in ctx.ActorLocationList)
                location.enabled = false;

            if (runtimeNode.ActorSprite != null)
            {
                var img = ctx.ActorLocationList[runtimeNode.LocationIndex];
                img.enabled = true;
                img.sprite = runtimeNode.ActorSprite;
            }

            await TypeTextWithSkipAsync(runtimeNode.DialogueText, ctx);
        }

        /// <summary>
        /// Executes the <see cref="SetDialogueRuntimeNodeWithPreviousActor"/> node, and keeps all previous actor settings
        /// while changing the dialogue text.
        /// </summary>
        public async Task ExecuteAsync(SetDialogueRuntimeNodeWithPreviousActor runtimeNode, VisualNovelDirector ctx)
        {
            if (string.IsNullOrEmpty(runtimeNode.DialogueText))
            {
                ctx.DialoguePanel.SetActive(false);
                return;
            }

            ctx.DialoguePanel.SetActive(true);

            await TypeTextWithSkipAsync(runtimeNode.DialogueText, ctx);
        }

        /// <summary>
        /// Executes a typewriter effect on the given <see cref="TextMeshProUGUI"/> label.
        /// </summary>
        /// <param name="dialogueText">The text to set</param>
        /// <param name="ctx">The <see cref="VisualNovelDirector"/> context to get settings and input from</param>
        /// <remarks>
        /// Input is used to skip the typewriter effect if it's in-progress.
        /// </remarks>
        static async Task TypeTextWithSkipAsync(string dialogueText, VisualNovelDirector ctx)
        {
            var label = ctx.DialogueText;
            var delayPerCharSeconds = ctx.GlobalTextDelayPerCharacter;
            var inputProvider = ctx.InputProvider;

            label.text = "";
            var builder = new StringBuilder();

            var insideRichTag = false;

            // Start listening for skip input
            var skipInputDetected = inputProvider.InputDetected();

            foreach (var c in dialogueText)
            {
                // Handle rich text tags (e.g., <b>, </i>)
                if (c == '<')
                    insideRichTag = true;

                builder.Append(c);

                if (c == '>')
                    insideRichTag = false;

                // Skip delay if rich text
                if (insideRichTag || char.IsWhiteSpace(c)) continue;

                label.text = builder.ToString();

                var timer = 0f;
                while (timer < delayPerCharSeconds)
                {
                    if (skipInputDetected.IsCompleted)
                    {
                        label.text = dialogueText;
                        return;
                    }
                    timer += Time.deltaTime;
                    await Task.Yield();
                }
            }

            label.text = dialogueText;
        }
    }

}
