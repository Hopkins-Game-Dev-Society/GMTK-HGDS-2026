using BirthdayJobJam.Core;
using TMPro;
using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class CountdownClockDisplay : MonoBehaviour
    {
        [SerializeField]
        private GameplayTimer timer;

        [SerializeField]
        private TMP_Text text;

        [Header("Display")]
        [SerializeField]
        private bool use24HourTime = false;


        private float initialTimerSeconds;


        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }


        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();
        }


        private void Start()
        {
            if (timer != null)
            {
                initialTimerSeconds = timer.SecondsRemaining;
            }
        }


        private void OnEnable()
        {
            if (timer != null)
                timer.SecondsRemainingChanged += UpdateClock;


            UpdateClock(
                timer != null
                    ? timer.SecondsRemaining
                    : 0f
            );
        }


        private void OnDisable()
        {
            if (timer != null)
                timer.SecondsRemainingChanged -= UpdateClock;
        }


        private void UpdateClock(float secondsRemaining)
        {
            // How much time has passed since the countdown began
            float elapsed =
                initialTimerSeconds - secondsRemaining;


            // Clock starts at midnight minus the countdown length
            float currentTime =
                (24f * 60f * 60f)
                - initialTimerSeconds
                + elapsed;


            // Wrap around if needed
            currentTime %= 24f * 60f * 60f;


            int hours =
                Mathf.FloorToInt(currentTime / 3600f);

            int minutes =
                Mathf.FloorToInt(
                    (currentTime % 3600f) / 60f);


            if (text == null)
                return;


            if (use24HourTime)
            {
                text.text =
                    $"{hours:00}:{minutes:00}";
            }
            else
            {
                string suffix =
                    hours >= 12 ? "PM" : "AM";

                int displayHour =
                    hours % 12;

                if (displayHour == 0)
                    displayHour = 12;


                text.text =
                    $"{displayHour:00}:{minutes:00} {suffix}";
            }
        }
    }
}