using Unity.GraphToolkit.Editor;
using Unity.GraphToolkit.Editor.GraphVisualization;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;
using UnityEditor;

namespace Unity.GraphToolkit.Samples.VisualNovelDirector.Editor
{
    [GraphToolbarElement("DebugViewToggle", typeof(VisualNovelDirectorGraph))]
    public class DebugViewToggle : EditorToolbarToggle, IAccessContainerWindow
    {
        public EditorWindow containerWindow { get; set; }
        
        public DebugViewToggle()
        {
            text = "Debug View";
            SetValueWithoutNotify(true);
            this.RegisterValueChangedCallback(OnValueChanged);
            
            SetVisibility();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnValueChanged(ChangeEvent<bool> evt)
        {
            if (containerWindow is IGraphWindow graphWindow)
            {
                var context = Registry.GetActiveContext(graphWindow.Graph.ID);
                if (context == null)
                    return;

                context.NodeCustomizationEnabled = evt.newValue;
                context.PortPreviewEnabled = evt.newValue;
            }
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