using System.Collections;
using BirthdayJobJam.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BirthdayJobJam.UI
{
    public sealed class TimerExpiredOverlay : MonoBehaviour
    {
        [Header("Timer")]
        [SerializeField] private GameplayTimer timer;

        [Header("Fade")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField, Min(0f)] private float fadeDurationSeconds = 1.25f;
        [SerializeField, Min(0f)] private float blackScreenHoldSeconds = 2f;

        [Header("End Screen")]
        [SerializeField] private CanvasGroup endScreenCanvasGroup;
        [TextArea] [SerializeField] private string endingMessage = "You were executed on your birthday.\nHappy birthday by the way!";
        [SerializeField] private TMP_Text endingMessageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnToMenuButton;

        [Header("Scene Navigation")]
        [SerializeField] private string menuSceneName = "title";

        private Coroutine sequence;

        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();

            HideGroup(fadeCanvasGroup);
            HideGroup(endScreenCanvasGroup);

            if (endingMessageText != null)
                endingMessageText.text = endingMessage;
        }

        private void OnEnable()
        {
            if (timer != null)
                timer.Expired += HandleTimerExpired;

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartCurrentScene);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnDisable()
        {
            if (timer != null)
                timer.Expired -= HandleTimerExpired;

            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartCurrentScene);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void HandleTimerExpired()
        {
            if (sequence != null)
                StopCoroutine(sequence);

            sequence = StartCoroutine(PlayExpiredSequence());
        }

        private IEnumerator PlayExpiredSequence()
        {
            Time.timeScale = 1f;

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.gameObject.SetActive(true);
                fadeCanvasGroup.blocksRaycasts = true;
                fadeCanvasGroup.interactable = false;

                float elapsed = 0f;
                while (elapsed < fadeDurationSeconds)
                {
                    elapsed += Time.unscaledDeltaTime;
                    fadeCanvasGroup.alpha = fadeDurationSeconds <= 0f
                        ? 1f
                        : Mathf.Clamp01(elapsed / fadeDurationSeconds);

                    yield return null;
                }

                fadeCanvasGroup.alpha = 1f;
            }

            if (blackScreenHoldSeconds > 0f)
                yield return new WaitForSecondsRealtime(blackScreenHoldSeconds);

            if (endScreenCanvasGroup != null)
            {
                endScreenCanvasGroup.gameObject.SetActive(true);
                endScreenCanvasGroup.alpha = 1f;
                endScreenCanvasGroup.blocksRaycasts = true;
                endScreenCanvasGroup.interactable = true;
            }
        }

        private void RestartCurrentScene()
        {
            Time.timeScale = 1f;
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        private void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        private static void HideGroup(CanvasGroup group)
        {
            if (group == null)
                return;

            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            group.gameObject.SetActive(false);
        }
    }
}
