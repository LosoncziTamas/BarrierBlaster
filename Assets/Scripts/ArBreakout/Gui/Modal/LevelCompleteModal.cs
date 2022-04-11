using System.Threading.Tasks;
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
        [SerializeField] private Image _overlay; 
        
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
                await Show("I", new StagePerformance{Stars = 1});
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

        public Task<Result> Show(string stageName, StagePerformance stagePerformance)
        {
            Debug.Assert(_taskCompletionSource == null);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            _shadow.DOLocalMove(Vector3.down * 20.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _stageText.text = $"STAGE {stageName}";
            _canvas.enabled = true;
            _taskCompletionSource = new TaskCompletionSource<Result>();
            return _taskCompletionSource.Task;
        }
    }
}