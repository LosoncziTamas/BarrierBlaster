using System;
using System.Collections.Generic;
using ArBreakout.Game.Bricks;
using ArBreakout.Game.Obstacles;
using ArBreakout.Levels.Builder;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        private const string KeyFormat = "unlock_{0}";
        
        [SerializeField] private string _name;
        [SerializeField] private string _id;
        [SerializeField] private bool _unlockedManually;
        
        public string Name => _name;

        public bool Unlocked
        {
            get => _unlockedManually || Convert.ToBoolean(PlayerPrefs.GetInt(Key, 0));
            set
            {
                var result = value ? 1 : 0; 
                PlayerPrefs.SetInt(Key, result);
            }
        }
        
        private string Key => string.Format(KeyFormat, _id);

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public List<BrickAttributes> BrickAttributes = new();
        public List<ObstacleAttributes> ObstacleAttributes = new();
    }
}