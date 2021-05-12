using UnityEngine;

namespace ArBreakout.Game
{
    [RequireComponent(typeof(Collider))]
    public class Bobbing : MonoBehaviour
    {
        private const float Extent = 0.15f;
        private const float Speed = 5.0f;
        private const float Offset = 0.5f;
        
        private float _baseValue;
        private bool _enabled;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void Enable()
        {
            _enabled = true;
            // Apply extra offset to make sure bobbing doesn't interfere with collisions.
            _baseValue = transform.localPosition.z + Offset;
            _collider.enabled = false;

        }

        public void Disable()
        {
            _enabled = false;
            _collider.enabled = true;
        }
        private void FixedUpdate()
        {
            if (_enabled)
            {
                var position = transform.localPosition;
                position = new Vector3(position.x, position.y, _baseValue + Mathf.Sin(Time.time * Speed) * Extent);
                transform.localPosition = position;
            }
        }
    }
}