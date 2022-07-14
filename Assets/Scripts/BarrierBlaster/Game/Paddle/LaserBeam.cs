using BarrierBlaster.Common;
using BarrierBlaster.Game.Bricks;
using BarrierBlaster.Game.Obstacles;
using UnityEngine;

namespace BarrierBlaster.Game.Paddle
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private LaserBeamProperties _beamProperties;
        [SerializeField] private LineRenderer _lineRenderer;

        private float _activeTime;
        private float _textureOffset;
        private Gradient _originalGradient;

        public bool Launching => _activeTime > 0f;
        
        private void Awake()
        {
            _originalGradient = _lineRenderer.colorGradient;
        }

        public void BeginLaunching()
        {
            if (Mathf.Approximately(_activeTime, 0) || _activeTime < 0)
            {
                _activeTime = 0f;
                _textureOffset = 0f;
            }
            _activeTime += _beamProperties.Duration;
            _lineRenderer.colorGradient = _originalGradient;
        }

        public void EndLaunching()
        {
            _activeTime = 0;
            _lineRenderer.positionCount = 0;
            _textureOffset = 0f;
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
                _activeTime -= GameTime.FixedDelta;
                Animate();
                LaunchRay();
                if (_activeTime < 0.5f)
                {
                    var gradient = _lineRenderer.colorGradient;
                    var alphaKeys = gradient.alphaKeys;
                    alphaKeys[0] = new GradientAlphaKey(_activeTime, 0.0f);
                    gradient.SetKeys(
                        gradient.colorKeys,
                        alphaKeys
                    );
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

            var length = _beamProperties.Length;
            if (Physics.Raycast(ray.origin, ray.direction, out var hit, _beamProperties.Length))
            {
                var hitCollider = hit.collider;
                if (hitCollider.CompareTag(BrickBehaviour.GameObjectTag))
                {
                    hitCollider.GetComponent<BrickBehaviour>().Smash(times: 1);
                    length = hit.distance;
                }
                if (hitCollider.CompareTag(Obstacle.Tag))
                {
                    length = hit.distance;
                }
            }
            _lineRenderer.SetPosition(1, ray.origin + ray.direction * length);
        }

        private void OnDestroy()
        {
            AudioPlayer.Instance.StopSound(AudioPlayer.SoundType.Laser);
        }
    }
}