using UnityEngine;

namespace ArBreakout.GamePhysics
{
    [CreateAssetMenu]
    public class RotationProperties : ScriptableObject
    {
        public Vector3 rotationAxis;
        public float rotationValue;
    }
}