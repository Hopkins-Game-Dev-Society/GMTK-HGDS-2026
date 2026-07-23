using System;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Core
{
    public sealed class GameplayTimer : MonoBehaviour
    {
        [Header("Timer")]
        [SerializeField, Min(1f)] private float durationSeconds = 300f;
        [SerializeField] private bool startOnStart = true;
        [SerializeField] private bool useUnscaledTime;

        [Header("Runtime Debug")]
        [SerializeField, Min(0f)] private float secondsRemaining;
        [SerializeField] private bool allowInspectorTimeEditsInPlayMode = true;

        [Header("Events")]
        [SerializeField] private GameEvent timerStarted;
        [SerializeField] private GameEvent timerPaused;
        [SerializeField] private GameEvent timerResumed;
        [SerializeField] private GameEvent timerExpired;
        [SerializeField] private FloatGameEvent secondsRemainingChanged;
        [SerializeField] private FloatGameEvent normalizedRemainingChanged;

        private bool isRunning;
        private bool hasExpired;
        private float lastValidatedDurationSeconds;
        private float lastValidatedSecondsRemaining;

        public event Action Started;
        public event Action Paused;
        public event Action Resumed;
        public event Action Expired;
        public event Action<float> SecondsRemainingChanged;
        public event Action<float> NormalizedRemainingChanged;

        public float DurationSeconds
        {
            get => durationSeconds;
            set => SetDurationSeconds(value, resetRemaining: false);
        }

        public float SecondsRemaining
        {
            get => secondsRemaining;
            set => SetSecondsRemaining(value);
        }

        public float SecondsElapsed => Mathf.Max(0f, durationSeconds - secondsRemaining);
        public float NormalizedRemaining => durationSeconds <= 0f ? 0f : Mathf.Clamp01(secondsRemaining / durationSeconds);
        public bool IsRunning => isRunning;
        public bool HasExpired => hasExpired;
        public bool StartOnStart => startOnStart;

        private void Awake()
        {
            ResetTimer();
        }

        private void OnValidate()
        {
            durationSeconds = Mathf.Max(1f, durationSeconds);
            secondsRemaining = Mathf.Clamp(secondsRemaining, 0f, durationSeconds);

            if (!global::UnityEngine.Application.isPlaying || !allowInspectorTimeEditsInPlayMode)
            {
                CacheValidatedTime();
                return;
            }

            bool durationChanged = !Mathf.Approximately(durationSeconds, lastValidatedDurationSeconds);
            bool remainingChanged = !Mathf.Approximately(secondsRemaining, lastValidatedSecondsRemaining);

            if (!durationChanged && !remainingChanged)
                return;

            if (secondsRemaining > 0f && hasExpired)
                hasExpired = false;

            if (secondsRemaining <= 0f && !hasExpired)
            {
                CacheValidatedTime();
                Expire();
                return;
            }

            CacheValidatedTime();
            RaiseTimeChanged();
        }

        private void Start()
        {
            if (startOnStart)
                StartTimer();
        }

        private void Update()
        {
            if (!isRunning || hasExpired)
                return;

            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            SetSecondsRemaining(secondsRemaining - delta);

            if (secondsRemaining <= 0f)
                Expire();
        }

        public void StartTimer()
        {
            StartTimer(resetToFullDuration: false);
        }

        public void StartTimer(bool resetToFullDuration)
        {
            hasExpired = false;

            if (resetToFullDuration || secondsRemaining <= 0f)
                SetSecondsRemaining(durationSeconds);

            isRunning = true;
            Started?.Invoke();
            timerStarted?.Raise();
            RaiseTimeChanged();
        }

        public void StopTimer()
        {
            if (!isRunning)
                return;

            isRunning = false;
            Paused?.Invoke();
            timerPaused?.Raise();
        }

        public void Pause()
        {
            if (!isRunning || hasExpired)
                return;

            isRunning = false;
            Paused?.Invoke();
            timerPaused?.Raise();
        }

        public void Resume()
        {
            if (isRunning || hasExpired)
                return;

            isRunning = true;
            Resumed?.Invoke();
            timerResumed?.Raise();
        }

        public void ResetTimer()
        {
            hasExpired = false;
            isRunning = false;
            SetSecondsRemaining(durationSeconds);
        }

        public void SetDurationSeconds(float value, bool resetRemaining = true)
        {
            durationSeconds = Mathf.Max(1f, value);

            if (resetRemaining)
                SetSecondsRemaining(durationSeconds);
            else
                SetSecondsRemaining(Mathf.Min(secondsRemaining, durationSeconds));

            CacheValidatedTime();
        }

        public void AddSeconds(float seconds)
        {
            SetSecondsRemaining(secondsRemaining + seconds);
        }

        public void SetSecondsRemaining(float value)
        {
            float clamped = Mathf.Clamp(value, 0f, durationSeconds);
            if (Mathf.Approximately(secondsRemaining, clamped))
            {
                if (clamped <= 0f && !hasExpired)
                    Expire();

                return;
            }

            secondsRemaining = clamped;
            CacheValidatedTime();
            RaiseTimeChanged();

            if (secondsRemaining <= 0f && !hasExpired)
                Expire();
        }

        [ContextMenu("Timer/Set Remaining To 10 Seconds")]
        public void SetRemainingToTenSeconds()
        {
            SetSecondsRemaining(10f);
        }

        [ContextMenu("Timer/Expire Now")]
        public void ExpireNow()
        {
            SetSecondsRemaining(0f);
            Expire();
        }

        [ContextMenu("Timer/Reset And Start")]
        public void ResetAndStart()
        {
            ResetTimer();
            StartTimer();
        }

        private void Expire()
        {
            if (hasExpired)
                return;

            hasExpired = true;
            isRunning = false;
            SetSecondsRemainingWithoutExpiring(0f);
            Expired?.Invoke();
            timerExpired?.Raise();
        }

        private void RaiseTimeChanged()
        {
            SecondsRemainingChanged?.Invoke(secondsRemaining);
            NormalizedRemainingChanged?.Invoke(NormalizedRemaining);
            secondsRemainingChanged?.Raise(secondsRemaining);
            normalizedRemainingChanged?.Raise(NormalizedRemaining);
        }

        private void CacheValidatedTime()
        {
            lastValidatedDurationSeconds = durationSeconds;
            lastValidatedSecondsRemaining = secondsRemaining;
        }

        private void SetSecondsRemainingWithoutExpiring(float value)
        {
            float clamped = Mathf.Clamp(value, 0f, durationSeconds);
            if (Mathf.Approximately(secondsRemaining, clamped))
                return;

            secondsRemaining = clamped;
            CacheValidatedTime();
            RaiseTimeChanged();
        }
    }
}
