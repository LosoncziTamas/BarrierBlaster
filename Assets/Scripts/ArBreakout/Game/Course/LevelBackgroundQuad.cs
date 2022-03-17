using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace ArBreakout.Game.Course
{
    public class LevelBackgroundQuad : MonoBehaviour
    {
        private const float HitAnimDuration = 0.2f;
        private const float MissAnimDuration = 0.3f;
        private static readonly int PrimaryColor = Shader.PropertyToID("_Color");

        [SerializeField] private Color _hitColor;
        [SerializeField] private Color _missColor;
        [SerializeField] private MeshRenderer _renderer;

        private Color _defaultColor;
        
        private void Awake()
        {
            _defaultColor = _renderer.material.GetColor(PrimaryColor);
        }
        
        private void DoHighlightAnimation()
        {
            DOTween.Sequence()
                .Insert(0, _renderer.material.DOColor(_hitColor, HitAnimDuration))
                .Insert(HitAnimDuration, _renderer.material.DOColor(_defaultColor, HitAnimDuration))
                .Play();
        }

        [UsedImplicitly]
        public void OnBrickSmashed()
        {
            DoHighlightAnimation();
        }
        
        [UsedImplicitly]
        public void OnBallMissed()
        {
            DOTween.Sequence()
                .Insert(0, _renderer.material.DOColor(_missColor, MissAnimDuration))
                .Insert(MissAnimDuration, _renderer.material.DOColor(_defaultColor, MissAnimDuration))
                .Play();
        }
    }
}