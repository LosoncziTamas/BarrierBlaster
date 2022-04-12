using UnityEngine;

namespace ArBreakout.Common.Variables
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