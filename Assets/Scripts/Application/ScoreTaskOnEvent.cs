using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ScoreTaskOnEvent : MonoBehaviour
    {
        [SerializeField] private ApplicationScoreManager scoreManager;
        [SerializeField] private ApplicationTaskDefinition task;
        [SerializeField, Range(0, 10)] private int points = 10;

        public void Score()
        {
            if (scoreManager == null)
                scoreManager = FindAnyObjectByType<ApplicationScoreManager>();

            if (scoreManager == null)
            {
                Debug.LogWarning("ScoreTaskOnEvent: no ApplicationScoreManager found.", this);
                return;
            }

            scoreManager.SetTaskScore(task, points);
        }
    }
}
