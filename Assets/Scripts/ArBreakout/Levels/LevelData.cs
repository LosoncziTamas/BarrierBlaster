using Array2DEditor;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private Array2DChar _layout;
        [SerializeField] private float _timeLimit;
        [SerializeField] private string _name;
        [SerializeField] private bool _unlocked;

        public Array2DChar Layout => _layout;

        public float TimeLimit => _timeLimit;

        public string Name => _name;

        public bool Unlocked => _unlocked;

        public int Id => GetInstanceID();
    }
}