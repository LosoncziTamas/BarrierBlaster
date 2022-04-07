using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Misc
{
    [CreateAssetMenu]
    public class TweenProperties : ScriptableObject
    {
        public float Duration;
        public Ease Ease;
    }
}