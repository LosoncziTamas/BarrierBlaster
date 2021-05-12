using ArBreakout.Misc;
using ArBreakout.SinglePlayer;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Main
{
    public class MainAppState : AppState
    {
        [SerializeField] private Button _selectLevelButton;
        [SerializeField] private CanvasGroup _canvas;

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            _canvas.SetVisibility(true);
        }

        public override void OnExit(AppState toState)
        {
            base.OnExit(toState);
            _canvas.SetVisibility(false);
        }

        private void OnEnable()
        {
            _selectLevelButton.onClick.AddListener(OnLevelSelectorButtonClick);
        }

        private void OnLevelSelectorButtonClick()
        {
            Controller.TransitionTo(typeof(LevelSelectorAppState));
        }

        private void OnDisable()
        {
            _selectLevelButton.onClick.RemoveListener(OnLevelSelectorButtonClick);
        }
    }
}