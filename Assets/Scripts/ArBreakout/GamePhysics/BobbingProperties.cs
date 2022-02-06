using UnityEngine;

namespace ArBreakout.GamePhysics
{
    [CreateAssetMenu]
    public class BobbingProperties : ScriptableObject
    {
        public float extent;
        public float speed;
        public float startOffsetZ;
        public Vector3 rotationAxis;
        public float rotationValue;
    }
}