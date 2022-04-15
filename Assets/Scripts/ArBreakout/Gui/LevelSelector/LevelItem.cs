using System;
using System.Collections;
using ArBreakout.Common.Tween;
using ArBreakout.Game.Scoring;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.LevelSelector
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Color _disabledColor;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _shadow;
        [SerializeField] private GameObject _lockedIcon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Stars _stars;
        [SerializeField] private ShakePositionProperties _lockedLevelShake;

        private Action<LevelModel> _onClickAction;
        private LevelModel _levelModel;
        private Tween _lockedAnimTween;

        private bool _unlocked;
        private bool _invoking;

        public bool Unlocked
        {
            set
            {
                if (value)
                {
                    Unlock();
                }
                else
                {
                    Lock();
                }

                _unlocked = value;
            }
        }
        
        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private IEnumerator AnimateAndInvokeClickEvent()
        {
            _invoking = true;
            const float animationDuration = 0.3f;
            transform.DOPunchScale(Vector3.one * 0.3f , animationDuration);
            yield return new WaitForSeconds(animationDuration);
            _invoking = false;
            _onClickAction?.Invoke(_levelModel);
        }

        private void OnButtonClick()
        {
            if (_unlocked)
            {
                if (_invoking)
                {
                    return;
                }
                StartCoroutine(AnimateAndInvokeClickEvent());
            }
            else
            {
                var isPlaying = _lockedAnimTween?.IsPlaying() ?? false;
                if (!isPlaying)
                {
                    _lockedAnimTween = transform.DOShakePosition(
                        _lockedLevelShake.Duration, 
                        _lockedLevelShake.Strength, 
                        _lockedLevelShake.Vibrato, 
                        _lockedLevelShake.Randomness, 
                        _lockedLevelShake.FadeOut);
                }
            }
        }

        public void Bind(LevelModel data, Action<LevelModel> click)
        {
            _levelModel = data;
            _onClickAction = click;
            _text.text = _levelModel.Text;
            Unlocked = data.Unlocked;
            var starCount = StagePerformanceTracker.GetStarCountForStage(data.Id);
            _stars.SetFilledCount(starCount);
        }

        private void Lock()
        {
            _stars.gameObject.SetActive(false);
            _button.image.color = _disabledColor;
            _shadow.SetActive(false);
            _text.gameObject.SetActive(false);
            _lockedIcon.SetActive(true);
        }

        private void Unlock()
        {
            _stars.gameObject.SetActive(true);
            _button.image.color = Color.white;
            _shadow.SetActive(true);
            _text.gameObject.SetActive(true);
            _lockedIcon.SetActive(false);
        }
    }
}