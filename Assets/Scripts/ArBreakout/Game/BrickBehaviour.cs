using System;
using System.Collections;
using ArBreakout.Game.Bricks;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BrickBehaviour : MonoBehaviour
    {
        public class BrickDestroyedArgs : EventArgs
        {
        }

        public const string GameObjectTag = "Brick";

        private static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
        private Collectable _collectableInstance;

        public BrickPool Pool { set; get; }

        [SerializeField] private ChangeMeshColor _changeMeshColor;
        [SerializeField] private PowerUpMapping _powerUpMappings;
        [SerializeField] private GameEntities _gameEntities;
        [SerializeField] private GameEvent _brickSmashed;

        private Renderer _renderer;
        private Collider _collider;
        private int _hitPoints;
        private PowerUpDescriptor _powerUpProperties;
        private Vector3 _targetScale;
        
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
            var scale01 = Mathf.Sin(((float) brickAttributes.RowIndex / totalRowCount) * Mathf.PI);

            var initialAnimScale = Mathf.Clamp(0.5f + (1 - scale01) * 0.5f, 0.5f, 1.0f);
            transform.localScale = _targetScale * initialAnimScale;

            SetupCollectable(brickAttributes.PowerUp);

            Invoke(nameof(AnimateAppear), scale01);
        }

        private void AnimateAppear()
        {
            // Use smaller scale, so it's not tightly packed.
            transform.AnimatePunchScale(_targetScale * 0.9f, Ease.Linear, 0.4f);
            _collider.enabled = true;
        }

        private void SetupCollectable(PowerUp powerUp)
        {
            if (powerUp == PowerUp.None)
            {
                _powerUpProperties = null;
                return;
            }
            _powerUpProperties = _powerUpMappings.GetPowerUpDescriptor(powerUp);
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