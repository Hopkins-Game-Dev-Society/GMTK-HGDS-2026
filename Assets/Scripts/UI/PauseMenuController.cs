using BirthdayJobJam.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BirthdayJobJam.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        [Header("Timer")]
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private bool pauseTimer = true;

        [Header("UI")]
        [SerializeField] private CanvasGroup pauseCanvasGroup;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnToMenuButton;

        [Header("Scene Navigation")]
        [SerializeField] private string menuSceneName = "title";

        private bool isPaused;
        private bool timerWasRunningBeforePause;

        public bool IsPaused => isPaused;

        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();

            SetPaused(false, force: true);
        }

        private void OnEnable()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartCurrentScene);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnDisable()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(Resume);

            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartCurrentScene);

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.RemoveListener(ReturnToMenu);

            if (isPaused)
                Time.timeScale = 1f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
                TogglePause();
        }

        public void TogglePause()
        {
            SetPaused(!isPaused);
        }

        public void Resume()
        {
            SetPaused(false);
        }

        public void Pause()
        {
            SetPaused(true);
        }

        private void SetPaused(bool paused, bool force = false)
        {
            if (isPaused == paused && !force)
                return;

            isPaused = paused;

            if (paused)
            {
                timerWasRunningBeforePause = timer != null && timer.IsRunning;

                if (pauseTimer && timer != null)
                    timer.Pause();

                Time.timeScale = 0f;
                ShowGroup(pauseCanvasGroup);
            }
            else
            {
                Time.timeScale = 1f;
                HideGroup(pauseCanvasGroup);

                if (pauseTimer && timer != null && timerWasRunningBeforePause && !timer.HasExpired)
                    timer.Resume();

                timerWasRunningBeforePause = false;
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

        private static void ShowGroup(CanvasGroup group)
        {
            if (group == null)
                return;

            group.gameObject.SetActive(true);
            group.alpha = 1f;
            group.blocksRaycasts = true;
            group.interactable = true;
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
