using System;
using System.Collections.Generic;
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
        [SerializeField] private List<Color> _referenceColors;

        public Array2DString Layout => _layout;

        public float TimeLimit => _timeLimit;

        public string Name => _name;

        public bool Unlocked => _unlocked;

        public Array2DColor Colors => _colors;

        public int Id => GetInstanceID();

        private void OnValidate()
        {
            // TODO: figure out if this can be enforced somehow
            if (_layout.GridSize == _colors.GridSize)
            {
                var layoutCells = _layout.GetCells();
                var colorCells = _colors.GetCells();
                var rowCount = layoutCells.GetLength(0);
                for (var row = 0; row < rowCount; row++)
                {
                    for (var col = 0; col < layoutCells.GetLength(1); col++)
                    {
                        var c = layoutCells[row, col];
                        if (string.IsNullOrWhiteSpace(c))
                        {
                            colorCells[row, col] = Color.black;
                            colorCells[row, col].a = 0;
                        }
                        else
                        {
                            colorCells[row, col].a = 1;
                        }
                    }
                }
            }
        }
    }
}