using System.Collections;
using ArBreakout.Common.Tween;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArBreakout.Splash
{
    public class Bootstrapper : MonoBehaviour
    {
        private const int GameSceneIndex = 1;

        [SerializeField] private CanvasGroup _splash;
        [SerializeField] private FloatTweenProperties _fadeTweenProperties;
        
        private void Awake()
        {
            _splash.alpha = 1.0f;
        }

        private IEnumerator Start()
        {
            var loadScene = SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Additive);
            loadScene.allowSceneActivation = false;
            while (loadScene.progress < 0.9f)
            {
                yield return null;
            }
            loadScene.allowSceneActivation = true;
            _splash.DOFade(_fadeTweenProperties.TargetValue, _fadeTweenProperties.Duration)
                .SetEase(_fadeTweenProperties.Ease);
        }
    }
}