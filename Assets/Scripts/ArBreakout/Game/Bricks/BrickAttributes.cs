using System;
using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game.Bricks
{
    [Serializable]
    public class BrickAttributes
    {
        public Color Color;
        public int HitPoints; 
        public PowerUp PowerUp;
        public int RowIndex;
        public Vector3 Scale;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}