using BarrierBlaster.Common.Tween;
using BarrierBlaster.Game.Obstacles;
using UnityEngine;

namespace BarrierBlaster.Levels.Builder
{
    [RequireComponent(typeof(Renderer))]
    public class LevelBuilderObstacle : MonoBehaviour
    {
        [SerializeField] private MoveTweenProperties _movementProperties;
        
        private Color GetColor()
        {
            return GetComponent<Renderer>().material.color;
        }
        
        public ObstacleAttributes GetObstacleAttributes()
        {
            Debug.Assert(_movementProperties != null);
            var obstacleTransform = transform;
            return new ObstacleAttributes
            {
                Color = GetColor(),
                Scale = obstacleTransform.localScale,
                StartPos = obstacleTransform.position,
                Rotation = obstacleTransform.rotation,
                MoveTweenProperties = _movementProperties
            };
        }
    }
}