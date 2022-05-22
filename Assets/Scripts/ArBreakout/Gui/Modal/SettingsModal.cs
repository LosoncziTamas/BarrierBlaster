using System.Threading.Tasks;
using ArBreakout.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class SettingsModal : MonoBehaviour
    {
        private TaskCompletionSource<bool> _tsc;
        
        public Vector3 HiddenPosition;
        public float AnimDuration;
        public Ease Ease;
        
        [SerializeField] private DualStateButton _musicToggle;
        [SerializeField] private DualStateButton _soundToggle;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _cancel;
        [SerializeField] private GameObject _root;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Image _overlay;

        private void Awake()
        {
            _panel.anchoredPosition = HiddenPosition;
        }

        private void OnEnable()
        {
            _musicToggle.OnClick.AddListener(OnMusicToggleClick);
            _soundToggle.OnClick.AddListener(OnSoundToggleClick);
            _backButton.onClick.AddListener(DismissAndResume);
            _cancel.onClick.AddListener(DismissAndResume);
        }
        
        private void OnDisable()
        {
            _musicToggle.OnClick.RemoveListener(OnMusicToggleClick);
            _soundToggle.OnClick.RemoveListener(OnSoundToggleClick);
            _backButton.onClick.RemoveListener(DismissAndResume);
            _cancel.onClick.RemoveListener(DismissAndResume);
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

        public Task Show()
        {
            _tsc = new TaskCompletionSource<bool>();
            _root.SetActive(true);
            var audioPlayer = AudioPlayer.Instance;
            _musicToggle.SetState(on: !audioPlayer.MusicIsMuted);
            _soundToggle.SetState(on: !audioPlayer.SoundsAreMuted);

            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            
            return _tsc.Task;
        }
        
        private void DismissAndResume()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration).SetEase(Ease).OnComplete(() =>
            {
                AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
                _root.SetActive(false);
                _tsc.SetResult(false);
            });
        }
    }
}