using BirthdayJobJam.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BirthdayJobJam.UI
{
    public sealed class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ApplicationScoreManager scoreManager;
        [SerializeField] private Text legacyText;
        [SerializeField] private string prefix = "APPLICATION SCORE: ";

        private void Reset()
        {
            legacyText = GetComponent<Text>();
        }

        private void Awake()
        {
            if (scoreManager == null)
                scoreManager = FindAnyObjectByType<ApplicationScoreManager>();
        }

        private void OnEnable()
        {
            if (scoreManager != null)
                scoreManager.ScoreChanged += UpdateText;

            UpdateText(scoreManager != null ? scoreManager.Score : 0);
        }

        private void OnDisable()
        {
            if (scoreManager != null)
                scoreManager.ScoreChanged -= UpdateText;
        }

        private void UpdateText(int score)
        {
            int maximum = scoreManager != null ? scoreManager.MaximumScore : 0;
            string text = maximum > 0 ? $"{prefix}{score}/{maximum}" : $"{prefix}{score}";

            if (legacyText != null)
                legacyText.text = text;
        }
    }
}
