using ArBreakout.Game;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using ArBreakout.Tutorial;
using JetBrains.Annotations;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui
{
    public class GamePlayGui : AppState
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private PowerUpPanel _powerUpPanel;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private IntVariable _lifeCount;
        
        private TutorialOverlay _tutorialOverlay;
        private GameOverModal _gameOverModal;
        private LevelRoot _levelRoot;

        protected override void Awake()
        {
            base.Awake();
            _tutorialOverlay = FindObjectOfType<TutorialOverlay>();
            _levelRoot = FindObjectOfType<LevelRoot>();
            _gameOverModal = FindObjectOfType<GameOverModal>(includeInactive: true);
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
            PaddleBehaviour.PowerUpStateChangeEvent += OnPowerUpStateChangeEvent;
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
            PaddleBehaviour.PowerUpStateChangeEvent -= OnPowerUpStateChangeEvent;
        }

        private void OnPowerUpStateChangeEvent(object sender, PaddleBehaviour.PowerUpState e)
        {
            _powerUpPanel.Refresh(e.ActivePowerUps, e.ActivePowerUpTimes);
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
                    _levelRoot.ContinueWithLevel(_levels.Selected);
                }
                else
                {
                    _levelRoot.ClearLevel();
                    Controller.TransitionTo(typeof(MainMenu));
                }
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