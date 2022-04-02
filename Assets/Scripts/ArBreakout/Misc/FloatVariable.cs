using UnityEngine;

namespace ArBreakout.Misc
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField] private float _value;
        
        public float Value
        {
            get => _value;
            set => _value = value;
        }
    }
}