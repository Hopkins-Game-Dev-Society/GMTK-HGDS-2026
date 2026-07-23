using JamAudioToolkit;
using UnityEngine;

namespace BirthdayJobJam.Audio
{
    public sealed class JamAudioEventRelay : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private JamSoundEvent soundEvent;
        [SerializeField] private Transform soundPosition;

        [Header("Music")]
        [SerializeField] private JamMusicEvent musicEvent;

        public void PlaySound()
        {
            if (soundEvent == null)
            {
                Debug.LogWarning("JamAudioEventRelay: no sound event assigned.", this);
                return;
            }

            if (soundPosition != null)
                JamAudio.Play(soundEvent, soundPosition);
            else
                JamAudio.Play(soundEvent);
        }

        public void PlaySoundAtSelf()
        {
            if (soundEvent == null)
            {
                Debug.LogWarning("JamAudioEventRelay: no sound event assigned.", this);
                return;
            }

            JamAudio.Play(soundEvent, transform);
        }

        public void PlayMusic()
        {
            if (musicEvent == null)
            {
                Debug.LogWarning("JamAudioEventRelay: no music event assigned.", this);
                return;
            }

            JamAudio.PlayMusic(musicEvent);
        }

        public void StopMusic()
        {
            JamAudio.StopMusic();
        }

        public void PauseMusic()
        {
            JamAudio.PauseMusic();
        }

        public void ResumeMusic()
        {
            JamAudio.ResumeMusic();
        }
    }
}
