using System.Collections.Generic;
using ArBreakout.Game.Bricks;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private float _timeLimit;
        [SerializeField] private string _name;
        [SerializeField] private bool _unlocked;
        
        public float TimeLimit => _timeLimit;

        public string Name => _name;

        public bool Unlocked => _unlocked;
        
        public int Id => GetInstanceID();

        public List<BrickAttributes> BrickAttributes = new();
    }
}