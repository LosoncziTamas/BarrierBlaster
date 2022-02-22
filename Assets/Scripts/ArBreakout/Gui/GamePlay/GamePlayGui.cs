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
        [SerializeField] private LifeCounter _lifeCounter;
        [SerializeField] private Text _timeLeftText;
        [SerializeField] private Button _backButton;
        [SerializeField] private PowerUpPanel _powerUpPanel;
        [SerializeField] private Levels.Levels _levels;
        
        [SerializeField] private IntVariable _lifeCount;
        
        private TutorialOverlay _tutorialOverlay;
        private LevelRoot _levelRoot;

        protected override void Awake()
        {
            base.Awake();
            _tutorialOverlay = FindObjectOfType<TutorialOverlay>();
            _levelRoot = FindObjectOfType<LevelRoot>();
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
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
        }

        [UsedImplicitly]
        public void OnBallMissed()
        {
            _lifeCount.Value--;
            if (_lifeCount.Value < 0)
            {
                // TODO: display modal   
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