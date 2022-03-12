using System;
using System.Collections.Generic;
using ArBreakout.Game.Bricks;
using Array2DEditor;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private Array2DString _layout;
        [SerializeField] private Array2DColor _colors;
        [SerializeField] private float _timeLimit;
        [SerializeField] private string _name;
        [SerializeField] private bool _unlocked;

        public Array2DString Layout => _layout;

        public float TimeLimit => _timeLimit;

        public string Name => _name;

        public bool Unlocked => _unlocked;

        public Array2DColor Colors => _colors;

        public int Id => GetInstanceID();

        public List<BrickAttributes> BrickAttributes = new();
    }
}