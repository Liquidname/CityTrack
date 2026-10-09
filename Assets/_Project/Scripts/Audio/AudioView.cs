using UnityEngine;

namespace _Project.Scripts.Audio
{
    public class AudioView : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        
        public AudioSource SfxSource => sfxSource;
    }
}