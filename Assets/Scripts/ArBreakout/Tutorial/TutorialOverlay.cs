using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
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

        public Vector3 HiddenPosition;
        public float AnimDuration;
        public Ease Ease;
        
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _musicToggle;
        [SerializeField] private Button _soundToggle;
        [SerializeField] private Canvas _tutorialCanvas;
        [SerializeField] private RectTransform _panel;

        private TaskCompletionSource<ReturnState> _taskCompletionSource;
        private int _currentIdx;

        private void Awake()
        {
            _panel.anchoredPosition = HiddenPosition;
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _closeButton.onClick.AddListener(DismissAndResume);
            _continueButton.onClick.AddListener(DismissAndResume);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _closeButton.onClick.RemoveListener(DismissAndResume);
            _continueButton.onClick.RemoveListener(DismissAndResume);
        }

        private async void OnGUI()
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Show Pause"))
            {
                await Show();
            }
        }

        public Task<ReturnState> Show()
        {
            _tutorialCanvas.enabled = true;
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            Debug.Assert(_taskCompletionSource == null);
            _taskCompletionSource = new TaskCompletionSource<ReturnState>();
            return _taskCompletionSource.Task;
        }
        
        public void DismissAndResume()
        {
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                _tutorialCanvas.enabled = false;
                _taskCompletionSource.SetResult(ReturnState.Game);
                _taskCompletionSource = null;
            });
        }

        private void OnBackButtonClick()
        {
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                _tutorialCanvas.enabled = false;
                _taskCompletionSource.SetResult(ReturnState.MainMenu);
                _taskCompletionSource = null;
            });
        }
    }
}