using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class Aimer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private AimerProperties _aimerProperties;

        private Tweener _tweener;

        private void Start()
        {
            _tweener = DOVirtual.Float(_aimerProperties.StartAngle, _aimerProperties.EndAngle, _aimerProperties.Duration, OnFloatChange)
                .SetEase(_aimerProperties.Ease)
                .SetLoops(-1, _aimerProperties.LoopType);
        }

        private void OnDisable()
        {
            _tweener.Pause();
        }

        private void OnEnable()
        {
            _tweener.Restart();
        }

        private void OnFloatChange(float newValue)
        {
            Debug.Log("OnFloatChange" + newValue);
            var x = Mathf.Cos(newValue * Mathf.Deg2Rad);
            var y = Mathf.Sin(newValue * Mathf.Deg2Rad);
            transform.localPosition = new Vector3(x, y, 0) * _aimerProperties.Length;
        }
    }
}