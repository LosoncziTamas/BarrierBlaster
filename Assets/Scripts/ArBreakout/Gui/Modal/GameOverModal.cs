using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class GameOverModal : MonoBehaviour
    {
        public Vector3 HiddenPosition;
        public float AnimDuration;
        public Ease Ease;
        
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _overlay;
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _stageText;

        private TaskCompletionSource<bool> _completionSource;

        private void Awake()
        {
            _root.anchoredPosition = HiddenPosition;
        }

        private void OnEnable()
        {
            _backToMenuButton.onClick.AddListener(OnBackToMenuClick);
            _retryButton.onClick.AddListener(OnRetryClick);
            _closeButton.onClick.AddListener(OnRetryClick);
        }

        private void OnDisable()
        {
            _backToMenuButton.onClick.RemoveListener(OnBackToMenuClick);
            _retryButton.onClick.RemoveListener(OnRetryClick);
            _closeButton.onClick.RemoveListener(OnRetryClick);
        }

        private async void OnGUI()
        {
            GUILayout.Space(300);
            if (GUILayout.Button("GameOverModal"))
            {
                await Show("IV");
            }
        }

        private void OnRetryClick()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                Finish(true);
            });
        }

        private void OnBackToMenuClick()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                Finish(false);
            });
        }

        private void Finish(bool retry)
        {
            _completionSource.SetResult(retry);
            _canvas.enabled = false;
        }

        public Task<bool> Show(string stageName)
        {
            _stageText.text = $"STAGE {stageName}";
            _canvas.enabled = true;
            
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            
            _completionSource = new TaskCompletionSource<bool>();
            return _completionSource.Task;
        }
    }
}