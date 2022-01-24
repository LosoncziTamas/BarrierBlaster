using Array2DEditor;
using UnityEngine;

namespace ArBreakout.Levels
{
    [CreateAssetMenu]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private Array2DChar _layout;
        [SerializeField] private float _timeLimit;
        
        // TODO: entirely replace legacy level loading
    }
}