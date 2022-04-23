using System;
using System.Collections.Generic;
using System.Linq;
using ArBreakout.Common;
using ArBreakout.Gui.GamePlay;
using ArBreakout.Gui.LevelSelector;
using ArBreakout.Levels;
using Possible.AppController;
using UnityEngine;

namespace ArBreakout.Gui
{
    public class MainMenu : AppState
    {
        [SerializeField] private RectTransform _levelItemContainer;
        [SerializeField] private Levels.Levels _levels;
        [SerializeField] private CanvasGroup _canvasGroup;

        private List<LevelItem> _items;

        protected override void Awake()
        {
            base.Awake();
            _items = _levelItemContainer.GetComponentsInChildren<LevelItem>().ToList();
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
    }
}