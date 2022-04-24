using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui
{
    public class DualStateButton : MonoBehaviour
    {
        [SerializeField] private Image _imageOn;
        [SerializeField] private Image _imageOff;
        [SerializeField] private Button _button;

        public Button.ButtonClickedEvent OnClick => _button.onClick;

        public void SetState(bool on)
        {
            _imageOn.gameObject.SetActive(on);
            _imageOff.gameObject.SetActive(!on);
        }
    }
}