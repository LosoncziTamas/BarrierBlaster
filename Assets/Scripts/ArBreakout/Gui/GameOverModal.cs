using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui
{
    public class GameOverModal : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _backToMenuButton;

        private TaskCompletionSource<bool> _completionSource;

        private void OnEnable()
        {
            _backToMenuButton.onClick.AddListener(OnBackToMenuClick);
            _retryButton.onClick.AddListener(OnRetryClick);
        }

        private void OnDisable()
        {
            _backToMenuButton.onClick.RemoveListener(OnBackToMenuClick);
            _retryButton.onClick.RemoveListener(OnRetryClick);
        }

        private void OnRetryClick()
        {
            _completionSource.SetResult(true);
            gameObject.SetActive(false);
        }

        private void OnBackToMenuClick()
        {
            _completionSource.SetResult(false);
            gameObject.SetActive(false);
        }

        public Task<bool> Show()
        {
            gameObject.SetActive(true);
            _panel.DOPunchScale(Vector3.one * 0.2f, 0.4f);
            _completionSource = new TaskCompletionSource<bool>();
            return _completionSource.Task;
        }
    }
}