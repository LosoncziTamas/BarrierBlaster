using System;
using System.Threading.Tasks;
using ArBreakout.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class SettingsModal : MonoBehaviour
    {
        private TaskCompletionSource<bool> _tsc;
        
        [SerializeField] private DualStateButton _musicToggle;
        [SerializeField] private DualStateButton _soundToggle;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _cancel;
        [SerializeField] private Canvas _canvas;

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
            _canvas.enabled = true;
            return _tsc.Task;
        }
        
        private void DismissAndResume()
        {
            AudioPlayer.Instance.PlaySound(AudioPlayer.SoundType.Click);
            _canvas.enabled = false;
            _tsc.SetResult(false);
        }
        
    }
}