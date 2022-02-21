using System;
using UnityEngine;

namespace ArBreakout.Game
{
    public class Gap : MonoBehaviour
    {
        public class BallHasLeftTheGameArgs : EventArgs
        {
        }

        public static event EventHandler<BallHasLeftTheGameArgs> BallHasLeftTheGameEvent;

        [SerializeField] private GameEntities _gameEntities;


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
                var eventHandler = BallHasLeftTheGameEvent;
                eventHandler?.Invoke(this, new BallHasLeftTheGameArgs());
            }
        }
    }
}