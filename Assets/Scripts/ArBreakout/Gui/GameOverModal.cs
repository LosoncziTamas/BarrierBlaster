using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui
{
    public class GameOverModal : MonoBehaviour
    {
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
            _completionSource = new TaskCompletionSource<bool>();
            return _completionSource.Task;
        }
    }
}