using ArBreakout.Game;
using UnityEngine;

namespace ArBreakout.GamePhysics
{
    public class RaycastReflection : MonoBehaviour
    {
        public int reflections;
        public float maxLength;
        public float minAngle;
        public float maxAngle;
        public float maxTime;

        [SerializeField] private LineRenderer _lineRenderer;
        
        private Ray _ray;
        private RaycastHit _hit;
        private Vector3 _direction;
        private float _multiplier = 1.0f;
        
        private float _accumulator;
        

        private void Update()
        {
            _accumulator += Time.deltaTime;
            var t = _accumulator / maxTime;
            if (t > 1.0f)
            {
                t = 0;
                _accumulator = 0;
            }

            Transform transform1;
            (transform1 = transform).rotation = Quaternion.Euler(0f, Mathf.Lerp(minAngle, maxAngle, t), 0f);
            _ray = new Ray(transform1.position, transform1.forward);

            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(0, transform1.position);
            var remainingLength = maxLength;

            for (var i = 0; i < reflections; i++)
            {
                if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength))
                {
                    var positionCount = _lineRenderer.positionCount;
                    positionCount += 1;
                    _lineRenderer.positionCount = positionCount;
                    _lineRenderer.SetPosition(positionCount - 1, _hit.point);
                    remainingLength -= Vector3.Distance(_ray.origin, _hit.point);
                    _ray = new Ray(_hit.point - _ray.direction * 0.01f, Vector3.Reflect(_ray.direction, _hit.normal));
                    var hitCollider = _hit.collider;
                    if (hitCollider.CompareTag("Brick"))
                    {
                        hitCollider.GetComponent<BrickBehaviour>().Smash();
                    }
                    else if (!hitCollider.CompareTag("Brick") && !hitCollider.CompareTag("Wall"))
                    {
                        break;
                    }
                }
                else
                {
                    var positionCount = _lineRenderer.positionCount;
                    positionCount += 1;
                    _lineRenderer.positionCount = positionCount;
                    _lineRenderer.SetPosition(positionCount - 1, _ray.origin + _ray.direction * remainingLength);
                }
            }
        }
    }
}