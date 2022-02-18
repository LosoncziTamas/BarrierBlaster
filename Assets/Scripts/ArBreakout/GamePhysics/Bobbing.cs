using UnityEngine;

namespace ArBreakout.GamePhysics
{
    [RequireComponent(typeof(Collider))]
    public class Bobbing : MonoBehaviour
    {
        [SerializeField] private BobbingProperties _bobbingProperties;

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
            _baseValue = transform.localPosition.z + _bobbingProperties.startOffsetZ;
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
                position = new Vector3(position.x, position.y,
                    _baseValue + Mathf.Sin(Time.time * _bobbingProperties.speed) * _bobbingProperties.extent);
                transform.localPosition = position;
            }
        }
    }
}