using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    [CreateAssetMenu]
    public class AimerProperties : ScriptableObject
    {
        public Vector3[] Vectors;
        public float Duration;
        public LoopType LoopType;
        public PathType PathType;
        public PathMode PathMode;
        public Ease Ease;
        
    }
}