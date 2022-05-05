using System;
using System.Collections.Generic;
using ArBreakout.Game.Bricks;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _id;
        [SerializeField] private bool _unlockedManually;
        
        public string Name => _name;

        public bool Unlocked
        {
            get => _unlockedManually || Convert.ToBoolean(PlayerPrefs.GetInt(_id, 0));
            set
            {
                var result = value ? 1 : 0; 
                PlayerPrefs.SetInt(_id, result);
            }
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public List<BrickAttributes> BrickAttributes = new();
    }
}