using UnityEngine;

namespace BarrierBlaster.Game.Paddle
{
    [CreateAssetMenu]
    public class PaddleProperties : ScriptableObject
    {
        public float DefaultSpeed = 18.0f;
        public float Drag = 2.0f;
        public float WallCollisionBounce = 15.0f;
        public float BallCollisionBounce = 5.0f;
    }
}