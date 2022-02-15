using System;
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
        [SerializeField] private PointerDetector _leftButton;
        [SerializeField] private PointerDetector _rightButton;
        [SerializeField] private PointerDetector _fireButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private PowerUpPanel _powerUpPanel;

        private void Start()
        {
            
        }
    }
}