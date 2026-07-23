using BirthdayJobJam.Core;
using TMPro;
using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class CountdownTimerDisplay : MonoBehaviour
    {
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private TMP_Text text;
        [SerializeField] private string prefix = "TIME UNTIL 22: ";
        [SerializeField] private bool showCentiseconds = true;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
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

            if (this.text != null)
                this.text.text = text;
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
