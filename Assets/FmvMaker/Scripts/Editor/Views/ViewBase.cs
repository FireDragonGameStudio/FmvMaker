using System;
using UnityEngine;

namespace Assets.FmvMaker.Scripts.Editor.Views {
    [Serializable]
    public class ViewBase {

        public string ViewTitle { get; private set; }
        public Rect ViewRect { get; set; }

        protected GUISkin viewSkin;

        public ViewBase(string title) {
            ViewTitle = title;
        }

        public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e) {
            ViewRect = new Rect(
                editorRect.x * percentageRect.x,
                editorRect.y * percentageRect.y,
                editorRect.width * percentageRect.width,
                editorRect.height * percentageRect.height);
        }

        public virtual void ProcessEvents(Event e) {

        }

        protected void GetEditorSkin() {

        }
    }
}