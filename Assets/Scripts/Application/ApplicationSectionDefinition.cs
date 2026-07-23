using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [Serializable]
    public sealed class ApplicationSectionDefinition
    {
        [SerializeField] private ApplicationSectionId sectionId;
        [SerializeField] private string displayName;
        [SerializeField] private string progressLabel;
        [SerializeField, Min(0f)] private float refreshCooldownSeconds = 5f;
        [SerializeField] private bool resetChallengesOnRefresh = true;
        [SerializeField] private List<ApplicationChallengeDefinition> challenges = new List<ApplicationChallengeDefinition>();

        public ApplicationSectionDefinition()
        {
        }

        internal ApplicationSectionDefinition(
            ApplicationSectionId sectionId,
            string displayName,
            string progressLabel,
            float refreshCooldownSeconds,
            bool resetChallengesOnRefresh,
            List<ApplicationChallengeDefinition> challenges)
        {
            this.sectionId = sectionId;
            this.displayName = displayName;
            this.progressLabel = progressLabel;
            this.refreshCooldownSeconds = refreshCooldownSeconds;
            this.resetChallengesOnRefresh = resetChallengesOnRefresh;
            this.challenges = challenges ?? new List<ApplicationChallengeDefinition>();
        }

        public ApplicationSectionId SectionId => sectionId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? sectionId.ToString() : displayName;
        public string ProgressLabel => string.IsNullOrWhiteSpace(progressLabel) ? DisplayName : progressLabel;
        public float RefreshCooldownSeconds => refreshCooldownSeconds;
        public bool ResetChallengesOnRefresh => resetChallengesOnRefresh;
        public IReadOnlyList<ApplicationChallengeDefinition> Challenges => challenges;
    }
}
