using ArBreakout.Common.Tween;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Stage
{
    public class Obstacle : MonoBehaviour
    {
        public const string Tag = "Obstacle";
        
        [SerializeField] private MoveTweenProperties _tweenProperties;
        [SerializeField] private ChangeMeshColor _changeMeshColor;
        [SerializeField] private Color _color;

        private Tween _tween;
        private Vector3 _startPos;
        
        private void Start()
        {
            _startPos = transform.position;
            _changeMeshColor.SetColor(_color);

            _tween = transform.DOMove(_tweenProperties.EndValue, _tweenProperties.Duration)
                .SetEase(_tweenProperties.Ease)
                .SetLoops(_tweenProperties.LoopCount, _tweenProperties.LoopType);
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Reanimate obstacle"))
            {
                _changeMeshColor.SetColor(_color);
                _tween.Kill();
                transform.position = _startPos;
                _tween = transform.DOMove(_tweenProperties.EndValue, _tweenProperties.Duration)
                    .SetEase(_tweenProperties.Ease)
                    .SetLoops(_tweenProperties.LoopCount, _tweenProperties.LoopType);
            }
        }
    }
}