using ArBreakout.Misc;
using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game
{
    public class Gap : MonoBehaviour
    {
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private GameEvent _ballMissedEvent;
        
        private void Awake()
        {
            _gameEntities.Add(this);
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(BallBehaviour.GameObjectTag))
            {
                _ballMissedEvent.Raise();
            }
            else if (other.gameObject.CompareTag(Collectable.GameObjectTag))
            {
                var collectable = other.gameObject.GetComponentInParent<Collectable>();
                collectable.Destroy();
            }
        }
    }
}