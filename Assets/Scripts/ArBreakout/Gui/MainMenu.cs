using System;
using System.Collections.Generic;
using System.Linq;
using ArBreakout.Levels;
using Possible.AppController;
using UnityEngine;

namespace ArBreakout.Gui
{
    public class MainMenu : AppState
    {
        [SerializeField] private RectTransform _levelItemContainer;

        private List<LevelItem> _items;
        private LevelProgression _levelProgression;
        
        protected override void Awake()
        {
            base.Awake();
            _items = _levelItemContainer.GetComponentsInChildren<LevelItem>().ToList();
            _levelProgression = LevelProgression.Instance;
        }

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            
            var levels = _levelProgression.Levels.Select(info => new LevelModel
            {
                Text = info.displayedName,
                Unlocked = info.unlocked
            }).ToList();
            
            SetData(levels);
        }

        private void SetData(List<LevelModel> levels)
        {
            var max = Math.Min(levels.Count, _items.Count);
            for (var i = 0; i < max; i++)
            {
                var levelModel = levels[i];
                _items[i].Bind(levelModel, OnLevelSelect);
            }
        }
        
        private void OnLevelSelect(LevelModel levelModel)
        {
            Controller.TransitionTo(typeof(GamePlayGui));
        }
    }
}