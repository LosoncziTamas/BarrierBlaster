using ArBreakout.PowerUps;
using UnityEngine;

namespace ArBreakout.Game.Bricks
{
    public class BrickAttributes
    {
        public Color Color { get; set; }
        public int HitPoints { get; set; }
        public PowerUp PowerUp { get; set; }
        public int RowIndex { get; set; }
    }
}