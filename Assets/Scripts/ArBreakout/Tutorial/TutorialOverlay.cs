using System;
using System.Threading.Tasks;
using ArBreakout.PowerUps;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ArBreakout.Tutorial
{
    public class TutorialOverlay : MonoBehaviour
    {
        public enum ReturnState
        {
            Game,
            MainMenu
        }

        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Canvas _tutorialCanvas;
        [SerializeField] private PowerUpMapping _powerUpMappings;

        private ObjectSwapper _objectSwapper;
        private TaskCompletionSource<ReturnState> _taskCompletionSource;
        private int _currentIdx;

        private void Start()
        {
            _objectSwapper = FindObjectOfType<ObjectSwapper>();
            _objectSwapper.SwapToPowerUpObject(_powerUpMappings.mappings[0].powerUp, 120);
            DisplayItemAtIndex(0);
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _prevButton.onClick.AddListener(OnPrevButtonClick);
            _nextButton.onClick.AddListener(OnNextButtonClick);
            _closeButton.onClick.AddListener(DismissAndResume);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _prevButton.onClick.RemoveListener(OnPrevButtonClick);
            _nextButton.onClick.RemoveListener(OnNextButtonClick);
            _closeButton.onClick.RemoveListener(DismissAndResume);
        }

        public Task<ReturnState> Show()
        {
            Debug.Assert(_taskCompletionSource == null);
            _taskCompletionSource = new TaskCompletionSource<ReturnState>();
            _tutorialCanvas.enabled = true;
            return _taskCompletionSource.Task;
        }

        public void DismissAndResume()
        {
            _tutorialCanvas.enabled = false;

            _taskCompletionSource.SetResult(ReturnState.Game);
            _taskCompletionSource = null;
        }

        private void OnBackButtonClick()
        {
            _tutorialCanvas.enabled = false;

            _taskCompletionSource.SetResult(ReturnState.MainMenu);
            _taskCompletionSource = null;
        }

        private void OnNextButtonClick()
        {
            var nextIdx = Math.Min(_currentIdx + 1, _powerUpMappings.mappings.Length - 1);
            var nextObject = _powerUpMappings.mappings[nextIdx];
            _objectSwapper.SwapToPowerUpObject(nextObject.powerUp, 120);
            DisplayItemAtIndex(nextIdx);
        }

        private void OnPrevButtonClick()
        {
            var prevIdx = Math.Max(_currentIdx - 1, 0);
            var prevObject = _powerUpMappings.mappings[prevIdx];
            _objectSwapper.SwapToPowerUpObject(prevObject.powerUp, -120);
            DisplayItemAtIndex(prevIdx);
        }
        
        private void DisplayItemAtIndex(int index)
        {
            Assert.IsTrue(index > -1 && index < _powerUpMappings.mappings.Length);
            _currentIdx = index;
            DOTween.Sequence().Append(_descriptionText.DOFade(0.0f, 0.3f)).AppendCallback(() =>
            {
                _descriptionText.DOFade(1.0f, 0.3f);
                _descriptionText.text = _powerUpMappings.mappings[index].descriptionText;
            });
        }
    }
}