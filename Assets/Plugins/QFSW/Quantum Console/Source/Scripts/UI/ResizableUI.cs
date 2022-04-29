using UnityEngine;
using UnityEngine.EventSystems;

namespace QFSW.QC.UI
{
    [DisallowMultipleComponent]
    public class ResizableUI : MonoBehaviour, IDragHandler
    {
        [SerializeField] private bool _lockInScreen = true;
        [SerializeField] private Vector2 _minSize;
        [SerializeField] private Canvas _resizeCanvas;
        [SerializeField] private RectTransform _resizeRoot;

        public void OnDrag(PointerEventData eventData)
        {
            var minBounds = (_resizeRoot.offsetMin + _minSize) * _resizeCanvas.scaleFactor;
            var maxBounds = _lockInScreen
                ? new Vector2(Screen.width, Screen.height)
                : new Vector2(Mathf.Infinity, Mathf.Infinity);

            var delta = eventData.delta;
            var posCurrent = eventData.position;
            var posLast = posCurrent - delta;

            var posCurrentBounded = new Vector2(
                Mathf.Clamp(posCurrent.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(posCurrent.y, minBounds.y, maxBounds.y)
            );

            var posLastBounded = new Vector2(
                Mathf.Clamp(posLast.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(posLast.y, minBounds.y, maxBounds.y)
            );

            var deltaBounded = posCurrentBounded - posLastBounded;

            _resizeRoot.offsetMax += deltaBounded / _resizeCanvas.scaleFactor;
        }
    }
}