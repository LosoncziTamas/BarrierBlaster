using System.Threading.Tasks;
using ArBreakout.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public partial class PauseModal : MonoBehaviour
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
        [SerializeField] private Button _continueButton;
        [SerializeField] private DualStateButton _musicToggle;
        [SerializeField] private DualStateButton _soundToggle;
        [SerializeField] private Button _cancel;
        [SerializeField] private GameObject _root;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Image _overlay;
        [SerializeField] private TextMeshProUGUI _title;

        private TaskCompletionSource<ReturnState> _taskCompletionSource;

        private void Awake()
        {
            _panel.anchoredPosition = HiddenPosition;
            var audioPlayer = AudioPlayer.Instance;
            _musicToggle.SetState(on: !audioPlayer.MusicIsMuted);
            _soundToggle.SetState(on: !audioPlayer.SoundsAreMuted);
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _continueButton.onClick.AddListener(DismissAndResume);
            _cancel.onClick.AddListener(DismissAndResume);
            _musicToggle.OnClick.AddListener(OnMusicToggleClick);
            _soundToggle.OnClick.AddListener(OnSoundToggleClick);
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _continueButton.onClick.RemoveListener(DismissAndResume);
            _cancel.onClick.RemoveListener(DismissAndResume);
            _musicToggle.OnClick.RemoveListener(OnMusicToggleClick);
            _soundToggle.OnClick.RemoveListener(OnSoundToggleClick);
        }

        private void OnMusicToggleClick()
        {
            var audioPlayer = AudioPlayer.Instance;
            audioPlayer.PlaySound(AudioPlayer.SoundType.Click);
            audioPlayer.MusicIsMuted = !audioPlayer.MusicIsMuted;
            _musicToggle.SetState(on: !audioPlayer.MusicIsMuted);
        }
        
        private void OnSoundToggleClick()
        {
            var audioPlayer = AudioPlayer.Instance;
            audioPlayer.PlaySound(AudioPlayer.SoundType.Click);
            audioPlayer.SoundsAreMuted = !audioPlayer.SoundsAreMuted;
            _soundToggle.SetState(on: !audioPlayer.SoundsAreMuted);
        }
        
        public Task<ReturnState> Show(string stageName)
        {
            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 0.0f);
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.ModalAppear);
            _root.SetActive(true);
            _title.text = $"STAGE {stageName}";
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            Debug.Assert(_taskCompletionSource == null);
            _taskCompletionSource = new TaskCompletionSource<ReturnState>();
            return _taskCompletionSource.Task;
        }

        private void OnHidden(ReturnState returnState)
        {
            _taskCompletionSource.SetResult(returnState);
            _taskCompletionSource = null;
            AudioPlayer.Instance.SetVolume(AudioPlayer.SoundType.Laser, 1.0f);
            _root.SetActive(false);
        }
        
        private void DismissAndResume()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration)
                .SetEase(Ease)
                .OnComplete(() => OnHidden(ReturnState.Game));
        }

        private void OnBackButtonClick()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration)
                .SetEase(Ease)
                .OnComplete(() => OnHidden(ReturnState.MainMenu));
        }
    }
}