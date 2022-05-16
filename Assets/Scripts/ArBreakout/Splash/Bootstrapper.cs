using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArBreakout.Splash
{
    public class Bootstrapper : MonoBehaviour
    {
        private const int GameSceneIndex = 1;

        [SerializeField] private CanvasGroup _splash;

        private Tween _fadeTween;
        
        private void Awake()
        {
            _splash.alpha = 0;
        }

        private IEnumerator Start()
        {
            _fadeTween = _splash.DOFade(1.0f, 1.0f).SetAutoKill(false);
            var loadScene = SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive);
            loadScene.allowSceneActivation = false;
            // TODO: fade
            // TODO: fix lights
            while (loadScene.progress < 0.9f || _fadeTween.IsPlaying())
            {
                yield return null;
            }
            _fadeTween.PlayBackwards();
            _fadeTween.OnComplete(() =>
            {
                // loadScene.allowSceneActivation = true;
            }).Rewind();
        }
    }
}