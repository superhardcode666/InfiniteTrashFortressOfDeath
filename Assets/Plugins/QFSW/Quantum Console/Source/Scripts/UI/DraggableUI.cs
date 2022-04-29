using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace QFSW.QC.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _dragRoot;
        private bool _isDragging;

        private Vector2 _lastPos;
        [SerializeField] private bool _lockInScreen = true;

        [SerializeField] private UnityEvent _onBeginDrag;
        [SerializeField] private UnityEvent _onDrag;
        [SerializeField] private UnityEvent _onEndDrag;
        [SerializeField] private KeyCode[] _requiredKeys = new KeyCode[0];

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = _requiredKeys.All(InputHelper.GetKey);
            if (_isDragging)
            {
                _onBeginDrag.Invoke();
                _lastPos = eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _onEndDrag.Invoke();
            }
        }

        public void LateUpdate()
        {
            if (_isDragging)
            {
                Transform root = _dragRoot;
                if (!root) root = transform as RectTransform;

                Vector2 pos = Input.mousePosition;
                var delta = pos - _lastPos;
                _lastPos = pos;

                if (_lockInScreen)
                {
                    var resolution = new Vector2(Screen.width, Screen.height);
                    if (pos.x <= 0 || pos.x >= resolution.x) delta.x = 0;
                    if (pos.y <= 0 || pos.y >= resolution.y) delta.y = 0;
                }

                root.Translate(delta);
                _onDrag.Invoke();
            }
        }
    }
}