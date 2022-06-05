using UnityEngine;
using UnityEngine.UI;

namespace BarrierBlaster.Gui.Modal
{
    public class LevelCompleteStar : MonoBehaviour
    {
        [SerializeField] private Image _filledStar;
        
        public Image FilledStar => _filledStar;
    }
}