using UnityEngine;

namespace ArBreakout.Common
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _backgroundTrack;
        
        public enum Sound
        {
            
        }
        
        
        public void PlaySound(Sound sound)
        {
            
        }

        public void MuteMusic(bool mute)
        {
            _backgroundTrack.mute = mute;
        }
    }
}