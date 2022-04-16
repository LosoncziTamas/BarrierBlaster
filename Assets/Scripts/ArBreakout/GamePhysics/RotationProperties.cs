using UnityEngine;

namespace ArBreakout.GamePhysics
{
    [CreateAssetMenu]
    public class RotationProperties : ScriptableObject
    {
        public Vector3 RotationAxis;
        public float RotationValue;
        public float DefaultRotationValue;
    }
}