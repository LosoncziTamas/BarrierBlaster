using System;
using System.Collections.Generic;
using DG.Tweening;
using Possible;
using UnityEngine;

namespace BarrierBlaster.Common
{
    public class AudioPlayer : SingletonBehaviour<AudioPlayer>
    {
        private const string MusicIsMutedKey = "music_is_muted";
        private const string SoundsAreMutedKey = "sounds_are_muted";
        
        [SerializeField] private AudioSource _backgroundTrack;
        [SerializeField] private List<SoundEntry> _soundBank;

        public bool MusicIsMuted
        {
            set
            {
                _backgroundTrack.mute = value;
                var result = Convert.ToInt32(value);
                PlayerPrefs.SetInt(MusicIsMutedKey, result);
            }
            get => Convert.ToBoolean(PlayerPrefs.GetInt(MusicIsMutedKey, 0));
        }

        public bool SoundsAreMuted
        {
            set
            {
                if (value)
                {
                    StopPlayingSounds();
                }
                var result = Convert.ToInt32(value);
                PlayerPrefs.SetInt(SoundsAreMutedKey, result);
            }
            get => Convert.ToBoolean(PlayerPrefs.GetInt(SoundsAreMutedKey, 0));
        }

        [Serializable]
        public class SoundEntry
        {
            public SoundType SoundType;
            public AudioSource AudioSource;
        }
        
        [Serializable]
        public enum SoundType
        {
            Pop,
            Hit,
            Click,
            Laser,
            Launch,
            Appear,
            ModalAppear,
            Death,
            Trophy,
            WallHit
        }

        protected override void Awake()
        {
            base.Awake();
            _backgroundTrack.mute = MusicIsMuted;
            if (MusicIsMuted)
            {
                return;
            }
            _backgroundTrack.volume = 0f;
            _backgroundTrack.DOFade(1.0f, 1.0f);
        }

        public void PlaySound(SoundType sound)
        {
            if (SoundsAreMuted)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.loop = false;
            entry.AudioSource.Play();
        }

        public void PlaySoundLooped(SoundType sound)
        {
            if (SoundsAreMuted)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.loop = true;
            entry.AudioSource.Play();
        }
        
        public void StopSound(SoundType sound)
        {
            if (SoundsAreMuted)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.Stop();
        }

        public void SetVolume(SoundType sound, float volume)
        {
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.volume = volume;
        }

        private void StopPlayingSounds()
        {
            foreach (var entry in _soundBank)
            {
                if (entry.AudioSource.isPlaying)
                {
                    entry.AudioSource.Stop();
                }
            }
        }
    }
}