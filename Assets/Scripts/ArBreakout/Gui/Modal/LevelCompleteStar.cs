using ArBreakout.Misc;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class LevelCompleteStar : MonoBehaviour
    {
        [SerializeField] private Image _filledStar;
        [SerializeField] private PunchScaleProperties _punchScaleProperties;

        private Sequence _tweener;

        private Sequence CreateSequence()
        {
            return DOTween.Sequence()
                .Insert(0, _filledStar.DOFade(1.0f, _punchScaleProperties.Duration))
                .SetEase(_punchScaleProperties.Ease)
                .Insert(_punchScaleProperties.Duration, 
                    _filledStar.transform.DOPunchScale(_punchScaleProperties.Punch, _punchScaleProperties.Duration, _punchScaleProperties.Vibrato, _punchScaleProperties.Elasticity).SetEase(_punchScaleProperties.Ease))
                .SetAutoKill(false);
        }

        private void OnGUI()
        {
            GUILayout.Space(200);
            if (GUILayout.Button("          Star Display"))
            {
                _tweener = CreateSequence();
            }
            if (GUILayout.Button("          rewind"))
            {
                _tweener.Rewind();
            }
        }
    }
}