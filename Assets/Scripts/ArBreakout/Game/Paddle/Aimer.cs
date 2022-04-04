using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Game.Paddle
{
    public class Aimer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        private void Start()
        {
            var path = new List<Vector3> { new Vector3(-1.0f, 1.0f, 0.0f), new Vector3(0.0f, 2.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f) };
            _transform.DOLocalPath(path.ToArray(), 2.0f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}