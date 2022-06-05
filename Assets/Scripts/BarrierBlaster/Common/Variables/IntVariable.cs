using UnityEngine;

namespace BarrierBlaster.Common.Variables
{
    [CreateAssetMenu]
    public class IntVariable : ScriptableObject
    {
        [SerializeField] private int _value;
        public int Value
        {
            get => _value;
            set => _value = value;
        }
    }
}