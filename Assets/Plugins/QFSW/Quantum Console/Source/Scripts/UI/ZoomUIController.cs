using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC.UI
{
    [ExecuteInEditMode]
    public class ZoomUIController : MonoBehaviour
    {
        private float _lastZoom = -1;
        [SerializeField] private float _maxZoom = 2f;
        [SerializeField] private float _minZoom = 0.1f;

        [SerializeField] private DynamicCanvasScaler _scaler;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private Button _zoomDownBtn;
        [SerializeField] private float _zoomIncrement = 0.1f;
        [SerializeField] private Button _zoomUpBtn;

        private float ClampAndSnapZoom(float zoom)
        {
            var clampedZoom = Mathf.Min(_maxZoom, Mathf.Max(_minZoom, zoom));
            var snappedZoom = Mathf.Round(clampedZoom / _zoomIncrement) * _zoomIncrement;
            return snappedZoom;
        }

        public void ZoomUp()
        {
            _scaler.ZoomMagnification = ClampAndSnapZoom(_scaler.ZoomMagnification + _zoomIncrement);
        }

        public void ZoomDown()
        {
            _scaler.ZoomMagnification = ClampAndSnapZoom(_scaler.ZoomMagnification - _zoomIncrement);
        }

        private void LateUpdate()
        {
            if (_scaler && _text)
            {
                var zoom = _scaler.ZoomMagnification;
                if (zoom != _lastZoom)
                {
                    _lastZoom = zoom;

                    var percentage = Mathf.RoundToInt(100 * zoom);
                    _text.text = $"{percentage}%";
                }
            }

            if (_zoomDownBtn) _zoomDownBtn.interactable = _lastZoom > _minZoom;

            if (_zoomUpBtn) _zoomUpBtn.interactable = _lastZoom < _maxZoom;
        }
    }
}