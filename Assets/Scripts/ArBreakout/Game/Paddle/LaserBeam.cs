using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private LaserBeamProperties _beamProperties;
        [SerializeField] private LineRenderer _lineRenderer;
        
        private void FixedUpdate()
        {
            // TODO: add movement
            LaunchRay();
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