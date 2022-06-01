using System;
using System.Collections;
using ArBreakout.Common;
using ArBreakout.Common.Events;
using ArBreakout.Common.Tween;
using ArBreakout.PowerUps;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Bricks
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BrickBehaviour : MonoBehaviour
    {
        public const string GameObjectTag = "Brick";

        private static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
        private static readonly int MetallicProperty = Shader.PropertyToID("_Metallic");
        private Collectable _collectableInstance;

        public BrickPool Pool { set; get; }

        [SerializeField] private ChangeMeshColor _changeMeshColor;
        [SerializeField] private PowerUpMapping _powerUpMappings;
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private GameEvent _brickSmashed;
        [SerializeField] private FloatTweenProperties _powerUpBrickTweenProperties;

        private Renderer _renderer;
        private Collider _collider;
        private int _hitPoints;
        private PowerUpDescriptor _powerUpProperties;
        private Vector3 _targetScale;
        private Tween _powerUpBrickTween;
        
        private void Awake()
        {
            _gameEntities.Add(this);
            _renderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }

        private void OnDestroy()
        {
            _gameEntities.Remove(this);
        }

        public void Init(BrickAttributes brickAttributes, int totalRowCount)
        {
            _hitPoints = brickAttributes.HitPoints;
            _targetScale = brickAttributes.Scale;
            _changeMeshColor.SetColor(brickAttributes.Color);
            _collider.enabled = false;
            transform.localScale = Vector3.zero;
            KillAnimation();

            SetupCollectable(brickAttributes.PowerUp);

            var animAppearDelay = Mathf.Sin((float) brickAttributes.RowIndex / totalRowCount * Mathf.PI);
            Invoke(nameof(AnimateAppear), animAppearDelay);
        }

        public void KillAnimation()
        {
            _powerUpBrickTween?.Rewind(false);
            _powerUpBrickTween?.Kill();
            _powerUpBrickTween = null;
        }

        private void AnimateAppear()
        {
            // Use smaller scale, so it's not tightly packed.
            _renderer.material.SetFloat(HighlightIntensity, 0.0f);
            transform.AnimatePunchScale(_targetScale, Ease.Linear, 0.4f);
            _collider.enabled = true;
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Appear);
        }

        private void SetupCollectable(PowerUp powerUp)
        {
            if (powerUp == PowerUp.None)
            {
                _powerUpProperties = null;
                return;
            }
            _powerUpProperties = _powerUpMappings.GetPowerUpDescriptor(powerUp);
            _powerUpBrickTween = CreateTween();
        }

        private Tween CreateTween()
        {
            return _renderer.material.DOFloat(_powerUpBrickTweenProperties.TargetValue, MetallicProperty, _powerUpBrickTweenProperties.Duration)
                .SetEase(_powerUpBrickTweenProperties.Ease)
                .SetLoops(_powerUpBrickTweenProperties.LoopCount, _powerUpBrickTweenProperties.LoopType);
        }
        
        public void Smash(int times)
        {
            _hitPoints = Math.Max(0, _hitPoints - times);

            if (_hitPoints == 0)
            {
                _brickSmashed.Raise();
                // Temporarily disable collision until the animation finishes off.
                _collider.enabled = false;
                transform.AnimatePunchScale(transform.localScale, Ease.Linear, 0.2f);
                StartCoroutine(AnimateHit(0.2f, destroy: true));
                return;
            }
            else
            {
                var currentScale = transform.localScale;
                transform.DOScale(currentScale * 0.8f, 0.4f);
            }

            StartCoroutine(AnimateHit(0.4f, destroy: false));
        }

        private IEnumerator AnimateHit(float duration, bool destroy)
        {
            var left = duration;
            while (left > 0)
            {
                left -= Time.deltaTime;
                _renderer.material.SetFloat(HighlightIntensity,
                    Mathf.Sin((duration - left) / duration * Mathf.PI) * 0.2f);
                yield return new WaitForEndOfFrame();
            }

            if (destroy)
            {
                EmitCollectable();
                _collider.enabled = true;
                Pool.ReturnBrick(this, raiseEvent: true);
            }
        }

        private void EmitCollectable()
        {
            if (_powerUpProperties)
            {
                _collectableInstance = Instantiate(_powerUpProperties.collectablePrefab, transform.parent);
                _collectableInstance.transform.position = transform.position;
                _collectableInstance.Init(_powerUpProperties);
                _collectableInstance.AnimateAppearance();
            }
        }
    }
}