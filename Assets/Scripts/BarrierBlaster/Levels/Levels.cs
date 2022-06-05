using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BarrierBlaster.Levels
{
    [CreateAssetMenu]
    public class Levels : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels;

        public List<LevelData> All => _levels;

        private LevelData _selected;

        public LevelData NextLevel
        {
            get
            {
                var currentLevelIdx = _levels.IndexOf(_selected);
                return currentLevelIdx + 1 < _levels.Count ? _levels[currentLevelIdx + 1] : null;
            }
        }

        public LevelData Selected
        {
            get => _selected;
            set
            {
                if (_selected != null)
                {
                    Debug.Log($"old: {_selected.Name} idx: {_levels.IndexOf(_selected)}");
                }
                _selected = value;
                Debug.Log($"new: {_selected.Name} idx: {_levels.IndexOf(_selected)}");
            }
        }

        public LevelData GetById(string id)
        {
            return _levels.FirstOrDefault((data => data.Id.Equals(id)));
        }
    }
}