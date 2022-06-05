using UnityEngine;

namespace BarrierBlaster.Common.Tween
{
    [CreateAssetMenu(menuName = "Tweens/Move Tween Properties")]
    public class MoveTweenProperties : TweenProperties
    {
        public Vector3 EndValue;
    }
}