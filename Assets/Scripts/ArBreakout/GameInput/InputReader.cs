using UnityEngine;

namespace ArBreakout.GameInput
{
    public class InputReader : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PointerDetector _leftButton;
        [SerializeField] private PointerDetector _rightButton;
        [SerializeField] private PointerDetector _fireButton;

        private void Update()
        {
            _playerInput.Clear();
            if (_leftButton.PointerDown || Input.GetAxis("Horizontal") < 0)
            {
                _playerInput.Left = true;
            }
            if (_rightButton.PointerDown || Input.GetAxis("Horizontal") > 0)
            {
                _playerInput.Right = true;
            }
            if (_fireButton.PointerDown || Input.GetButton("Jump"))
            {
                _playerInput.Fire = true;
            }
        }
    }
}