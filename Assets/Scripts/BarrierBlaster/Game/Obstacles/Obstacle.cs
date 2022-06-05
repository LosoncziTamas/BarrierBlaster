using BarrierBlaster.Common;
using BarrierBlaster.Common.Tween;
using DG.Tweening;
using UnityEngine;

namespace BarrierBlaster.Game.Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        public const string Tag = "Obstacle";
        
        [SerializeField] private ChangeMeshColor _changeMeshColor;
        [SerializeField] private GameEntities _gameEntities;

        private ObstacleAttributes _obstacleAttributes;
        private MoveTweenProperties _tweenProperties;

        private Tween _tween;

        private void Awake()
        {
            _gameEntities.Add(this);
        }
        
        private void Start()
        {
            _changeMeshColor.SetColor(_obstacleAttributes.Color);

            _tween = transform.DOMove(_tweenProperties.EndValue, _tweenProperties.Duration)
                .SetEase(_tweenProperties.Ease)
                .SetLoops(_tweenProperties.LoopCount, _tweenProperties.LoopType);
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }

        public void Init(ObstacleAttributes obstacleAttributes)
        {
            var t = transform;
            t.localScale = obstacleAttributes.Scale;
            t.localRotation = obstacleAttributes.Rotation;
            t.position = obstacleAttributes.StartPos;
            _obstacleAttributes = obstacleAttributes;
            _tweenProperties = _obstacleAttributes.MoveTweenProperties;
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