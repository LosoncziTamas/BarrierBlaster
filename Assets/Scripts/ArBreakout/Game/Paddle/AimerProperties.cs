using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    [CreateAssetMenu]
    public class AimerProperties : ScriptableObject
    {
        public float Duration;
        public LoopType LoopType;
        public float Length;
        public Ease Ease;
        public float StartAngle;
        public float EndAngle;
        public float FadeDuration;
        
    }
}