using UnityEngine;

namespace _Project.Scripts.Audio
{
    public class AudioSystem
    {
        private AudioView _view;

        public AudioSystem(AudioView view)
        {
            _view = view;
        }
        
        public void PlaySound(AudioClip clip)
        {
            _view.SfxSource.PlayOneShot(clip);
        }
        
        public void PlaySound(AudioClip clip, float volume)
        {
            _view.SfxSource.PlayOneShot(clip, volume);
        }
    }
}