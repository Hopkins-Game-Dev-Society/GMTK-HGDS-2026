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
        [SerializeField, Min(0f)] private float refreshCooldownSeconds = 5f;
        [SerializeField] private bool resetChallengesOnRefresh = true;
        [SerializeField] private List<ApplicationChallengeDefinition> challenges = new List<ApplicationChallengeDefinition>();

        public ApplicationSectionDefinition()
        {
        }

        internal ApplicationSectionDefinition(
            ApplicationSectionId sectionId,
            string displayName,
            float refreshCooldownSeconds,
            bool resetChallengesOnRefresh,
            List<ApplicationChallengeDefinition> challenges)
        {
            this.sectionId = sectionId;
            this.displayName = displayName;
            this.refreshCooldownSeconds = refreshCooldownSeconds;
            this.resetChallengesOnRefresh = resetChallengesOnRefresh;
            this.challenges = challenges ?? new List<ApplicationChallengeDefinition>();
        }

        public ApplicationSectionId SectionId => sectionId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? sectionId.ToString() : displayName;
        public float RefreshCooldownSeconds => refreshCooldownSeconds;
        public bool ResetChallengesOnRefresh => resetChallengesOnRefresh;
        public IReadOnlyList<ApplicationChallengeDefinition> Challenges => challenges;
    }
}
