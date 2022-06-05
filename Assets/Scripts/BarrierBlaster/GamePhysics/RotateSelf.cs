using UnityEngine;

namespace BarrierBlaster.GamePhysics
{
    public class RotateSelf : MonoBehaviour
    {
        [SerializeField] private RotationProperties _rotationProperties;

        private void FixedUpdate()
        {
            transform.Rotate(_rotationProperties.RotationAxis, _rotationProperties.RotationValue, Space.Self);
        }
    }
}