using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace NekomimiDaimao
{
    public class HolePlaceManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _mainCamera;

        [SerializeField]
        private ARRaycastManager _raycastManager;

        private bool _holePlaced = false;

        public bool HolePlaced
        {
            get => _holePlaced;
            set
            {
                if (_holePlaced == value)
                {
                    return;
                }

                _holePlaced = value;
                _otherWorld.SetActive(value);
                _holeCanvas.gameObject.SetActive(value);
                _holeCamera.gameObject.SetActive(value);
            }
        }

        [SerializeField]
        private GameObject _otherWorld;

        [SerializeField]
        private Transform _holeCamera;

        private readonly Vector3 AppendHeight = Vector3.up * 1.5f;

        [SerializeField]
        private Transform _holeCanvas;

        private readonly List<ARRaycastHit> _hitResults = new List<ARRaycastHit>();

        private void Start()
        {
            HolePlaced = false;
        }

        private void Update()
        {
            if (HolePlaced)
            {
                _holeCamera.SetPositionAndRotation(
                    _mainCamera.position + AppendHeight,
                    _mainCamera.rotation
                );
            }

            PlaceHole();
        }

        private void PlaceHole()
        {
            if (Input.touchCount < 1)
            {
                return;
            }

            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
            {
                return;
            }

            _hitResults.Clear();
            if (!_raycastManager.Raycast(touch.position, _hitResults, TrackableType.Planes))
            {
                return;
            }

            var p = _hitResults[0].pose;
            _holeCanvas.SetPositionAndRotation(p.position, p.rotation);
            // 壁の場合, transform.upがnormalになる
            var normal = _holeCanvas.up;
            _holeCanvas.forward = normal;
            // eularAngleのxz成分を0にすることで穴の向きを合わせる
            var euler = _holeCanvas.eulerAngles;
            _holeCanvas.eulerAngles = new Vector3(0f, euler.y, 0f);
            HolePlaced = true;
        }
    }
}
