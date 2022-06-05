using DG.Tweening;
using UnityEngine;

namespace BarrierBlaster.Common.Tween
{
    [CreateAssetMenu(menuName = "Tweens/Base Tween Properties")]
    public class TweenProperties : ScriptableObject
    {
        public float Duration;
        public Ease Ease;
        public int LoopCount;
        public LoopType LoopType;
    }
}