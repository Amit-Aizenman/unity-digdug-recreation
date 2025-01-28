using System;
using Misc;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        public Sound[] sounds;
        public static SoundManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = s.spatialBlend;
            }
        }

        void Start()
        {
            Play("walkingSound");
            Pause("walkingSound");
        }

        public void Play(string soundName)
        {
            Debug.Log(soundName);
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            s.source.Play();
        }

        public void Pause(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            s.source.Pause();
        }

        public void UnPause(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            s.source.UnPause();
        }

        public AudioClip getSoundClip(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            return s.clip;
        }
    }
}