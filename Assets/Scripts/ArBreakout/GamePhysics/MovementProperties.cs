using UnityEngine;

namespace ArBreakout.GamePhysics
{
    [CreateAssetMenu]
    public class MovementProperties : ScriptableObject
    {
        public float speed;
        public float drag;
        public float rotation;
    }
}