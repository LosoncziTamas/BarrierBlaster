using System;
using ArBreakout.Common;
using ArBreakout.Game;
using ArBreakout.GamePhysics;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.PlaneDetection
{
    public class LevelGhost : MonoBehaviour
    {
        private static readonly Vector3 PlaceholderTargetScale =
            new Vector3(BreakoutPhysics.LevelDimX, 1.0f, BreakoutPhysics.LevelDimY);

        private static readonly Vector3 ShadowTargetScale = new Vector3(3.0f, 1.0f, 3.0f);
        private static readonly Vector3 InitialScale = Vector3.one;

        private static readonly Color SemiTransparentBlack = new Color(0f, 0f, 0f, 0.5f);

        private const float DefaultInnerSpeed = 15.0f;
        private const float DefaultOuterSpeed = 10.0f;

        private float outerSpeed = DefaultOuterSpeed;
        private float innerSpeed = DefaultInnerSpeed;

        public GameObject shadowParent;
        public Transform outer;
        public Transform inner;
        public GameObject placeHolder;
        public GameObject placeHolderTop;

        private MeshRenderer _placeHolderRenderer;

        public bool IsDragging { get; private set; }

        private void Awake()
        {
            placeHolder.transform.localScale = InitialScale;
            shadowParent.transform.localScale = InitialScale;
            _placeHolderRenderer = placeHolder.GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            IsDragging = false;
            placeHolder.transform.DOScale(PlaceholderTargetScale, 0.6f);
            shadowParent.transform.DOScale(ShadowTargetScale, 0.6f);
        }

        private void Update()
        {
            outer.Rotate(Vector3.up, outerSpeed * GameTime.Delta, Space.Self);
            inner.Rotate(Vector3.up, innerSpeed * GameTime.Delta, Space.Self);
        }

        public void Drag()
        {
            IsDragging = true;
            innerSpeed *= 4.0f;
            outerSpeed *= 3.0f;
        }

        public void Release()
        {
            IsDragging = false;
            innerSpeed = DefaultInnerSpeed;
            outerSpeed = DefaultOuterSpeed;
        }

        public void SwapToLevelBase(Action onAnimationComplete)
        {
            placeHolderTop.SetActive(false);
            shadowParent.SetActive(false);
            _placeHolderRenderer.material.DOColor(SemiTransparentBlack, 0.6f);
            DOTween.Sequence()
                .Append(placeHolder.transform.DOLocalMoveY(-0.01f, 0.6f))
                .Join(placeHolder.transform.DOScale(
                    new Vector3(BreakoutPhysics.LevelDimX, 0.01f, BreakoutPhysics.LevelDimY), 0.6f))
                .OnComplete(() => { onAnimationComplete?.Invoke(); });
        }
    }
}