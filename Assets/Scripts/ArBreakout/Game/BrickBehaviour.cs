using System;
using System.Collections;
using ArBreakout.Misc;
using ArBreakout.PowerUps;
using DG.Tweening;
using Possible.Scheduling;
using UnityEngine;

namespace ArBreakout.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BrickBehaviour : MonoBehaviour
    {
        public static event EventHandler<BrickDestroyedArgs> BrickDestroyedEvent;
        public class BrickDestroyedArgs : EventArgs {}

        public const string GameObjectTag = "Brick";
        
        private static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
        private static readonly int ScaleProperty = Shader.PropertyToID("_Scale");
        private static readonly int RotationProperty = Shader.PropertyToID("_Rotation");
        
        public PowerUp PowerUp { private set; get; }
        public BrickPool Pool { set; get; }
        
        // [SerializeField] private Material _hardBrickMaterial1;
        // [SerializeField] private Material _hardBrickMaterial2;
        [SerializeField] private ChangeMeshColor _changeMeshColor;

        private Renderer _renderer;
        private Collider _collider;
        private int _hitPoints;
        private float _initialAnimScale = 1.0f;
        
        private void Awake()
        {
            // TODO: add self to list
            _renderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }

        public void Init(Color color, int index, int count)
        {
            //PowerUp = type;
            //_hitPoints = type == PowerUp.Hard ? 3 : 1;
            //var powerUpSO = PowerUpMappingScriptableObject.Instance.GetPowerUpSO(type);
            //_renderer.material = powerUpSO.material;
            
            // Appear animation
            _changeMeshColor.SetColor(color);
            _collider.enabled = false;
            var scale01 = Mathf.Sin(((float)index / count) * Mathf.PI);
            
            /*if (type.EffectsPaddle())
            {
                _renderer.material.SetFloat(ScaleProperty,  0.8f);
                _renderer.material.SetFloat(RotationProperty,  45f);
            }
            else
            {
                _initialAnimScale = Mathf.Clamp(0.5f + (1 - scale01) * 0.5f, 0.5f, 1.0f);
                _renderer.material.SetFloat(ScaleProperty,  _initialAnimScale);
                _renderer.material.SetFloat(RotationProperty,  0.0f);
            }*/

            Wait.For(scale01)
                .ThenDo(() =>
                {
                    // Use smaller scale, so it's not tightly packed.
                    // transform.AnimatePunchScale(Vector3.one * 0.9f, Ease.Linear, 0.4f);
                    _collider.enabled = true;
                })
                .StartOn(this);
        }

        public void Smash()
        {
            --_hitPoints;
            
            if (_hitPoints == 0)
            {
                BrickDestroyedEvent?.Invoke(this, new BrickDestroyedArgs());
                // Temporarily disable collision until the animation finishes off.
                _collider.enabled = false;
                transform.AnimatePunchScale(transform.localScale, Ease.Linear, 0.2f);
                StartCoroutine(AnimateHit(0.2f, destroy: true));
                return;
            }
            
            if (_hitPoints == 2)
            {
                // _renderer.material = _hardBrickMaterial2;
            }
            else if (_hitPoints == 1)
            {
                // _renderer.material = _hardBrickMaterial1;
            }
            _renderer.material.SetFloat(ScaleProperty,  _initialAnimScale);
            StartCoroutine(AnimateHit(0.4f, destroy: false));
        }
        
        private IEnumerator AnimateHit(float duration, bool destroy)
        {
            var left = duration;
            while (left > 0)
            {
                left -= Time.deltaTime;
                _renderer.material.SetFloat(HighlightIntensity, Mathf.Sin((duration - left)/duration * Mathf.PI) * 0.2f);
                yield return new WaitForEndOfFrame();
            }

            if (destroy)
            {                
                _collider.enabled = true;
                Pool.ReturnBrick(this);
            }
        }
    }
}