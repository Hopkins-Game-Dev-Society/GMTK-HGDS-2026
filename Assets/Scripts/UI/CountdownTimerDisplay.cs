using BirthdayJobJam.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BirthdayJobJam.UI
{
    public sealed class CountdownTimerDisplay : MonoBehaviour
    {
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private Text legacyText;
        [SerializeField] private string prefix = "TIME UNTIL 22: ";
        [SerializeField] private bool showCentiseconds = true;

        private void Reset()
        {
            legacyText = GetComponent<Text>();
        }

        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();
        }

        private void OnEnable()
        {
            if (timer != null)
                timer.SecondsRemainingChanged += UpdateText;

            UpdateText(timer != null ? timer.SecondsRemaining : 0f);
        }

        private void OnDisable()
        {
            if (timer != null)
                timer.SecondsRemainingChanged -= UpdateText;
        }

        public void UpdateText(float secondsRemaining)
        {
            string formatted = FormatTime(secondsRemaining);
            string text = $"{prefix}{formatted}";

            if (legacyText != null)
                legacyText.text = text;
        }

        private string FormatTime(float totalSeconds)
        {
            totalSeconds = Mathf.Max(0f, totalSeconds);

            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);

            if (!showCentiseconds)
                return $"{minutes:00}:{seconds:00}";

            int centiseconds = Mathf.FloorToInt(totalSeconds * 100f) % 100;
            return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
        }
    }
}
