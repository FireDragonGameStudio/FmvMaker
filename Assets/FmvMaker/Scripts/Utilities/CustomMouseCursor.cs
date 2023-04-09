using UnityEngine;
using UnityEngine.EventSystems;

namespace FmvMaker.Utilities {
    public class CustomMouseCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        [SerializeField]
        private Texture2D cursorTexture;

        public void OnPointerClick(PointerEventData eventData) {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}