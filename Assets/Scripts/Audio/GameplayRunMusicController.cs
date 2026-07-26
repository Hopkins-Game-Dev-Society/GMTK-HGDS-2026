using BirthdayJobJam.Core;
using JamAudioToolkit;
using UnityEngine;

namespace BirthdayJobJam.Audio
{
    public sealed class GameplayRunMusicController : MonoBehaviour
    {
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private JamMusicEvent gameplayTheme;
        [SerializeField] private bool restartOnTimerStarted = true;
        [SerializeField] private bool playIfTimerAlreadyRunning = true;
        [SerializeField] private bool stopBeforeRestart;
        [SerializeField] private bool stopOnTimerExpired = true;
        [SerializeField, Min(0f)] private float restartFadeOutSeconds;
        [SerializeField, Min(0f)] private float expiredFadeOutSeconds = 0.25f;

        private void Awake()
        {
            ResolveTimer();
        }

        private void OnEnable()
        {
            ResolveTimer();

            if (timer != null)
            {
                timer.Started += HandleTimerStarted;
                timer.Expired += HandleTimerExpired;
            }

            if (playIfTimerAlreadyRunning && timer != null && timer.IsRunning)
                RestartGameplayTheme();
        }

        private void OnDisable()
        {
            if (timer == null)
                return;

            timer.Started -= HandleTimerStarted;
            timer.Expired -= HandleTimerExpired;
        }

        [ContextMenu("Restart Gameplay Theme")]
        public void RestartGameplayTheme()
        {
            if (gameplayTheme == null)
            {
                Debug.LogWarning("GameplayRunMusicController has no gameplay theme assigned.", this);
                return;
            }

            if (stopBeforeRestart)
                JamAudio.StopMusic(restartFadeOutSeconds);

            JamAudio.PlayMusic(gameplayTheme);
        }

        [ContextMenu("Stop Gameplay Theme")]
        public void StopGameplayTheme()
        {
            JamAudio.StopMusic(expiredFadeOutSeconds);
        }

        private void HandleTimerStarted()
        {
            if (restartOnTimerStarted)
                RestartGameplayTheme();
        }

        private void HandleTimerExpired()
        {
            if (stopOnTimerExpired)
                StopGameplayTheme();
        }

        private void ResolveTimer()
        {
            if (timer != null)
                return;

            timer = FindAnyObjectByType<GameplayTimer>();
        }
    }
}
