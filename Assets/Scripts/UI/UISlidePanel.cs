using System.Collections;
using UnityEngine;

namespace BirthdayJobJam.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class UISlidePanel : MonoBehaviour
    {
        [SerializeField]
        private Vector2 hiddenPosition;

        [SerializeField]
        private Vector2 shownPosition;

        [SerializeField]
        private float animationTime = 0.35f;

        private RectTransform rectTransform;

        private Coroutine animation;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = hiddenPosition;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (animation != null)
                StopCoroutine(animation);

            animation = StartCoroutine(
                Animate(hiddenPosition, shownPosition));
        }

        public void Hide()
        {
            if (animation != null)
                StopCoroutine(animation);

            animation = StartCoroutine(
                HideRoutine());
        }

        private IEnumerator HideRoutine()
        {
            yield return Animate(shownPosition, hiddenPosition);

            gameObject.SetActive(false);
        }

        private IEnumerator Animate(Vector2 from, Vector2 to)
        {
            float elapsed = 0f;

            rectTransform.anchoredPosition = from;

            while (elapsed < animationTime)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / animationTime);

                // Ease-out
                t = 1f - Mathf.Pow(1f - t, 3f);

                rectTransform.anchoredPosition =
                    Vector2.Lerp(from, to, t);

                yield return null;
            }

            rectTransform.anchoredPosition = to;
        }
    }
}