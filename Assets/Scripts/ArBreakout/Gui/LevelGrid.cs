using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArBreakout.Gui
{
    public class LevelGrid : MonoBehaviour
    {
        [SerializeField] private RectTransform _levelItemContainer;

        private List<LevelItem> _items;

        private void Awake()
        {
            _items = _levelItemContainer.GetComponentsInChildren<LevelItem>().ToList();
            SetData(LevelModel.CreateDebugData());
        }

        public void SetData(List<LevelModel> levels)
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
            Debug.Log(levelModel);
        }
    }
}