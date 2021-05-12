using System;
using System.Collections.Generic;
using ArBreakout.Misc;
using UnityEngine;

namespace ArBreakout.Main
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private LevelSelectorItem _selectorItemPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ItemPool _pool;

        private List<ItemData> _items = new List<ItemData>();

        public class ItemData
        {
            public readonly Action<ItemData> onClick;
            public readonly string levelText;
            public readonly bool unlocked;
            public readonly object dataRef;

            public ItemData(Action<ItemData> onClick, string levelText, bool unlocked, object dataRef = null)
            {
                this.onClick = onClick;
                this.levelText = levelText;
                this.dataRef = dataRef;
                this.unlocked = unlocked;
            }
        }

        private void Clear()
        {
            _items.Clear();
            while (_content.childCount > 0)
            {
                var child = _content.GetChild(0);
                _pool.ReturnItem(child.gameObject);
            }
        }

        public void SetItems(List<ItemData> itemsToAdd)
        {
            Clear();
            _items.AddRange(itemsToAdd);
            foreach (var itemData in _items)
            {
                var itemInstance = _pool.GetItem().GetComponent<LevelSelectorItem>();
                itemInstance.Init(itemData);

                var itemTransform = itemInstance.transform;
                itemTransform.SetParent(_content.transform);
                itemTransform.localScale = Vector3.one;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}