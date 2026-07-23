using BirthdayJobJam.Core;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class TimerExpiryEndingDriver : MonoBehaviour
    {
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private ApplicationScoreManager scoreManager;
        [SerializeField] private GameEvent timeExpiredEndingStarted;

        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();

            if (scoreManager == null)
                scoreManager = FindAnyObjectByType<ApplicationScoreManager>();
        }

        private void OnEnable()
        {
            if (timer != null)
                timer.Expired += HandleTimerExpired;
        }

        private void OnDisable()
        {
            if (timer != null)
                timer.Expired -= HandleTimerExpired;
        }

        private void HandleTimerExpired()
        {
            scoreManager?.ResolveEnding();
            timeExpiredEndingStarted?.Raise();
        }
    }
}
