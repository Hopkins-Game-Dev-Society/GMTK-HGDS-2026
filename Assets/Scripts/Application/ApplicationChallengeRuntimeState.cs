using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationChallengeRuntimeState
    {
        public ApplicationChallengeRuntimeState(
            string challengeId,
            string displayName,
            bool required,
            int maxPoints)
        {
            ChallengeId = challengeId;
            DisplayName = displayName;
            Required = required;
            MaxPoints = Mathf.Max(0, maxPoints);
        }

        public string ChallengeId { get; }
        public string DisplayName { get; }
        public bool Required { get; }
        public int MaxPoints { get; }
        public ApplicationChallengeStatus Status { get; private set; }
        public bool IsComplete => Status == ApplicationChallengeStatus.Complete;
        public int PointsAwarded { get; private set; }
        public int WrongAttempts { get; private set; }
        public string LastErrorMessage { get; private set; }

        internal void MarkComplete(int pointsAwarded)
        {
            Status = ApplicationChallengeStatus.Complete;
            PointsAwarded = Mathf.Clamp(pointsAwarded, 0, MaxPoints);
            LastErrorMessage = string.Empty;
        }

        internal void RecordWrongAnswer(string errorMessage)
        {
            WrongAttempts++;
            LastErrorMessage = errorMessage;
        }

        internal void ResetProgress()
        {
            Status = ApplicationChallengeStatus.Incomplete;
            PointsAwarded = 0;
            LastErrorMessage = string.Empty;
        }

        internal void ResetAll()
        {
            ResetProgress();
            WrongAttempts = 0;
        }
    }
}
