using UnityEngine;

namespace ArBreakout.GameInput
{
    [CreateAssetMenu]
    public class PlayerInput : ScriptableObject
    {
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Fire { get; set; }

        public void Clear()
        {
            Left = Right = Fire = false;
        }
    }
}