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