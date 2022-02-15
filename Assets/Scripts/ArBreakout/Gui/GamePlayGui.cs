using ArBreakout.Misc;
using ArBreakout.PowerUps;
using ArBreakout.Tutorial;
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

        private TutorialOverlay _tutorialOverlay;
        
        protected override void Awake()
        {
            base.Awake();
            _tutorialOverlay = FindObjectOfType<TutorialOverlay>();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnPause);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
        }
        
        private async void OnPause()
        {
            GameTime.paused = true;
            var returnTo = await _tutorialOverlay.Show();
            GameTime.paused = false;
            if (returnTo == TutorialOverlay.ReturnState.MainMenu)
            {
                Controller.TransitionTo(typeof(MainMenu));
            }
        }
    }
}