using System.Threading.Tasks;
using ArBreakout.Common;
using ArBreakout.Common.Tween;
using ArBreakout.Game.Scoring;
using ArBreakout.Levels;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public partial class LevelCompleteModal : MonoBehaviour
    {
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _goToMenuButton;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _cancel;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TextMeshProUGUI _stageText; 
        [SerializeField] private Canvas _canvas; 
        [SerializeField] private RectTransform _shadow; 
        [SerializeField] private RectTransform _modalTrans; 
        [SerializeField] private Image _overlay; 
        
        [SerializeField] private LevelCompleteStar _levelCompleteStar1; 
        [SerializeField] private LevelCompleteStar _levelCompleteStar2; 
        [SerializeField] private LevelCompleteStar _levelCompleteStar3; 
        
        [SerializeField] private PunchScaleProperties _punchScaleProperties;
        [SerializeField] private ShakePositionProperties _shakePositionProperties;
        
        private TaskCompletionSource<Result> _taskCompletionSource;
        private Sequence _showAnimation;
        private Sequence _starAnimation;
        
        public Vector3 HiddenPosition;
        public float AnimDuration;
        public Ease Ease;

        public class Result
        {
            public LevelData Level { get; set; }
            public bool AllLevelsComplete { get; set; }
            public bool GoBackToMenu { get; set; }
        }

        private void Awake()
        {
            _panel.anchoredPosition = HiddenPosition;
            _shadow.anchoredPosition = HiddenPosition;
        }

        private void OnEnable()
        {
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
            _cancel.onClick.AddListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.AddListener(OnGoToMenuButtonClick);
            _replayButton.onClick.AddListener(OnReplayButtonClick);
        }
        
        private void OnDisable()
        {
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.RemoveListener(OnGoToMenuButtonClick);
            _replayButton.onClick.RemoveListener(OnReplayButtonClick);
            _cancel.onClick.RemoveListener(OnNextLevelButtonClick);
        }
        
        public Task<Result> Show(string stageName, StagePerformance stagePerformance)
        {
            Debug.Assert(_taskCompletionSource == null);

            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 0.0f);
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.ModalAppear);
            
            var color = _levelCompleteStar1.FilledStar.color;
            color.a = 0.0f;
            _levelCompleteStar1.FilledStar.color = _levelCompleteStar2.FilledStar.color = _levelCompleteStar3.FilledStar.color = color;
            _levelCompleteStar1.FilledStar.transform.localScale = _levelCompleteStar2.FilledStar.transform.localScale = _levelCompleteStar3.FilledStar.transform.localScale = Vector3.one;

            _showAnimation = CreateShowAnimation();
            _showAnimation.OnComplete(() => AnimateStars(stagePerformance.Stars));
            
            _stageText.text = $"STAGE {stageName}";
            _canvas.enabled = true;
            _taskCompletionSource = new TaskCompletionSource<Result>();
            return _taskCompletionSource.Task;
        }
        
        private void OnGoToMenuButtonClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() => OnHidden(new Result
            {
                Level = null,
                AllLevelsComplete = false,
                GoBackToMenu = true
            }));
        }
        
        private void OnNextLevelButtonClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            var currentLevel = _levels.Selected;
            var currLevelIdx = _levels.All.IndexOf(currentLevel);
            var allLevelComplete = false;
            
            if (_levels.All.Count - 1 == currLevelIdx)
            {
                allLevelComplete = true;
            }
            else
            {
                var nextLevel = _levels.All[currLevelIdx + 1];
                _levels.Selected.Unlocked = true;
                _levels.Selected = nextLevel;
            }

            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() => OnHidden(new Result
            {
                Level = _levels.Selected,
                AllLevelsComplete = allLevelComplete,
                GoBackToMenu = false
            }));
        }
        
        private void OnReplayButtonClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() => OnHidden(new Result
            {
                Level = _levels.Selected,
                AllLevelsComplete = false,
                GoBackToMenu = false
            }));
        }
        
        private void OnHidden(Result result)
        {
            _canvas.enabled = false;
            _taskCompletionSource.SetResult(result);
            _taskCompletionSource = null;
            _showAnimation?.Kill();
            _starAnimation?.Kill();
            _showAnimation = _starAnimation = null;
            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 1.0f);
        }

        private Sequence CreateShowAnimation()
        {
            return DOTween.Sequence()
                .Append(_overlay.DOFade(0.5f, AnimDuration).SetEase(Ease))
                .Insert(0, _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease))
                .Insert(0, _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease));
        }

        private Tween CreateStarFadeTween(Image filledStar)
        {
            var fadeTime = 1.0f;
            return filledStar.DOFade(1.0f, fadeTime).SetEase(_punchScaleProperties.Ease);
        }

        private Tween CreateModalShakeTween()
        {
            return _modalTrans.DOShakePosition(_shakePositionProperties.Duration, _shakePositionProperties.Strength, _shakePositionProperties.Vibrato, _shakePositionProperties.Randomness);
        }

        private Tween CreateStarPunchTween(Image filledStar)
        {
            return filledStar.transform.DOPunchScale(_punchScaleProperties.Punch, _punchScaleProperties.Duration, _punchScaleProperties.Vibrato, _punchScaleProperties.Elasticity).SetEase(_punchScaleProperties.Ease);
        }

        private static void PlayStarSound()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Trophy);
        }
        
        private void AnimateStars(int filledStarCount)
        {
            if (filledStarCount == 0)
            {
                return;
            }
            
            _starAnimation = DOTween.Sequence();
            var timeDiff = _shakePositionProperties.Duration;
            var fadeTime = 0.4f;
            
            // TODO: figure out sounds
            
            if (filledStarCount >= 1)
            {
                _starAnimation.Insert(0, CreateStarFadeTween(_levelCompleteStar1.FilledStar))
                    .Insert(fadeTime, CreateModalShakeTween())
                    .Insert(fadeTime, CreateStarPunchTween(_levelCompleteStar1.FilledStar).OnPlay(PlayStarSound));
            }

            if (filledStarCount >= 2)
            {
                var delay = fadeTime + timeDiff;
                _starAnimation.Insert(delay, CreateStarFadeTween(_levelCompleteStar2.FilledStar))
                    .Insert(2 * fadeTime + timeDiff, CreateModalShakeTween())
                    .Insert(2 * fadeTime + timeDiff, CreateStarPunchTween(_levelCompleteStar2.FilledStar).OnPlay(PlayStarSound));
            }

            if (filledStarCount == 3)
            {
                var delay = 2 * fadeTime + 2 * timeDiff;
                _starAnimation.Insert(delay, CreateStarFadeTween(_levelCompleteStar3.FilledStar))
                    .Insert(3 * fadeTime + 2 * timeDiff, CreateModalShakeTween())
                    .Insert(3 * fadeTime + 2 * timeDiff, CreateStarPunchTween(_levelCompleteStar3.FilledStar).OnPlay(PlayStarSound));
            }
        }
    }
}