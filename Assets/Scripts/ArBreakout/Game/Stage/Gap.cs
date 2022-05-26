using ArBreakout.Common;
using ArBreakout.Common.Events;
using ArBreakout.Game.Ball;
using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game.Stage
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
                other.gameObject.GetComponent<BallBehaviour>().IsMissed = true;
                AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Death);
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