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
        private float _textureOffset;
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
                _activeTime = 0f;
                _textureOffset = 0f;
                transform.rotation = _defaultRotation;
            }
            _activeTime += _beamProperties.Duration;
        }

        private void Animate()
        {
            _textureOffset += Time.fixedDeltaTime * _beamProperties.Speed;
            _lineRenderer.material.mainTextureOffset = new Vector2(_textureOffset, 0);
        }
        
        private void FixedUpdate()
        {
            if (_activeTime > 0f)
            {
                _activeTime -= GameTime.fixedDelta;
                Animate();
                LaunchRay();
                if (_activeTime < 0.5f)
                {
                    // TODO: alter alpha
                    var gradient = _lineRenderer.colorGradient;
                    gradient.alphaKeys[0].alpha = 0.5f;
                    _lineRenderer.colorGradient = gradient;
                }
            }
            else
            {
                _lineRenderer.positionCount = 0;
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