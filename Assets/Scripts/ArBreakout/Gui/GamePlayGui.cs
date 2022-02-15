using System;
using ArBreakout.GameInput;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
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

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnPause);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnPause);
        }
        
        private void OnPause()
        {
            GameTime.paused = !GameTime.paused;
        }
    }
}