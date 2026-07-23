using System;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [Serializable]
    public sealed class ApplicationChallengeDefinition
    {
        [SerializeField] private string challengeId;
        [SerializeField] private string displayName;
        [SerializeField] private bool required = true;
        [SerializeField, Range(0, 5)] private int maxPoints = 1;

        public ApplicationChallengeDefinition()
        {
        }

        internal ApplicationChallengeDefinition(
            string challengeId,
            string displayName,
            bool required,
            int maxPoints)
        {
            this.challengeId = challengeId;
            this.displayName = displayName;
            this.required = required;
            this.maxPoints = maxPoints;
        }

        public string ChallengeId => challengeId;
        public string DisplayName => displayName;
        public bool Required => required;
        public int MaxPoints => maxPoints;

        internal string GetResolvedId(int fallbackIndex)
        {
            if (!string.IsNullOrWhiteSpace(challengeId))
                return challengeId;

            if (!string.IsNullOrWhiteSpace(displayName))
                return MakeId(displayName);

            return $"challenge_{fallbackIndex + 1}";
        }

        internal string GetResolvedDisplayName(int fallbackIndex)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
                return displayName;

            return GetResolvedId(fallbackIndex);
        }

        private static string MakeId(string text)
        {
            return text.Trim()
                .ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("-", "_");
        }
    }
}
