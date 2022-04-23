using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Common
{
    public class AudioPlayer : MonoBehaviour
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
            Laser
        }

        private void OnGUI()
        {
            if (GUILayout.Button(SoundType.Click.ToString()))
            {
                PlaySound(SoundType.Click);
            }
            if (GUILayout.Button(SoundType.Pop.ToString()))
            {
                PlaySound(SoundType.Pop);
            }
            if (GUILayout.Button(SoundType.Hit.ToString()))
            {
                PlaySound(SoundType.Hit);
            }
            if (GUILayout.Button(SoundType.Laser.ToString()))
            {
                PlaySound(SoundType.Laser);
            }
        }

        public void PlaySound(SoundType sound)
        {
            if (_muteSounds)
            {
                return;
            }
            
            var entry = _soundBank.Find(entry => entry.SoundType == sound);
            entry.AudioSource.Play();
        }

        public void MuteMusic(bool mute)
        {
            _backgroundTrack.mute = mute;
        }

        public void MuteSound(bool mute)
        {
            _muteSounds = mute;
        }
    }
}