using UnityEngine;

namespace ArBreakout.GamePhysics
{
    public class RotateSelf : MonoBehaviour
    {
        [SerializeField] private RotationProperties _rotationProperties;
        
        private void FixedUpdate()
        {
            transform.Rotate(_rotationProperties.rotationAxis, _rotationProperties.rotationValue, Space.Self);
        }
    }
}