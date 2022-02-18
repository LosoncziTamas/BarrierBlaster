using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArBreakout.PlaneDetection
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PlaneDetectionHint : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float HintInterval = 0.1f;
#else
        private const float HintInterval = 1.5f;
#endif

        private const float FadeDuration = 0.6f;

        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private string[] _hints;

        private CanvasGroup _root;
        private bool _guideCompleted;
        private int _currentHintIdx;

        public bool GuideCompleted => _guideCompleted;

        private void OnValidate()
        {
            Assert.IsNotNull(_hints);
            Assert.IsTrue(_hints.Length > 0);
        }

        private void Awake()
        {
            _root = GetComponent<CanvasGroup>();
        }

        public void StartGuide()
        {
            _currentHintIdx = 0;
            _hintText.text = _hints[_currentHintIdx];

            var fadeInAndWait = DOTween.Sequence()
                .Append(_root.DOFade(1.0f, FadeDuration))
                .AppendInterval(HintInterval);

            DOTween.Sequence()
                .Append(fadeInAndWait)
                .Append(FadeOutThenFadeIn(_hints[++_currentHintIdx]))
                .AppendInterval(HintInterval)
                .AppendCallback(() => _guideCompleted = true);
        }

        private Sequence FadeOutThenFadeIn(string newText)
        {
            return DOTween.Sequence()
                .Append(_hintText.DOFade(0.0f, FadeDuration))
                .AppendCallback(() => _hintText.text = newText)
                .Append(_hintText.DOFade(1.0f, FadeDuration));
        }

        public void HideGuide()
        {
            _root.DOFade(0.0f, FadeDuration);
        }
    }
}