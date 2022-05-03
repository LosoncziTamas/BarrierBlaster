using System.Collections.Generic;
using ArBreakout.Game.Bricks;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _unlocked;
        [SerializeField] private string _id;
        
        public string Name => _name;

        public bool Unlocked
        {
            get => _unlocked;
            set
            {
                _unlocked = value;
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