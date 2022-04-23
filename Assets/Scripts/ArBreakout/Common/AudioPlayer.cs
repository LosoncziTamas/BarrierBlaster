using System;
using System.Collections.Generic;
using Possible;
using UnityEngine;

namespace ArBreakout.Common
{
    public class AudioPlayer : SingletonBehaviour<AudioPlayer>
    {
        [SerializeField] private AudioSource _backgroundTrack;
        [SerializeField] private List<SoundEntry> _soundBank;

        private bool _muteSounds;

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
            Trophy
        }

        public void PlaySound(SoundType sound)
        {
            if (_muteSounds)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.loop = false;
            entry.AudioSource.Play();
        }

        public void PlaySoundLooped(SoundType sound)
        {
            if (_muteSounds)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.loop = true;
            entry.AudioSource.Play();
        }
        
        public void StopSound(SoundType sound)
        {
            if (_muteSounds)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.Stop();
        }

        public void MuteMusic(bool mute)
        {
            _backgroundTrack.mute = mute;
        }

        public void MuteSound(bool mute)
        {
            _muteSounds = mute;
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