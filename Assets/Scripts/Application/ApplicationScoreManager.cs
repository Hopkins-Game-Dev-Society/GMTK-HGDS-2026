using System;
using System.Collections.Generic;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationScoreManager : MonoBehaviour
    {
        [SerializeField] private List<EndingDefinition> endings = new List<EndingDefinition>();

        [Header("Events")]
        [SerializeField] private FloatGameEvent scoreChanged;
        [SerializeField] private StringGameEvent endingResolved;

        private readonly Dictionary<string, int> taskScores = new Dictionary<string, int>();
        private readonly Dictionary<string, int> taskMaximums = new Dictionary<string, int>();

        public event Action<int> ScoreChanged;
        public event Action<EndingDefinition> EndingResolved;

        public int Score { get; private set; }
        public int MaximumScore { get; private set; }

        public void SetTaskScore(ApplicationTaskDefinition task, int points)
        {
            if (task == null)
            {
                Debug.LogWarning("ApplicationScoreManager: attempted to score a null task.", this);
                return;
            }

            string id = task.TaskId;
            int clampedPoints = Mathf.Clamp(points, 0, task.MaxPoints);

            taskScores[id] = clampedPoints;
            taskMaximums[id] = task.MaxPoints;
            RecalculateScore();
        }

        public void AddLoosePoints(int points)
        {
            Score = Mathf.Max(0, Score + points);
            ScoreChanged?.Invoke(Score);
            scoreChanged?.Raise(Score);
        }

        public void ClearScores()
        {
            taskScores.Clear();
            taskMaximums.Clear();
            Score = 0;
            MaximumScore = 0;
            ScoreChanged?.Invoke(Score);
            scoreChanged?.Raise(Score);
        }

        public EndingDefinition ResolveEnding()
        {
            EndingDefinition best = null;

            foreach (EndingDefinition ending in endings)
            {
                if (ending == null || Score < ending.MinimumScore)
                    continue;

                if (best == null || ending.MinimumScore > best.MinimumScore)
                    best = ending;
            }

            EndingResolved?.Invoke(best);
            endingResolved?.Raise(best != null ? best.EndingId : string.Empty);

            return best;
        }

        private void RecalculateScore()
        {
            int scoreTotal = 0;
            int maximumTotal = 0;

            foreach (int value in taskScores.Values)
                scoreTotal += value;

            foreach (int value in taskMaximums.Values)
                maximumTotal += value;

            Score = scoreTotal;
            MaximumScore = maximumTotal;
            ScoreChanged?.Invoke(Score);
            scoreChanged?.Raise(Score);
        }
    }
}
