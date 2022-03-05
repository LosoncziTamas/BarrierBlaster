using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class Levels : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels;

        public List<LevelData> All => _levels;
        
        public LevelData Selected { get; set; }

        public LevelData GetById(int id)
        {
            return _levels.FirstOrDefault((data => data.Id.Equals(id)));
        }
        
        
    }
}