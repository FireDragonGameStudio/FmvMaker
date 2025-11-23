using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector
{
    /// <summary>
    /// Implementation of the <see cref="IVisualNovelInputProvider"/> interface using Unity's Input System.
    /// </summary>
    public class UnityInputProvider : MonoBehaviour, IVisualNovelInputProvider
    {
        /// <summary>
        /// An <see cref="InputActionAsset"/> that defines the input actions for the visual novel system.
        /// <br/><br/>
        /// See the 'VisualNovelInput.inputactions' asset and corresponding generated script file 'VisualNovelInput.cs'.
        /// </summary>
        private VisualNovelInput _inputActions;

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
    }
}
