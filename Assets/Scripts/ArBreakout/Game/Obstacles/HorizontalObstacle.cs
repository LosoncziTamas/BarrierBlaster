using ArBreakout.Common;
using ArBreakout.Common.Tween;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Obstacles
{
    public class HorizontalObstacle : Obstacle
    {
        public const string Tag = "Obstacle";
        
        [SerializeField] private ChangeMeshColor _changeMeshColor;
        [SerializeField] private Color _color;

        private ObstacleAttributes _obstacleAttributes;
        private MoveTweenProperties _tweenProperties;

        private Tween _tween;
        private Vector3 _startPos;

        public void Init(ObstacleAttributes obstacleAttributes)
        {
            var t = transform;
            t.localScale = obstacleAttributes.Scale;
            t.localRotation = obstacleAttributes.Rotation;
            t.position = obstacleAttributes.StartPos;
            _obstacleAttributes = obstacleAttributes;
            _tweenProperties = _obstacleAttributes.MoveTweenProperties;
        }
        
        private void Start()
        {
            _startPos = _obstacleAttributes.StartPos;
            _changeMeshColor.SetColor(_obstacleAttributes.Color);

            _tween = transform.DOMove(_tweenProperties.EndValue, _tweenProperties.Duration)
                .SetEase(_tweenProperties.Ease)
                .SetLoops(_tweenProperties.LoopCount, _tweenProperties.LoopType);
        }

        private void FixedUpdate()
        {
            switch (GameTime.Paused)
            {
                case true when _tween.IsPlaying():
                    _tween.Pause();
                    break;
                case false when !_tween.IsPlaying():
                    _tween.Play();
                    break;
            }
        }
    }
}