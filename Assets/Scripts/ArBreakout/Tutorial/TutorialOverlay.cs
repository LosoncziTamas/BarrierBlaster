using System;
using ArBreakout.PowerUps;
using DG.Tweening;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ArBreakout.Tutorial
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private HorizontalSelector _brickSelector;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Canvas _tutorialCanvas;
        [SerializeField] private PowerUpMapping _powerUpMappings;

        private Action _onDismissed;
        private Action _onExitGame;
        private ObjectSwapper _objectSwapper;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_powerUpMappings);
            Assert.IsTrue(_powerUpMappings.mappings.Length > 0);
            Assert.IsNotNull(_descriptionText);
            Assert.IsNotNull(_brickSelector);
        }
        
        private void Awake()
        {
            _objectSwapper = FindObjectOfType<ObjectSwapper>();
            foreach (var description in _powerUpMappings.mappings)
            {
                _brickSelector.CreateNewItem(description.name);
            }

            DisplayItemAtIndex(0);
            _objectSwapper.SwapToPowerUpObject( _powerUpMappings.mappings[0].powerUp, 120);
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _prevButton.onClick.AddListener(OnPrevButtonClick);
            _nextButton.onClick.AddListener(OnNextButtonClick);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _prevButton.onClick.RemoveListener(OnPrevButtonClick);
            _nextButton.onClick.RemoveListener(OnNextButtonClick);
            _onDismissed = null;
            _onExitGame = null;
        }

        public void Show(Action onDismissed, Action onExitGame)
        {
            _onDismissed = onDismissed;
            _onExitGame = onExitGame;
            _tutorialCanvas.enabled = true;
        }

        public void DismissAndResume()
        {
            _tutorialCanvas.enabled = false;
            _onDismissed?.Invoke();
            _onDismissed = _onExitGame = null;
        }

        private void OnBackButtonClick()
        {
            _tutorialCanvas.enabled = false;
            _onExitGame?.Invoke();
            _onDismissed = _onExitGame = null;
        }

        private void OnNextButtonClick()
        {
            var nextIdx = Math.Min(_brickSelector.index, _powerUpMappings.mappings.Length - 1);
            var nextObject = _powerUpMappings.mappings[nextIdx];
            _objectSwapper.SwapToPowerUpObject(nextObject.powerUp, 120);
        }
        
        private void OnPrevButtonClick()
        {
            var prevIdx = Math.Max(_brickSelector.index, 0);
            var prevObject = _powerUpMappings.mappings[prevIdx];
            _objectSwapper.SwapToPowerUpObject(prevObject.powerUp, -120);
        }
        
        private void DisplayItemAtIndex(int index)
        {
            Assert.IsTrue(index > -1 && index < _powerUpMappings.mappings.Length);
            DOTween.Sequence().Append(_descriptionText.DOFade(0.0f, 0.3f)).AppendCallback(() =>
            {
                _descriptionText.DOFade(1.0f, 0.3f);
                _descriptionText.text = _powerUpMappings.mappings[index].descriptionText;
            });   
        }
    }
}
