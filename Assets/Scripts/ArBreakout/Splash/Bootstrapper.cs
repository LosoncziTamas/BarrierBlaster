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
        
        private void Awake()
        {
            _splash.alpha = 0;
        }

        private IEnumerator Start()
        {
            var loadScene = SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive);
            loadScene.allowSceneActivation = false;
            // TODO: fade
            // TODO: fix lights
            while (loadScene.progress < 0.9f)
            {
                yield return null;
            }
            loadScene.allowSceneActivation = true;
        }
    }
}