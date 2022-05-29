using System;
using ArBreakout.Common.Tween;
using UnityEngine;

namespace ArBreakout.Game.Obstacles
{
    [Serializable]
    public class ObstacleAttributes
    {
        public Vector3 StartPos;
        public Vector3 Scale;
        public Quaternion Rotation;
        public MoveTweenProperties MoveTweenProperties;
        public Color Color;
    }
}