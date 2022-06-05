using UnityEngine;

namespace BarrierBlaster.GamePhysics
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

        public void Enable(bool addOffset = true)
        {
            _enabled = true;
            // Apply extra offset to make sure bobbing doesn't interfere with collisions.
            _baseValue = transform.localPosition.z + (addOffset ? _bobbingProperties.startOffsetZ : 0.0f);
            _collider.enabled = false;
            _accumulator = 0;
        }

        public void Disable()
        {
            _enabled = false;
            _collider.enabled = true;
        }

        private float _accumulator;
        
        private void FixedUpdate()
        {
            if (_enabled)
            {
                _accumulator += Time.fixedDeltaTime;
                var position = transform.localPosition;
                position = new Vector3(position.x, position.y, _baseValue + Mathf.Sin(_accumulator * _bobbingProperties.speed) * _bobbingProperties.extent);
                transform.localPosition = position;
            }
        }
    }
}