using System;
using System.Collections.Generic;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //This keeps the result for each answer internal and gives the rest of the game one overall score from 0 to 10.
    public sealed class ApplicationScoreManager : MonoBehaviour
    {
        private const string QuestionContributionPrefix = "question:";
        private const string TaskContributionPrefix = "task:";

        [SerializeField] private List<EndingDefinition> endings = new List<EndingDefinition>();

        [Header("Application Session")]
        [SerializeField] private ApplicationSessionManager sessionManager;

        [Header("Events")]
        [SerializeField] private FloatGameEvent scoreChanged;
        [SerializeField] private StringGameEvent endingResolved;

        [Header("Runtime State (Inspector Only)")]
        [SerializeField, Range(0, 10)] private int applicationScore;
        [SerializeField] private bool hasFinalScore;
        [SerializeField] private int answeredQuestionCount;

        private readonly Dictionary<string, int> scoreContributions = new Dictionary<string, int>();
        private readonly Dictionary<string, int> scoreMaximums = new Dictionary<string, int>();
        private readonly Dictionary<string, ApplicationQuestionAnswerRecord> questionAnswersBySlot =
            new Dictionary<string, ApplicationQuestionAnswerRecord>(StringComparer.OrdinalIgnoreCase);
        private readonly List<ApplicationQuestionAnswerRecord> questionAnswers =
            new List<ApplicationQuestionAnswerRecord>();

        public event Action<int> ScoreChanged;
        public event Action<EndingDefinition> EndingResolved;

        public int Score => applicationScore;
        public int MaximumScore => 10;
        public bool HasFinalScore => hasFinalScore;
        public IReadOnlyList<ApplicationQuestionAnswerRecord> QuestionAnswers => questionAnswers;

        private void Awake()
        {
            ResolveSessionManager();
        }

        private void OnEnable()
        {
            ResolveSessionManager();

            if (sessionManager != null)
                sessionManager.SessionGenerated += HandleSessionGenerated;
        }

        private void Start()
        {
            ResolveSessionManager();

            if (sessionManager != null && sessionManager.Current != null)
                HandleSessionGenerated(sessionManager.Current);
        }

        private void OnDisable()
        {
            if (sessionManager != null)
                sessionManager.SessionGenerated -= HandleSessionGenerated;
        }

        public bool RecordQuestionAnswer(string slotId, string answerId)
        {
            ResolveSessionManager();

            ApplicationQuestionRuntimeData question =
                sessionManager != null
                    ? sessionManager.FindQuestionBySlot(slotId)
                    : null;

            if (question == null
                || !question.Definition.TryGetAnswer(
                    answerId,
                    out ApplicationQuestionAnswerDefinition answer))
            {
                return false;
            }

            //If the same slot gets answered again I replace the result that was there before.
            questionAnswersBySlot[question.SlotId] =
                new ApplicationQuestionAnswerRecord(
                    question.SlotId,
                    question.QuestionId,
                    answer.AnswerId,
                    answer.Rating,
                    question.Weight);

            SetContribution(
                QuestionContributionPrefix + question.SlotId,
                answer.Rating * question.Weight,
                MaximumScore * question.Weight);

            RebuildQuestionAnswerSnapshot();
            UpdateQuestionCompletion();
            RecalculateScore();
            return true;
        }

        public void SetTaskScore(ApplicationTaskDefinition task, int points)
        {
            if (task == null)
            {
                Debug.LogWarning("ApplicationScoreManager: attempted to score a null task.", this);
                return;
            }

            int clampedPoints = Mathf.Clamp(points, 0, task.MaxPoints);

            SetContribution(
                TaskContributionPrefix + task.TaskId,
                clampedPoints,
                task.MaxPoints);
            RecalculateScore();
        }

        public void ClearScores()
        {
            scoreContributions.Clear();
            scoreMaximums.Clear();
            questionAnswersBySlot.Clear();
            questionAnswers.Clear();
            hasFinalScore = false;
            answeredQuestionCount = 0;
            SetScore(0);
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

        private void ResolveSessionManager()
        {
            if (sessionManager == null)
                sessionManager = GetComponent<ApplicationSessionManager>();

            if (sessionManager == null)
                sessionManager = GetComponentInChildren<ApplicationSessionManager>(includeInactive: true);

            if (sessionManager == null)
                sessionManager = FindAnyObjectByType<ApplicationSessionManager>();
        }

        private void HandleSessionGenerated(ApplicationSessionData session)
        {
            scoreContributions.Clear();
            scoreMaximums.Clear();
            questionAnswersBySlot.Clear();
            questionAnswers.Clear();
            hasFinalScore = false;
            answeredQuestionCount = 0;
            SetScore(0);
        }

        private void RebuildQuestionAnswerSnapshot()
        {
            questionAnswers.Clear();

            IReadOnlyList<ApplicationQuestionRuntimeData> questions =
                sessionManager != null
                    ? sessionManager.Questions
                    : Array.Empty<ApplicationQuestionRuntimeData>();

            for (int i = 0; i < questions.Count; i++)
            {
                if (questionAnswersBySlot.TryGetValue(
                    questions[i].SlotId,
                    out ApplicationQuestionAnswerRecord answer))
                {
                    questionAnswers.Add(answer);
                }
            }

            answeredQuestionCount = questionAnswers.Count;
        }

        private void UpdateQuestionCompletion()
        {
            int questionCount = sessionManager != null ? sessionManager.Questions.Count : 0;
            hasFinalScore = questionCount > 0 && questionAnswers.Count == questionCount;
        }

        private void SetContribution(string id, int points, int maximum)
        {
            if (string.IsNullOrWhiteSpace(id))
                return;

            if (maximum <= 0)
            {
                scoreContributions.Remove(id);
                scoreMaximums.Remove(id);
                return;
            }

            scoreContributions[id] = Mathf.Clamp(points, 0, maximum);
            scoreMaximums[id] = maximum;
        }

        private void RecalculateScore()
        {
            //I run every task and question contribution through the same calculation and keep the result from 0 to 10.
            int scoreTotal = 0;
            int maximumTotal = 0;

            foreach (int value in scoreContributions.Values)
                scoreTotal += value;

            foreach (int value in scoreMaximums.Values)
                maximumTotal += value;

            int normalizedScore = maximumTotal > 0
                ? Mathf.RoundToInt(MaximumScore * ((float)scoreTotal / maximumTotal))
                : 0;

            SetScore(normalizedScore);
        }

        private void SetScore(int value)
        {
            applicationScore = Mathf.Clamp(value, 0, MaximumScore);
            ScoreChanged?.Invoke(Score);
            scoreChanged?.Raise(Score);
        }
    }
}
