using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraTweenProperties _cameraTweenProperties;

        [UsedImplicitly]
        public void ShakeCamera()
        {
            _camera.DOShakePosition(_cameraTweenProperties.Duration, _cameraTweenProperties.Strength, _cameraTweenProperties.Vibrato, _cameraTweenProperties.Randomness);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("        Shake camera"))
            {
                ShakeCamera();
            }
        }
    }
}