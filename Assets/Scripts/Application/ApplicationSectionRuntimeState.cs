using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationSectionRuntimeState
    {
        private readonly List<ApplicationChallengeRuntimeState> challenges = new List<ApplicationChallengeRuntimeState>();

        public ApplicationSectionRuntimeState(ApplicationSectionDefinition definition)
        {
            Definition = definition;
            SectionId = definition.SectionId;
            DisplayName = definition.DisplayName;
            ProgressLabel = definition.ProgressLabel;
            RefreshCooldownSeconds = definition.RefreshCooldownSeconds;
            ResetChallengesOnRefresh = definition.ResetChallengesOnRefresh;

            IReadOnlyList<ApplicationChallengeDefinition> challengeDefinitions = definition.Challenges;
            for (int i = 0; i < challengeDefinitions.Count; i++)
            {
                ApplicationChallengeDefinition challenge = challengeDefinitions[i];
                if (challenge == null)
                    continue;

                challenges.Add(new ApplicationChallengeRuntimeState(
                    challenge.GetResolvedId(i),
                    challenge.GetResolvedDisplayName(i),
                    challenge.Required,
                    challenge.MaxPoints));
            }
        }

        public ApplicationSectionDefinition Definition { get; }
        public ApplicationSectionId SectionId { get; }
        public string DisplayName { get; }
        public string ProgressLabel { get; }
        public float RefreshCooldownSeconds { get; }
        public bool ResetChallengesOnRefresh { get; }
        public IReadOnlyList<ApplicationChallengeRuntimeState> Challenges => challenges;
        public bool IsBlocked { get; private set; }
        public bool RequiresReauthenticationBeforeRefresh { get; private set; }
        public string ErrorMessage { get; private set; }
        public int WrongAttempts { get; private set; }
        public int RefreshCount { get; private set; }
        public float RefreshAvailableAtTime { get; private set; }

        public int RequiredChallengeCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < challenges.Count; i++)
                {
                    if (challenges[i].Required)
                        count++;
                }

                return count;
            }
        }

        public int CompletedRequiredChallengeCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < challenges.Count; i++)
                {
                    if (challenges[i].Required && challenges[i].IsComplete)
                        count++;
                }

                return count;
            }
        }

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < challenges.Count; i++)
                {
                    if (challenges[i].Required && !challenges[i].IsComplete)
                        return false;
                }

                return true;
            }
        }

        public int PointsAwarded
        {
            get
            {
                int total = 0;
                for (int i = 0; i < challenges.Count; i++)
                    total += challenges[i].PointsAwarded;

                return total;
            }
        }

        public ApplicationChallengeRuntimeState FindChallenge(string challengeId)
        {
            if (string.IsNullOrWhiteSpace(challengeId))
                return null;

            for (int i = 0; i < challenges.Count; i++)
            {
                if (string.Equals(challenges[i].ChallengeId, challengeId, System.StringComparison.OrdinalIgnoreCase))
                    return challenges[i];
            }

            return null;
        }

        internal void Block(string errorMessage, float refreshAvailableAtTime, bool requiresReauthenticationBeforeRefresh = false)
        {
            IsBlocked = true;
            RequiresReauthenticationBeforeRefresh = requiresReauthenticationBeforeRefresh;
            ErrorMessage = errorMessage;
            RefreshAvailableAtTime = Mathf.Max(Time.time, refreshAvailableAtTime);
        }

        internal void ClearBlock()
        {
            IsBlocked = false;
            RequiresReauthenticationBeforeRefresh = false;
            ErrorMessage = string.Empty;
            RefreshAvailableAtTime = 0f;
        }

        internal void CompleteReauthentication(float refreshAvailableAtTime)
        {
            RequiresReauthenticationBeforeRefresh = false;
            RefreshAvailableAtTime = Mathf.Max(Time.time, refreshAvailableAtTime);
        }

        internal void RecordWrongAnswer()
        {
            WrongAttempts++;
        }

        internal void RecordRefresh()
        {
            RefreshCount++;
        }

        internal void ResetChallengeProgress()
        {
            for (int i = 0; i < challenges.Count; i++)
                challenges[i].ResetProgress();
        }

        internal void ResetAll()
        {
            ClearBlock();
            WrongAttempts = 0;
            RefreshCount = 0;

            for (int i = 0; i < challenges.Count; i++)
                challenges[i].ResetAll();
        }
    }
}
