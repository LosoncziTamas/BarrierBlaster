using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class Aimer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private AimerProperties _aimerProperties;

        private Sequence _sequence;
        
        private Sequence CreateFromProperties()
        {
            return DOTween.Sequence().Append(
                _transform.DOLocalPath(_aimerProperties.Vectors, 
                    _aimerProperties.Duration, 
                    _aimerProperties.PathType, 
                    _aimerProperties.PathMode)
                    .SetEase(_aimerProperties.Ease)
                    .SetLoops(-1, _aimerProperties.LoopType)
            );
        }

        private void OnGUI()
        {
            GUILayout.Space(400);
            if (GUILayout.Button("Refresh"))
            {
                _sequence?.Goto(0);
                _sequence?.Kill();
                _sequence = CreateFromProperties();
            }
        }
    }
}