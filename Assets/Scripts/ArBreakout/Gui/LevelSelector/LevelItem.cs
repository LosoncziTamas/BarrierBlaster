using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.LevelSelector
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _shadow;
        [SerializeField] private GameObject _lockedIcon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Stars _stars;
        [SerializeField] private RectTransform _rectTransform;

        private Action<LevelModel> _onClickAction;
        private LevelModel _levelModel;

        private bool _unlocked;

        public bool Unlocked
        {
            get => _unlocked;
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

        private void Start()
        {
            _rectTransform.DOPunchScale(Vector3.one, 0.6f);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _onClickAction?.Invoke(_levelModel);
        }

        public void Bind(LevelModel data, Action<LevelModel> click)
        {
            _levelModel = data;
            _onClickAction = click;
            _text.text = _levelModel.Text;
            Unlocked = data.Unlocked;
        }

        private void Lock()
        {
            _stars.gameObject.SetActive(false);
            _button.interactable = false;
            _shadow.SetActive(false);
            _text.gameObject.SetActive(false);
            _lockedIcon.SetActive(true);
        }

        private void Unlock()
        {
            _stars.gameObject.SetActive(true);
            _button.interactable = true;
            _shadow.SetActive(true);
            _text.gameObject.SetActive(true);
            _lockedIcon.SetActive(false);
        }
    }
}