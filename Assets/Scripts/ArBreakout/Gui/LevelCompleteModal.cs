using System.Threading.Tasks;
using ArBreakout.Levels;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui
{
    public class LevelCompleteModal : MonoBehaviour
    {
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _goToMenuButton;
        [SerializeField] private RectTransform _panel;
        
        private TaskCompletionSource<Result> _taskCompletionSource;

        public class Result
        {
            public LevelData Level { get; set; }
            public bool AllLevelsComplete { get; set; }
            public bool GoBackToMenu { get; set; }
        }

        private void OnEnable()
        {
            _replayButton.onClick.AddListener(OnReplayButtonClick);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.AddListener(OnGoToMenuButtonClick);
        }
        
        private void OnDisable()
        {
            _replayButton.onClick.RemoveListener(OnReplayButtonClick);
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
            _goToMenuButton.onClick.RemoveListener(OnGoToMenuButtonClick);
        }
        
        private void OnGoToMenuButtonClick()
        {
            gameObject.SetActive(false);
            _taskCompletionSource.SetResult(new Result
            {
                Level = null,
                AllLevelsComplete = false,
                GoBackToMenu = true
            });
            _taskCompletionSource = null;
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
            
            gameObject.SetActive(false);
            _taskCompletionSource.SetResult(new Result
            {
                Level = _levels.Selected,
                AllLevelsComplete = allLevelComplete,
                GoBackToMenu = false
            });
            _taskCompletionSource = null;
        }

        private void OnReplayButtonClick()
        {
            gameObject.SetActive(false);
            _taskCompletionSource.SetResult(new Result
            {
                Level = _levels.Selected,
                AllLevelsComplete = false,
                GoBackToMenu = false
            });
            _taskCompletionSource = null;
        }

        public Task<Result> Show()
        {
            Debug.Assert(_taskCompletionSource == null);
            gameObject.SetActive(true);
            _panel.DOPunchScale(Vector3.one * 0.2f, 0.4f);
            _taskCompletionSource = new TaskCompletionSource<Result>();
            return _taskCompletionSource.Task;
        }
    }
}