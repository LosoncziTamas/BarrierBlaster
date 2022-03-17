using System;
using ArBreakout.Misc;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private LaserBeamProperties _beamProperties;
        [SerializeField] private LineRenderer _lineRenderer;

        private float _activeTime;
        private float _degreeSign;
        private Quaternion _defaultRotation;

        private void OnGUI()
        {
            if (GUILayout.Button("Launch Beam"))
            {
                BeginLaunching();
            }
        }

        private void Awake()
        {
            _defaultRotation = transform.rotation;
        }

        public void BeginLaunching()
        {
            if (Mathf.Approximately(_activeTime, 0) || _activeTime < 0)
            {
                _activeTime = 0;
                _degreeSign = 1.0f;
                transform.rotation = _defaultRotation;
            }
            _activeTime += _beamProperties.Duration;
        }

        private void Rotate()
        {
            var currentAngle = transform.rotation.eulerAngles.y;
            if (currentAngle >= _beamProperties.MaxAngle)
            {
                _degreeSign = -1.0f;
            }
            else if (currentAngle <= _beamProperties.MinAngle)
            {
                _degreeSign = 1.0f;
            }
            transform.Rotate(Quaternion.Euler(0, _degreeSign * _beamProperties.RotationDegree, 0).eulerAngles);
        }
        
        private void FixedUpdate()
        {
            if (_activeTime > 0f)
            {
                _activeTime -= GameTime.fixedDelta;
                Rotate();
                LaunchRay();
            }
        }

        private void LaunchRay()
        {
            var cachedTransform = transform;
            var startPos = cachedTransform.position;
            var ray = new Ray(startPos, cachedTransform.forward);
            
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, startPos);

            if (Physics.Raycast(ray.origin, ray.direction, out var hit, _beamProperties.Length))
            {
                _lineRenderer.SetPosition(1, hit.point);
                
                var hitCollider = hit.collider;
                if (hitCollider.CompareTag(BrickBehaviour.GameObjectTag))
                {
                    hitCollider.GetComponent<BrickBehaviour>().Smash();
                }
            }
            else
            {
                _lineRenderer.SetPosition(1, ray.origin + ray.direction * _beamProperties.Length);
            }
        }
    }
}