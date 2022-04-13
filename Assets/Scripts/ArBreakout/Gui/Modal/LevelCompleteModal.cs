using System.Threading.Tasks;
using ArBreakout.Common.Tween;
using ArBreakout.Game.Scoring;
using ArBreakout.Levels;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class LevelCompleteModal : MonoBehaviour
    {
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _goToMenuButton;
        [SerializeField] private Button _replayButton;
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
            _closeButton.onClick.AddListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.AddListener(OnGoToMenuButtonClick);
            _replayButton.onClick.AddListener(OnReplayButtonClick);
        }
        
        private void OnDisable()
        {
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
            _closeButton.onClick.RemoveListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.RemoveListener(OnGoToMenuButtonClick);
            _replayButton.onClick.RemoveListener(OnReplayButtonClick);
        }
        
        private void OnGoToMenuButtonClick()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                _canvas.enabled = false;
                _taskCompletionSource.SetResult(new Result
                {
                    Level = null,
                    AllLevelsComplete = false,
                    GoBackToMenu = true
                });
                _taskCompletionSource = null;
            });
        }

        private void OnNextLevelButtonClick()
        {
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
                _levels.Selected = nextLevel;
            }

            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                _canvas.enabled = false;
                _taskCompletionSource.SetResult(new Result
                {
                    Level = _levels.Selected,
                    AllLevelsComplete = allLevelComplete,
                    GoBackToMenu = false
                });
                _taskCompletionSource = null;
            });
        }

        private async void OnGUI()
        {
            if (GUILayout.Button("Show"))
            {
                await Show("I", new StagePerformance{Stars = 3});
            }
        }

        private void OnReplayButtonClick()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                _canvas.enabled = false;
                _taskCompletionSource.SetResult(new Result
                {
                    Level = _levels.Selected,
                    AllLevelsComplete = false,
                    GoBackToMenu = false
                });
                _taskCompletionSource = null;
            });
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

        private Tween CreateModalShaleTween()
        {
            return _modalTrans.DOShakePosition(_shakePositionProperties.Duration, _shakePositionProperties.Strength, _shakePositionProperties.Vibrato, _shakePositionProperties.Randomness);
        }

        private Tween CreateStarPunchTween(Image filledStar)
        {
            return filledStar.transform.DOPunchScale(_punchScaleProperties.Punch, _punchScaleProperties.Duration, _punchScaleProperties.Vibrato, _punchScaleProperties.Elasticity).SetEase(_punchScaleProperties.Ease);
        }
        
        private void AnimateStars(int filledStarCount)
        {
            if (filledStarCount == 0)
            {
                return;
            }
            
            var sequence = DOTween.Sequence();
            var timeDiff = _shakePositionProperties.Duration;
            var fadeTime = 0.4f;
            
            if (filledStarCount >= 1)
            {
                sequence.Insert(0, CreateStarFadeTween(_levelCompleteStar1.FilledStar))
                    .Insert(fadeTime, CreateModalShaleTween())
                    .Insert(fadeTime, CreateStarPunchTween(_levelCompleteStar1.FilledStar));
            }

            if (filledStarCount >= 2)
            {
                sequence.Insert(fadeTime + timeDiff, CreateStarFadeTween(_levelCompleteStar2.FilledStar))
                    .Insert(2 * fadeTime + timeDiff, CreateModalShaleTween())
                    .Insert(2 * fadeTime + timeDiff, CreateStarPunchTween(_levelCompleteStar2.FilledStar));
            }

            if (filledStarCount == 3)
            {
                sequence.Insert(2 * fadeTime + 2 * timeDiff, CreateStarFadeTween(_levelCompleteStar3.FilledStar))
                    .Insert(3 * fadeTime + 2 * timeDiff, CreateModalShaleTween())
                    .Insert(3 * fadeTime + 2 * timeDiff, CreateStarPunchTween(_levelCompleteStar3.FilledStar));
            }
        }
        
        public Task<Result> Show(string stageName, StagePerformance stagePerformance)
        {
            Debug.Assert(_taskCompletionSource == null);

            var color = _levelCompleteStar1.FilledStar.color;
            color.a = 0.0f;
            _levelCompleteStar1.FilledStar.color = _levelCompleteStar2.FilledStar.color = _levelCompleteStar3.FilledStar.color = color;

            var show = CreateShowAnimation();
            show.OnComplete(() => AnimateStars(stagePerformance.Stars));
            
            _stageText.text = $"STAGE {stageName}";
            _canvas.enabled = true;
            _taskCompletionSource = new TaskCompletionSource<Result>();
            return _taskCompletionSource.Task;
        }
    }
}