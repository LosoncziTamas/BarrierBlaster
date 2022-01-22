using ArBreakout.GamePhysics;
using UnityEngine;

namespace ArBreakout.PowerUps
{
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        public const string GameObjectTag = "Collectable";

        public PowerUp PowerUp { get; private set; }
        
        [SerializeField] private MovementProperties _movementProperties;
        
        private Vector3 _velocity;
        private Vector3 _acceleration;

        private bool _destroyed;

        public void Init(PowerUp powerUp)
        {
            PowerUp = powerUp;
        }

        private void FixedUpdate()
        {
            if (_destroyed)
            {
                return;
            }
            
            _acceleration = Vector3.back;
            _acceleration *= _movementProperties.speed;
            _acceleration += -_movementProperties.drag * _velocity;

            _velocity += BreakoutPhysics.CalculateVelocityDelta(_acceleration);
            transform.localPosition += BreakoutPhysics.CalculateMovementDelta(_acceleration, _velocity);
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                Destroy(gameObject);
            }
            _destroyed = true;
        }
    }
}