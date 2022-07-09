using System;
using System.Collections.Generic;
using System.Linq;
using BarrierBlaster.Gui.GamePlay;
using BarrierBlaster.Gui.LevelSelector;
using BarrierBlaster.Gui.Modal;
using BarrierBlaster.Levels;
using Possible.AppController;
using UnityEngine;
using UnityEngine.UI;

namespace BarrierBlaster.Gui
{
    public class MainMenu : AppState
    {
        [SerializeField] private RectTransform _levelItemContainer;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _settingsButton;

        private List<LevelItem> _items;
        private SettingsModal _settingsModal;

        protected override void Awake()
        {
            base.Awake();
            _items = _levelItemContainer.GetComponentsInChildren<LevelItem>().ToList();
            _settingsModal = FindObjectOfType<SettingsModal>(includeInactive: true);
        }

        private void OnEnable()
        {
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnDisable()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        private async void OnSettingsClicked()
        {
            await _settingsModal.Show();
        }

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            foreach (var item in _items)
            {
                item.Unlocked = false;
            }
            SetData(_levels.All);
        }

        private void SetData(List<LevelData> levels)
        {
            var max = Math.Min(levels.Count, _items.Count);
            for (var i = 0; i < max; i++)
            {
                var levelModel = levels[i];
                if (levelModel)
                {
                    _items[i].Bind(LevelModel.Create(levelModel), OnLevelSelect);
                }
            }
        }

        private void OnLevelSelect(LevelModel levelModel)
        {
            var selected = _levels.GetById(levelModel.Id);
            _levels.Selected = selected;
            Controller.TransitionTo(typeof(GamePlayGui));
        }

#if UNITY_ANDROID
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
#endif
        
    }
}