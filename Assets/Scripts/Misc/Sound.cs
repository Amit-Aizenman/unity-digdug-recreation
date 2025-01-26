using UnityEngine;

namespace Misc
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        public float pitch;
        public bool loop;
        [Range(0f, 1f)]
        public float spatialBlend;
        [HideInInspector]
        public AudioSource source;
    }
}
