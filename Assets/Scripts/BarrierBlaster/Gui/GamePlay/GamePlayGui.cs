using BarrierBlaster.Common;
using BarrierBlaster.Common.Events;
using BarrierBlaster.Common.Variables;
using BarrierBlaster.Game;
using BarrierBlaster.Game.Scoring;
using BarrierBlaster.Game.Stage;
using BarrierBlaster.Gui.Modal;
using JetBrains.Annotations;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace BarrierBlaster.Gui.GamePlay
{
    public class GamePlayGui : AppState
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private IntVariable _lifeCount;
        [SerializeField] private GameEntities _entities;
        [SerializeField] private GameEvent _onLifeLost;

        private PauseModal _pauseModal;
        private GameOverModal _gameOverModal;
        private LevelCompleteModal _levelCompleteModal;
        private LevelRoot _levelRoot;
        private StagePerformanceTracker _stagePerformanceTracker;

        protected override void Awake()
        {
            base.Awake();
            _levelRoot = FindObjectOfType<LevelRoot>();
            _stagePerformanceTracker = FindObjectOfType<StagePerformanceTracker>();
            _pauseModal = FindObjectOfType<PauseModal>(includeInactive: true);
            _gameOverModal = FindObjectOfType<GameOverModal>(includeInactive: true);
            _levelCompleteModal = FindObjectOfType<LevelCompleteModal>(includeInactive: true);
        }
        
        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            _levelRoot.InitLevel(_levels.Selected);
            _lifeCount.Value = 3;
            _stagePerformanceTracker.BeginTracking(_levels.Selected);
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnPause);
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
        }

        [UsedImplicitly]
        public async void OnBallMissed()
        {
            var activeBallCount = 0;
            foreach (var ball in _entities.Balls)
            {
                if (!ball.IsMissed)
                {
                    activeBallCount++;
                }
            }

            if (activeBallCount > 0)
            {
                return;
            }
            
            _lifeCount.Value--;
            _onLifeLost.Raise();
            
            if (_lifeCount.Value < 1)
            {
                GameTime.Paused = true;
                var retry = await _gameOverModal.Show(_levels.Selected.Name);
                GameTime.Paused = false;
                if (retry)
                {
                    _lifeCount.Value = 3;
                    _stagePerformanceTracker.BeginTracking(_levels.Selected);
                    _levelRoot.ContinueWithLevel(_levels.Selected, reset: true);
                }
                else
                {
                    _levelRoot.ClearLevel();
                    Controller.TransitionTo(typeof(MainMenu));
                }
            }
        }

        public async void OnActiveBricksCleared()
        {
            GameTime.Paused = true;
            var performance = _stagePerformanceTracker.EndTracking();
            var result = await _levelCompleteModal.Show(_levels.Selected.Name, performance);
            GameTime.Paused = false;
            if (result.GoBackToMenu)
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            } 
            else if (result.AllLevelsComplete)
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
            else if (result.Level != null)
            {
                _stagePerformanceTracker.BeginTracking(_levels.Selected);
                _levelRoot.ContinueWithLevel(_levels.Selected, reset: false);
            }
        }

        private async void OnPause()
        {
            if (Input.anyKey)
            {
                return;
            }
            GameTime.Paused = true;
            var returnTo = await _pauseModal.Show(_levels.Selected.Name);
            GameTime.Paused = false;
            if (returnTo == PauseModal.ReturnState.MainMenu)
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
        }

#if UNITY_ANDROID
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKey(KeyCode.Escape))
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
        }
#endif
    }
}