using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Common.Tween
{
    [CreateAssetMenu]
    public class TweenProperties : ScriptableObject
    {
        public float Duration;
        public Ease Ease;
    }
}