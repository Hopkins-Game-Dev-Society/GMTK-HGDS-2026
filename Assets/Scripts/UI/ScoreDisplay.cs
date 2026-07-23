using BirthdayJobJam.Application;
using TMPro;
using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ApplicationScoreManager scoreManager;
        [SerializeField] private TMP_Text text;
        [SerializeField] private string prefix = "APPLICATION SCORE: ";

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
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

            if (this.text != null)
                this.text.text = text;
        }
    }
}
