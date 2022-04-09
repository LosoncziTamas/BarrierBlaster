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
        
        public string Name => _name;

        public bool Unlocked => _unlocked;
        
        public string Id { get; set; }

        public List<BrickAttributes> BrickAttributes = new();
    }
}