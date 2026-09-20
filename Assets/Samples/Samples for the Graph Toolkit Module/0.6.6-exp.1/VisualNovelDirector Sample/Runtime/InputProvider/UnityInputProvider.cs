using System.Threading.Tasks;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// Implementation of the <see cref="IVisualNovelInputProvider"/> interface using Unity's Input System if present in project, otherwise uses Unity's Input Manager.
    /// </summary>
    public class UnityInputProvider : MonoBehaviour, IVisualNovelInputProvider
    {
        /// <summary>
        /// A <see cref="TaskCompletionSource{TResult}"/> that is used to signal when the next input event is detected.
        /// <br/><br/>
        /// This is almost like a 'fake' <see cref="Task"/> that lets us use the Unity input systems events to signal when
        /// the next input is detected by marking the task as completed.
        /// <br/><br/>
        /// We're using <see cref="Task"/>s because some nodes can have long-running execution behaviour (e.g. the typewriter effect for dialogue).
        /// </summary>
        private TaskCompletionSource<bool> _nextTcs;
        
        /// <summary>
        /// A <see cref="TaskCompletionSource{TResult}"/> that is used to signal when a dialogue option has been selected by the player.
        /// <br/><br/>
        /// This completion source allows UI buttons or other input mechanisms to communicate the player's choice
        /// to the visual novel execution system. When an option is selected via <see cref="SelectOption"/>,
        /// this task is completed with the index of the selected option.
        /// <br/><br/>
        /// The result is an integer representing the zero-based index of the selected option
        /// (e.g., 0 for the first option, 1 for the second option).
        /// </summary>
        private TaskCompletionSource<int> _optionSelectedTcs;

#if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// An <see cref="InputActionAsset"/> that defines the input actions for the visual novel system.
        /// <br/><br/>
        /// See the 'VisualNovelInput.inputactions' asset and corresponding generated script file 'VisualNovelInput.cs'.
        /// </summary>
        private VisualNovelInput _inputActions;

        /// <summary>
        /// On Awake we set up our input actions and subscribe to input events.
        /// </summary>
        private void Awake()
        {
            _inputActions = new VisualNovelInput();
            if (_inputActions != null)
                _inputActions.Gameplay.Next.performed += OnNextPressed;
        }

        /// <summary>
        /// On Destroy we unsubscribe from input events and dispose of the input actions object.
        /// </summary>
        private void OnDestroy()
        {
            _inputActions.Gameplay.Next.performed -= OnNextPressed;
            _inputActions.Dispose();
        }

        /// <summary>
        /// When the 'Next' input action is performed, we set the <see cref="_nextTcs"/> task as completed.
        /// This <see cref="Task"/> can be awaited on by <see cref="IVisualNovelNodeExecutor{TNode}"/>s.
        /// </summary>
        private void OnNextPressed(InputAction.CallbackContext _) => _nextTcs?.TrySetResult(true);

        /// <summary>
        /// Enables the input actions when the object is enabled.
        /// </summary>
        private void OnEnable() => _inputActions.Enable();

        /// <summary>
        /// Disables the input actions when the object is disabled.
        /// </summary>
        private void OnDisable() => _inputActions.Disable();
#else

        static bool KeyPressed()
        {
            return Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(3) || Input.GetMouseButtonDown(2);
        }

        private void Update()
        {
            if (KeyPressed())
            {
                OnNextPressed();
            }
        }

        /// <summary>
        /// When the 'Next' input action is performed, we set the <see cref="_nextTcs"/> task as completed.
        /// This <see cref="Task"/> can be awaited on by <see cref="IVisualNovelNodeExecutor{TNode}"/>s.
        /// </summary>
        private void OnNextPressed() => _nextTcs?.TrySetResult(true);
#endif
        /// <summary>
        /// Creates a <see cref="TaskCompletionSource{TResult}"/> to wait for the next input event.
        /// <br/><br/>
        /// If there is already a <see cref="TaskCompletionSource{TResult}"/> created and it's not already completed,
        /// we just return it. This allows nodes to wait for the next input event without mistakenly waiting for input
        /// more than once.
        /// </summary>
        public Task InputDetected()
        {
            if (_nextTcs == null || _nextTcs.Task.IsCompleted)
            {
                _nextTcs = new TaskCompletionSource<bool>();
            }

            return _nextTcs.Task;
        }

        /// <summary>
        /// Signals that a dialogue option has been selected by the player.
        /// <br/><br/>
        /// This method is typically called by UI button click handlers or other input mechanisms
        /// to notify the visual novel system that the player has made a choice. The method completes
        /// the <see cref="_optionSelectedTcs"/> task with the provided option index, allowing awaiting
        /// executors to proceed with the selected branch of the narrative.
        /// </summary>
        /// <param name="option">
        /// The zero-based index of the selected option (e.g., 0 for Option 1, 1 for Option 2).
        /// This value is passed to the awaiting executor to determine which narrative branch to follow.
        /// </param>
        public void SelectOption(int option)
        {
            _optionSelectedTcs?.TrySetResult(option);
        }
        
        /// <summary>
        /// Creates a <see cref="TaskCompletionSource{TResult}"/> to wait for a dialogue option selection event.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that completes when the player selects an option via <see cref="SelectOption"/>.
        /// The task result is an integer representing the zero-based index of the selected option.
        /// </returns>
        public Task<int> OptionSelectionDetected()
        {
            if (_optionSelectedTcs == null || _optionSelectedTcs.Task.IsCompleted)
            {
                _optionSelectedTcs = new TaskCompletionSource<int>();
            }

            return _optionSelectedTcs.Task;
        }
    }
}