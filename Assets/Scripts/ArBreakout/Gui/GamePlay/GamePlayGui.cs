using ArBreakout.Game;
using ArBreakout.Game.Course;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using ArBreakout.Tutorial;
using JetBrains.Annotations;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.GamePlay
{
    public class GamePlayGui : AppState
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private PowerUpPanel _powerUpPanel;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private IntVariable _lifeCount;
        
        private TutorialOverlay _tutorialOverlay;
        private GameOverModal _gameOverModal;
        private LevelCompleteModal _levelCompleteModal;
        private LevelRoot _levelRoot;

        protected override void Awake()
        {
            base.Awake();
            _tutorialOverlay = FindObjectOfType<TutorialOverlay>();
            _levelRoot = FindObjectOfType<LevelRoot>();
            _gameOverModal = FindObjectOfType<GameOverModal>(includeInactive: true);
            _levelCompleteModal = FindObjectOfType<LevelCompleteModal>(includeInactive: true);
        }
        
        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            _levelRoot.InitLevel(_levels.Selected);
            _lifeCount.Value = 3;
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
            _lifeCount.Value--;
            if (_lifeCount.Value < 1)
            {
                GameTime.paused = true;
                var retry = await _gameOverModal.Show();
                GameTime.paused = false;
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
            GameTime.paused = true;
            var result = await _levelCompleteModal.Show(_levels.Selected.Name);
            GameTime.paused = false;
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
            GameTime.paused = true;
            var returnTo = await _tutorialOverlay.Show();
            GameTime.paused = false;
            if (returnTo == TutorialOverlay.ReturnState.MainMenu)
            {
                _levelRoot.ClearLevel();
                Controller.TransitionTo(typeof(MainMenu));
            }
        }
    }
}