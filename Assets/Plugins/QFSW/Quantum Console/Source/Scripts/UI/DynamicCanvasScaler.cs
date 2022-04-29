using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QFSW.QC.UI
{
    [ExecuteInEditMode]
    public class DynamicCanvasScaler : MonoBehaviour
    {
        private float _lastScaler;

        [Range(0.5f, 2f)] [SerializeField] private float _rectMagnification = 1f;

        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920, 1080);

        [SerializeField] private CanvasScaler _scaler;
        [SerializeField] private RectTransform _uiRoot;

        [Range(0.5f, 2f)] [SerializeField] private float _zoomMagnification = 1f;

        public float RectMagnification
        {
            get => _rectMagnification;
            set
            {
                if (value > 0) _rectMagnification = value;
            }
        }

        public float ZoomMagnification
        {
            get => _zoomMagnification;
            set
            {
                if (value > 0) _zoomMagnification = value;
            }
        }

        private float RootScaler => _rectMagnification / _zoomMagnification;

        private void OnEnable()
        {
            _lastScaler = RootScaler;
        }

        private void Update()
        {
            if (_scaler && _uiRoot)
                if (RootScaler != _lastScaler)
                {
                    var rootRect = new Rect(_uiRoot.offsetMin.x / _lastScaler, _uiRoot.offsetMin.y / _lastScaler,
                        _uiRoot.offsetMax.x / _lastScaler, _uiRoot.offsetMax.y / _lastScaler);
                    _lastScaler = RootScaler;

                    _scaler.referenceResolution = _referenceResolution / _zoomMagnification;
                    _uiRoot.offsetMin = new Vector2(rootRect.x, rootRect.y) * RootScaler;
                    _uiRoot.offsetMax = new Vector2(rootRect.width, rootRect.height) * RootScaler;

#if UNITY_EDITOR
                    EditorUtility.SetDirty(_uiRoot);
                    EditorUtility.SetDirty(_scaler);
#endif
                }
        }
    }
}