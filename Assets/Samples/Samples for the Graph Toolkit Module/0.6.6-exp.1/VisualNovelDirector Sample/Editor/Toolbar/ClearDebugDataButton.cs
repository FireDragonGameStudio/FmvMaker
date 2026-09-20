using Unity.GraphToolkit.Editor;
using Unity.GraphToolkit.Editor.GraphVisualization;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    [GraphToolbarElement(ButtonID, typeof(VisualNovelDirectorGraph))]
    class ClearDebugDataButton : EditorToolbarButton, IAccessContainerWindow
    {
        const string ButtonID = "VisualNovelDirector/ClearDebugData";

        public EditorWindow containerWindow { get; set; }

        public ClearDebugDataButton()
        {
            text = "Clear Debug Data";
            clicked += OnClicked;
            
            SetVisibility();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnClicked()
        {
            if (containerWindow is not IGraphWindow graphWindow)
                return;
            
            var context = Registry.GetActiveContext(graphWindow.Graph.ID);
            context?.ClearAllVisualization();
        }
        
        void SetVisibility()
        {
            // The debug view is only relevant in play mode, so we hide it otherwise.
            style.display = EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            SetVisibility();
        }
    }
}