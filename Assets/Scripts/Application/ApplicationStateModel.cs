using System;
using System.Collections.Generic;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationStateModel : MonoBehaviour
    {
        [Header("Application Structure")]
        [SerializeField] private ApplicationFlowDefinition flowDefinition;
        [SerializeField] private List<ApplicationSectionDefinition> sections = new List<ApplicationSectionDefinition>();
        [SerializeField] private ApplicationSectionId initialSection = ApplicationSectionId.CreateAccountSignIn;

        [Header("Default Error Copy")]
        [SerializeField] private string defaultErrorMessage = "An error occurred. Please refresh the page to continue.";
        [SerializeField, Min(0f)] private float fallbackRefreshCooldownSeconds = 5f;

        [Header("Events")]
        [SerializeField] private GameEvent stateChangedEvent;
        [SerializeField] private GameEvent sectionChangedEvent;
        [SerializeField] private GameEvent sectionCompletedEvent;
        [SerializeField] private GameEvent pageBlockedEvent;
        [SerializeField] private GameEvent pageRefreshedEvent;
        [SerializeField] private StringGameEvent currentSectionChangedEvent;
        [SerializeField] private StringGameEvent errorMessageChangedEvent;
        [SerializeField] private FloatGameEvent refreshCooldownChangedEvent;

        private readonly List<ApplicationSectionRuntimeState> runtimeSections = new List<ApplicationSectionRuntimeState>();
        private int currentSectionIndex = -1;
        private float lastReportedRefreshCooldown = -1f;

        public event Action StateChanged;
        public event Action<ApplicationSectionRuntimeState> SectionChanged;
        public event Action<ApplicationSectionRuntimeState> SectionCompleted;
        public event Action<ApplicationSectionRuntimeState> PageBlocked;
        public event Action<ApplicationSectionRuntimeState> PageRefreshed;
        public event Action<ApplicationChallengeRuntimeState> ChallengeChanged;

        public IReadOnlyList<ApplicationSectionRuntimeState> Sections => runtimeSections;
        public ApplicationSectionRuntimeState CurrentSection =>
            currentSectionIndex >= 0 && currentSectionIndex < runtimeSections.Count
                ? runtimeSections[currentSectionIndex]
                : null;

        public int CurrentSectionIndex => currentSectionIndex;
        public bool HasInitialized { get; private set; }
        public bool IsOnFinalSection => currentSectionIndex >= runtimeSections.Count - 1;
        public bool CanAdvanceCurrentSection => CurrentSection != null && !CurrentSection.IsBlocked && CurrentSection.IsComplete && !IsOnFinalSection;
        public bool CanRefreshCurrentSection => CurrentSection != null && CurrentSection.IsBlocked && RefreshCooldownRemaining <= 0f;
        public ApplicationFlowDefinition FlowDefinition => flowDefinition;

        public float RefreshCooldownRemaining
        {
            get
            {
                ApplicationSectionRuntimeState section = CurrentSection;
                if (section == null || !section.IsBlocked)
                    return 0f;

                return Mathf.Max(0f, section.RefreshAvailableAtTime - Time.time);
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            float remaining = RefreshCooldownRemaining;
            if (!Mathf.Approximately(remaining, lastReportedRefreshCooldown))
            {
                lastReportedRefreshCooldown = remaining;
                refreshCooldownChangedEvent?.Raise(remaining);
            }
        }

        private void Reset()
        {
            UseDefaultJamSections();
        }

        public void Initialize()
        {
            runtimeSections.Clear();

            IReadOnlyList<ApplicationSectionDefinition> activeSections = GetActiveSections();

            if (activeSections.Count == 0)
            {
                UseDefaultJamSections();
                activeSections = sections;
            }

            for (int i = 0; i < activeSections.Count; i++)
            {
                ApplicationSectionDefinition section = activeSections[i];
                if (section == null || section.SectionId == ApplicationSectionId.None)
                    continue;

                runtimeSections.Add(new ApplicationSectionRuntimeState(section));
            }

            currentSectionIndex = Mathf.Max(0, FindSectionIndex(GetInitialSection()));
            if (runtimeSections.Count == 0)
                currentSectionIndex = -1;

            HasInitialized = true;
            RaiseSectionChanged();
            RaiseStateChanged();
        }

        public bool TryAdvanceSection()
        {
            if (!CanAdvanceCurrentSection)
                return false;

            currentSectionIndex++;
            RaiseSectionChanged();
            RaiseStateChanged();
            return true;
        }

        public bool TryGoToSection(ApplicationSectionId sectionId)
        {
            int index = FindSectionIndex(sectionId);
            if (index < 0)
                return false;

            currentSectionIndex = index;
            RaiseSectionChanged();
            RaiseStateChanged();
            return true;
        }

        public bool MarkChallengeComplete(string challengeId)
        {
            return MarkChallengeComplete(challengeId, pointsAwarded: -1);
        }

        public bool MarkChallengeComplete(string challengeId, int pointsAwarded)
        {
            ApplicationSectionRuntimeState section = CurrentSection;
            if (section == null || section.IsBlocked)
                return false;

            ApplicationChallengeRuntimeState challenge = section.FindChallenge(challengeId);
            if (challenge == null)
            {
                Debug.LogWarning($"ApplicationStateModel: no challenge '{challengeId}' exists on section '{section.DisplayName}'.", this);
                return false;
            }

            int points = pointsAwarded < 0 ? challenge.MaxPoints : pointsAwarded;
            bool wasSectionComplete = section.IsComplete;

            challenge.MarkComplete(points);
            ChallengeChanged?.Invoke(challenge);

            if (!wasSectionComplete && section.IsComplete)
            {
                SectionCompleted?.Invoke(section);
                sectionCompletedEvent?.Raise();
            }

            RaiseStateChanged();
            return true;
        }

        public bool ReportWrongAnswer(string challengeId)
        {
            return ReportWrongAnswer(challengeId, defaultErrorMessage, ApplicationWrongAnswerConsequence.RequireRefresh, refreshCooldownOverrideSeconds: -1f);
        }

        public bool ReportWrongAnswer(
            string challengeId,
            string errorMessage,
            ApplicationWrongAnswerConsequence consequence,
            float refreshCooldownOverrideSeconds = -1f)
        {
            ApplicationSectionRuntimeState section = CurrentSection;
            if (section == null || section.IsBlocked)
                return false;

            string resolvedErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
                ? GetDefaultErrorMessage()
                : errorMessage;

            ApplicationChallengeRuntimeState challenge = section.FindChallenge(challengeId);
            if (challenge != null)
            {
                challenge.RecordWrongAnswer(resolvedErrorMessage);
                ChallengeChanged?.Invoke(challenge);
            }

            section.RecordWrongAnswer();

            switch (consequence)
            {
                case ApplicationWrongAnswerConsequence.ResetChallenge:
                    challenge?.ResetProgress();
                    break;

                case ApplicationWrongAnswerConsequence.ResetCurrentSection:
                    section.ResetChallengeProgress();
                    break;

                case ApplicationWrongAnswerConsequence.RequireRefresh:
                    float cooldown = refreshCooldownOverrideSeconds >= 0f
                        ? refreshCooldownOverrideSeconds
                        : Mathf.Max(GetFallbackRefreshCooldownSeconds(), section.RefreshCooldownSeconds);

                    section.Block(resolvedErrorMessage, Time.time + cooldown);
                    PageBlocked?.Invoke(section);
                    pageBlockedEvent?.Raise();
                    errorMessageChangedEvent?.Raise(resolvedErrorMessage);
                    break;
            }

            RaiseStateChanged();
            return true;
        }

        public bool TryRefreshCurrentSection()
        {
            ApplicationSectionRuntimeState section = CurrentSection;
            if (section == null || !section.IsBlocked || RefreshCooldownRemaining > 0f)
                return false;

            if (section.ResetChallengesOnRefresh)
                section.ResetChallengeProgress();

            section.ClearBlock();
            section.RecordRefresh();
            lastReportedRefreshCooldown = -1f;

            PageRefreshed?.Invoke(section);
            pageRefreshedEvent?.Raise();
            errorMessageChangedEvent?.Raise(string.Empty);
            RaiseStateChanged();
            return true;
        }

        public void ResetApplication()
        {
            for (int i = 0; i < runtimeSections.Count; i++)
                runtimeSections[i].ResetAll();

            currentSectionIndex = Mathf.Max(0, FindSectionIndex(GetInitialSection()));
            lastReportedRefreshCooldown = -1f;

            RaiseSectionChanged();
            RaiseStateChanged();
        }

        [ContextMenu("Use Default Jam Sections")]
        public void UseDefaultJamSections()
        {
            flowDefinition = null;
            sections = DefaultApplicationSections.Create();
            initialSection = ApplicationSectionId.CreateAccountSignIn;
        }

        public void SetFlowDefinition(ApplicationFlowDefinition definition, bool reinitialize = true)
        {
            flowDefinition = definition;

            if (reinitialize)
                Initialize();
        }

        private IReadOnlyList<ApplicationSectionDefinition> GetActiveSections()
        {
            if (flowDefinition != null && flowDefinition.Sections != null && flowDefinition.Sections.Count > 0)
                return flowDefinition.Sections;

            return sections;
        }

        private ApplicationSectionId GetInitialSection()
        {
            if (flowDefinition != null && flowDefinition.InitialSection != ApplicationSectionId.None)
                return flowDefinition.InitialSection;

            return initialSection;
        }

        private string GetDefaultErrorMessage()
        {
            if (flowDefinition != null && !string.IsNullOrWhiteSpace(flowDefinition.DefaultErrorMessage))
                return flowDefinition.DefaultErrorMessage;

            return defaultErrorMessage;
        }

        private float GetFallbackRefreshCooldownSeconds()
        {
            if (flowDefinition != null)
                return flowDefinition.FallbackRefreshCooldownSeconds;

            return fallbackRefreshCooldownSeconds;
        }

        private int FindSectionIndex(ApplicationSectionId sectionId)
        {
            for (int i = 0; i < runtimeSections.Count; i++)
            {
                if (runtimeSections[i].SectionId == sectionId)
                    return i;
            }

            IReadOnlyList<ApplicationSectionDefinition> activeSections = GetActiveSections();
            for (int i = 0; i < activeSections.Count; i++)
            {
                if (activeSections[i] != null && activeSections[i].SectionId == sectionId)
                    return i;
            }

            return -1;
        }

        private void RaiseSectionChanged()
        {
            ApplicationSectionRuntimeState section = CurrentSection;
            SectionChanged?.Invoke(section);
            sectionChangedEvent?.Raise();
            currentSectionChangedEvent?.Raise(section != null ? section.SectionId.ToString() : string.Empty);
        }

        private void RaiseStateChanged()
        {
            StateChanged?.Invoke();
            stateChangedEvent?.Raise();
        }
    }
}
