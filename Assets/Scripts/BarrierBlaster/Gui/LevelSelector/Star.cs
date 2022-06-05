using UnityEngine;
using UnityEngine.UI;

namespace BarrierBlaster.Gui.LevelSelector
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _filledColor;
        [SerializeField] private Color _emptyColor;

        public void SetFilled(bool filled)
        {
            _image.color = filled ? _filledColor : _emptyColor;
        }
    }
}