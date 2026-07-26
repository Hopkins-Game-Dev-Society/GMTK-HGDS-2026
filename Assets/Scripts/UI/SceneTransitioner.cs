using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BirthdayJobJam.UI
{
    public sealed class SceneTransitioner : MonoBehaviour
    {
        private const float DefaultFadeOutSeconds = 1f;
        private const float DefaultBlackHoldSeconds = 0.15f;
        private const float DefaultFadeInSeconds = 1f;
        private const int DefaultSortingOrder = 5000;

        private static SceneTransitioner instance;

        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private Coroutine transition;

        public static void LoadScene(
            string sceneName,
            float fadeOutSeconds = DefaultFadeOutSeconds,
            float blackHoldSeconds = DefaultBlackHoldSeconds,
            float fadeInSeconds = DefaultFadeInSeconds,
            int sortingOrder = DefaultSortingOrder)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                return;

            EnsureInstance(sortingOrder).StartSceneLoad(sceneName, fadeOutSeconds, blackHoldSeconds, fadeInSeconds);
        }

        public static void ReloadActiveScene(
            float fadeOutSeconds = DefaultFadeOutSeconds,
            float blackHoldSeconds = DefaultBlackHoldSeconds,
            float fadeInSeconds = DefaultFadeInSeconds,
            int sortingOrder = DefaultSortingOrder)
        {
            LoadScene(SceneManager.GetActiveScene().name, fadeOutSeconds, blackHoldSeconds, fadeInSeconds, sortingOrder);
        }

        private static SceneTransitioner EnsureInstance(int sortingOrder)
        {
            if (instance != null)
            {
                instance.SetSortingOrder(sortingOrder);
                return instance;
            }

            GameObject transitionObject = new GameObject(
                "Scene Transitioner",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(CanvasGroup),
                typeof(SceneTransitioner));

            DontDestroyOnLoad(transitionObject);

            instance = transitionObject.GetComponent<SceneTransitioner>();
            instance.Initialize(sortingOrder);
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize(DefaultSortingOrder);
        }

        private void Initialize(int sortingOrder)
        {
            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            if (transform.childCount == 0)
            {
                GameObject fadeObject = new GameObject("Black Fade", typeof(RectTransform), typeof(Image));
                fadeObject.transform.SetParent(transform, false);

                RectTransform rect = fadeObject.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                Image image = fadeObject.GetComponent<Image>();
                image.color = Color.black;
                image.raycastTarget = true;
            }

            gameObject.SetActive(false);
        }

        private void SetSortingOrder(int sortingOrder)
        {
            if (canvas != null)
                canvas.sortingOrder = sortingOrder;
        }

        private void StartSceneLoad(string sceneName, float fadeOutSeconds, float blackHoldSeconds, float fadeInSeconds)
        {
            gameObject.SetActive(true);

            if (transition != null)
                StopCoroutine(transition);

            transition = StartCoroutine(LoadSceneRoutine(
                sceneName,
                Mathf.Max(0f, fadeOutSeconds),
                Mathf.Max(0f, blackHoldSeconds),
                Mathf.Max(0f, fadeInSeconds)));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float fadeOutSeconds, float blackHoldSeconds, float fadeInSeconds)
        {
            Time.timeScale = 1f;
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = false;

            yield return FadeTo(1f, fadeOutSeconds);

            if (blackHoldSeconds > 0f)
                yield return new WaitForSecondsRealtime(blackHoldSeconds);

            SceneManager.LoadScene(sceneName);
            yield return null;

            yield return FadeTo(0f, fadeInSeconds);

            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            transition = null;
        }

        private IEnumerator FadeTo(float targetAlpha, float durationSeconds)
        {
            if (durationSeconds <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                yield break;
            }

            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < durationSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / durationSeconds));
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}
