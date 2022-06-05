using BarrierBlaster.Game.Bricks;
using BarrierBlaster.PowerUps;
using UnityEngine;

namespace BarrierBlaster.Levels.Builder
{
    public class LevelBuilderBrick : MonoBehaviour
    {
        [SerializeField] private int _hitPoints;
        [SerializeField] private PowerUp _powerUp;
        
        private Color GetColor()
        {
            return GetComponent<Renderer>().material.color;
        }
        
        public BrickAttributes GetBrickAttributes()
        {
            var brickTransform = transform;
            return new BrickAttributes()
            {
                Color = GetColor(),
                HitPoints = _hitPoints,
                Position = brickTransform.position,
                Rotation = brickTransform.rotation,
                Scale = brickTransform.localScale,
                PowerUp = _powerUp
            };
        }
    }
}