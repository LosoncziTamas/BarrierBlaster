using System.Threading.Tasks;
using ArBreakout.Common;
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
        [SerializeField] private Button _cancel;
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
            _cancel.onClick.AddListener(OnBackToMenuClick);
        }

        private void OnDisable()
        {
            _backToMenuButton.onClick.RemoveListener(OnBackToMenuClick);
            _retryButton.onClick.RemoveListener(OnRetryClick);
            _cancel.onClick.RemoveListener(OnBackToMenuClick);
        }

        private void OnRetryClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                Finish(true);
            });
        }

        private void OnBackToMenuClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                Finish(false);
            });
        }

        private void Finish(bool retry)
        {
            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 1.0f);
            _completionSource.SetResult(retry);
            gameObject.SetActive(false);
        }

        public Task<bool> Show(string stageName)
        {
            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 0.0f);
            _stageText.text = $"STAGE {stageName}";
            gameObject.SetActive(true);
            
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            
            _completionSource = new TaskCompletionSource<bool>();
            return _completionSource.Task;
        }
    }
}