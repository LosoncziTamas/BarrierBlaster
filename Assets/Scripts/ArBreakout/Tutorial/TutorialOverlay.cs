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
        [SerializeField] private GameObject _brick;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Canvas _tutorialCanvas;
        
        [SerializeField] private PowerUpMappingScriptableObject _powerUpMappings;

        private MeshRenderer _brickMeshRenderer;
        private Action _onDismissed;
        private Action _onExitGame;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_powerUpMappings);
            Assert.IsTrue(_powerUpMappings.mappings.Length > 0);
            Assert.IsNotNull(_descriptionText);
            Assert.IsNotNull(_brickSelector);
        }
        
        private void Awake()
        {
            foreach (var description in _powerUpMappings.mappings)
            {
                _brickSelector.CreateNewItem(description.name);
            }

            _brickMeshRenderer = _brick.GetComponent<MeshRenderer>();
            DisplayItemAtIndex(0);
            _brickMeshRenderer.material = _powerUpMappings.mappings[0].material; 
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
            AnimateBrickRotation(-120);
        }
        
        private void OnPrevButtonClick()
        {
            AnimateBrickRotation(120);
        }

        private void AnimateBrickRotation(float degree)
        {
            DisplayItemAtIndex(_brickSelector.index);
            DOTween.Sequence().Append(_brick.transform.DOPunchRotation(new Vector3(0.0f, degree, 0.0f), 1.0f, 1, 0.5f))
                .InsertCallback(0.3f,
                    () =>
                    {
                        _brickMeshRenderer.material = _powerUpMappings.mappings[_brickSelector.index].material; 
                    });
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
