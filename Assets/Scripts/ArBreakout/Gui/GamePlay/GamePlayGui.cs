using ArBreakout.Common;
using ArBreakout.Common.Events;
using ArBreakout.Common.Variables;
using ArBreakout.Game;
using ArBreakout.Game.Scoring;
using ArBreakout.Game.Stage;
using ArBreakout.Gui.Modal;
using ArBreakout.PowerUps;
using JetBrains.Annotations;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.GamePlay
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
            _pauseModal = FindObjectOfType<PauseModal>();
            _levelRoot = FindObjectOfType<LevelRoot>();
            _stagePerformanceTracker = FindObjectOfType<StagePerformanceTracker>();
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
            PowerUpActivator.PowerUpStateChangeEvent += OnPowerUpStateChangeEvent;
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
            PowerUpActivator.PowerUpStateChangeEvent -= OnPowerUpStateChangeEvent;
        }

        private void OnPowerUpStateChangeEvent(object sender, PowerUpActivator.PowerUpState e)
        {
            // _powerUpPanel.Refresh(e.ActivePowerUps, e.ActivePowerUpTimes);
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
                // TODO: show credits or something
                Debug.Log("All levels complete!");
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
            else if (result.Level != null)
            {
                _levelRoot.ContinueWithLevel(_levels.Selected, reset: false);
            }
        }

        private async void OnPause()
        {
            GameTime.Paused = true;
            var returnTo = await _pauseModal.Show();
            GameTime.Paused = false;
            if (returnTo == PauseModal.ReturnState.MainMenu)
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
        }
    }
}