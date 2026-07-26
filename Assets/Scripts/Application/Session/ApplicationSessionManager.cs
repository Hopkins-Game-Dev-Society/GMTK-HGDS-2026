using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [DisallowMultipleComponent]
    public sealed class ApplicationSessionManager : MonoBehaviour
    {
        private const string QuestionSlotPrefix = "question_";
        private const string MadlibSlotPrefix = "madlib_";
        private const string FixedNameId = "bartholomew_huang";
        private const string FixedUsernameId = "big_boss_username";
        private const string FixedPasswordId = "banana_password";
        private const string FixedTwoFactorCodeId = "birthday_0422";
        private static readonly string[] FixedQuestionIds =
        {
            "ideal_salary",
            "mothers_maiden_name",
            "time_left_seconds",
            "eye_color_outside",
            "visa_sponsorship_needed",
            "fight_horse"
        };
        private static readonly string[] FixedMadlibIds =
        {
            "biggest_strength",
            "overcome_stress",
            "workplace_argument",
            "five_years"
        };

        [Header("Authoring")]
        [SerializeField] private ApplicationSessionCatalog catalog;
        [SerializeField] private ApplicationStateModel applicationState;

        [Header("Deterministic Testing")]
        [SerializeField] private bool useFixedSeed;
        [SerializeField] private int fixedSeed = 2204;

        [Header("Runtime State (Inspector Only)")]
        [SerializeField] private string currentSessionId;
        [SerializeField] private int currentSeed;
        [SerializeField] private string currentNameId;
        [SerializeField] private string currentUsernameId;
        [SerializeField] private string currentPasswordId;
        [SerializeField] private string currentTwoFactorCodeId;
        [SerializeField] private List<string> currentQuestionIds = new();
        [SerializeField] private List<string> currentMadlibIds = new();
        [SerializeField] private List<string> currentClueIds = new();

        public event Action<ApplicationSessionData> SessionGenerated;

        public ApplicationSessionData Current { get; private set; }
        public bool HasSession => Current != null;
        public IReadOnlyList<ApplicationQuestionRuntimeData> Questions =>
            Current != null
                ? Current.Questions
                : Array.Empty<ApplicationQuestionRuntimeData>();
        public IReadOnlyList<ApplicationMadlibRuntimeData> Madlibs =>
            Current != null
                ? Current.Madlibs
                : Array.Empty<ApplicationMadlibRuntimeData>();
        public IReadOnlyList<ApplicationClueRuntimeData> Clues =>
            Current != null
                ? Current.Clues
                : Array.Empty<ApplicationClueRuntimeData>();

        private void Awake()
        {
            ResolveReferences();
        }

        private void Start()
        {
            if (Current == null)
                GenerateNewSession();
        }

        [ContextMenu("Session/Generate New Session")]
        public void GenerateNewSession()
        {
            ResolveReferences();

            if (catalog == null)
            {
                Debug.LogError("ApplicationSessionManager: no session catalog is assigned.", this);
                return;
            }

            List<ApplicationNameDefinition> names = GetValidNames();
            List<ApplicationRandomValueDefinition> usernames =
                GetValidValues(catalog.Usernames);
            List<ApplicationRandomValueDefinition> passwords =
                GetValidValues(catalog.Passwords);
            List<ApplicationRandomValueDefinition> twoFactorCodes =
                GetValidValues(catalog.TwoFactorCodes);
            List<ApplicationQuestionDefinition> questionBank = GetValidQuestions();
            List<ApplicationMadlibDefinition> madlibBank = GetValidMadlibs();
            List<QuestionSlot> questionSlots = GetQuestionSlots();
            List<QuestionSlot> madlibSlots = GetMadlibSlots();

            if (names.Count == 0)
            {
                Debug.LogError("ApplicationSessionManager: the catalog has no valid names.", this);
                return;
            }

            if (usernames.Count == 0
                || passwords.Count == 0
                || twoFactorCodes.Count == 0)
            {
                Debug.LogError(
                    "ApplicationSessionManager: the catalog must contain at least one valid "
                    + "username, password, and two-factor code.",
                    this);
                return;
            }

            if (questionSlots.Count == 0 && madlibSlots.Count == 0)
            {
                Debug.LogError("ApplicationSessionManager: the application flow has no question or madlib slots.", this);
                return;
            }

            int seed = fixedSeed;

            List<ApplicationQuestionDefinition> selectedQuestionDefinitions =
                SelectFixedQuestionsForSlots(questionBank, questionSlots.Count);

            if (selectedQuestionDefinitions.Count < questionSlots.Count)
            {
                Debug.LogError(
                    $"ApplicationSessionManager: the fixed question list could only provide {selectedQuestionDefinitions.Count} questions, "
                    + $"but the active application flow requires {questionSlots.Count}. Check the catalog and fixed question ids.",
                    this);
                return;
            }

            List<ApplicationMadlibDefinition> selectedMadlibDefinitions =
                SelectFixedMadlibsForSlots(madlibBank, madlibSlots.Count);

            if (selectedMadlibDefinitions.Count < madlibSlots.Count)
            {
                Debug.LogError(
                    $"ApplicationSessionManager: the fixed madlib list could only provide {selectedMadlibDefinitions.Count} madlibs, "
                    + $"but the active application flow requires {madlibSlots.Count}. Check the catalog and fixed madlib ids.",
                    this);
                return;
            }

            ApplicationNameDefinition selectedName =
                FindById(names, FixedNameId, name => name.NameId) ?? names[0];
            ApplicationRandomValueDefinition selectedUsername =
                FindById(usernames, FixedUsernameId, value => value.ValueId) ?? usernames[0];
            ApplicationRandomValueDefinition selectedPassword =
                FindById(passwords, FixedPasswordId, value => value.ValueId) ?? passwords[0];
            ApplicationRandomValueDefinition selectedTwoFactorCode =
                FindById(twoFactorCodes, FixedTwoFactorCodeId, value => value.ValueId) ?? twoFactorCodes[0];
            ApplicationApplicantRuntimeData applicant =
                new(
                    selectedName,
                    selectedUsername,
                    selectedPassword,
                    selectedTwoFactorCode);

            List<ApplicationQuestionRuntimeData> selectedQuestions = new(questionSlots.Count);
            for (int i = 0; i < questionSlots.Count; i++)
            {
                QuestionSlot slot = questionSlots[i];
                ApplicationQuestionDefinition selectedQuestion = selectedQuestionDefinitions[i];
                selectedQuestions.Add(new ApplicationQuestionRuntimeData(
                    slot.SlotId,
                    slot.SectionId,
                    slot.Weight,
                    selectedQuestion,
                    ResolvePreferredAnswerId(selectedQuestion)));
            }

            List<ApplicationMadlibRuntimeData> selectedMadlibs = new(madlibSlots.Count);
            for (int i = 0; i < madlibSlots.Count; i++)
            {
                QuestionSlot slot = madlibSlots[i];
                selectedMadlibs.Add(new ApplicationMadlibRuntimeData(
                    slot.SlotId,
                    slot.SectionId,
                    slot.Weight,
                    selectedMadlibDefinitions[i]));
            }

            List<ApplicationClueRuntimeData> selectedClues = ResolveClues(
                applicant,
                selectedQuestions);

            Current = new ApplicationSessionData(
                Guid.NewGuid().ToString("N"),
                seed,
                applicant,
                selectedQuestions,
                selectedMadlibs,
                selectedClues);

            UpdateInspectorState();
            SessionGenerated?.Invoke(Current);
        }

        public ApplicationQuestionRuntimeData FindQuestionBySlot(string slotId)
        {
            return Current?.FindQuestionBySlot(slotId);
        }

        public ApplicationMadlibRuntimeData FindMadlibBySlot(string slotId)
        {
            return Current?.FindMadlibBySlot(slotId);
        }

        private void ResolveReferences()
        {
            if (applicationState == null)
                applicationState = GetComponentInChildren<ApplicationStateModel>(includeInactive: true);

            if (applicationState == null)
                applicationState = FindAnyObjectByType<ApplicationStateModel>();
        }

        private List<ApplicationNameDefinition> GetValidNames()
        {
            List<ApplicationNameDefinition> result = new();
            IReadOnlyList<ApplicationNameDefinition> authored = catalog.Names;

            if (authored == null)
                return result;

            for (int i = 0; i < authored.Count; i++)
            {
                ApplicationNameDefinition name = authored[i];
                if (name != null && name.IsValid)
                    result.Add(name);
            }

            return result;
        }

        private List<ApplicationMadlibDefinition> GetValidMadlibs()
        {
            List<ApplicationMadlibDefinition> result = new();
            IReadOnlyList<ApplicationMadlibDefinition> authored = catalog.Madlibs;

            if (authored == null)
                return result;

            for (int i = 0; i < authored.Count; i++)
            {
                ApplicationMadlibDefinition madlib = authored[i];
                if (madlib != null && madlib.IsValid)
                    result.Add(madlib);
            }

            return result;
        }

        private static List<ApplicationRandomValueDefinition> GetValidValues(
            IReadOnlyList<ApplicationRandomValueDefinition> authored)
        {
            List<ApplicationRandomValueDefinition> result = new();

            if (authored == null)
                return result;

            for (int i = 0; i < authored.Count; i++)
            {
                ApplicationRandomValueDefinition value = authored[i];
                if (value != null && value.IsValid)
                    result.Add(value);
            }

            return result;
        }

        private List<ApplicationQuestionDefinition> GetValidQuestions()
        {
            List<ApplicationQuestionDefinition> result = new();
            IReadOnlyList<ApplicationQuestionDefinition> authored = catalog.Questions;

            if (authored == null)
                return result;

            for (int i = 0; i < authored.Count; i++)
            {
                ApplicationQuestionDefinition question = authored[i];
                if (question != null && question.IsValid)
                    result.Add(question);
            }

            return result;
        }

        private static List<ApplicationQuestionDefinition> SelectFixedQuestionsForSlots(
            List<ApplicationQuestionDefinition> questionBank,
            int slotCount)
        {
            List<ApplicationQuestionDefinition> selected = new(slotCount);
            for (int i = 0; i < FixedQuestionIds.Length && selected.Count < slotCount; i++)
            {
                ApplicationQuestionDefinition question =
                    FindById(questionBank, FixedQuestionIds[i], item => item.QuestionId);

                if (question != null)
                    selected.Add(question);
            }

            return selected;
        }

        private static string ResolvePreferredAnswerId(
            ApplicationQuestionDefinition question)
        {
            if (question == null)
                return string.Empty;

            return question.PreferredAnswerId;
        }

        private static List<ApplicationMadlibDefinition> SelectFixedMadlibsForSlots(
            List<ApplicationMadlibDefinition> madlibBank,
            int slotCount)
        {
            List<ApplicationMadlibDefinition> selected = new(slotCount);
            for (int i = 0; i < FixedMadlibIds.Length && selected.Count < slotCount; i++)
            {
                ApplicationMadlibDefinition madlib =
                    FindById(madlibBank, FixedMadlibIds[i], item => item.MadlibId);

                if (madlib != null)
                    selected.Add(madlib);
            }

            return selected;
        }

        private List<QuestionSlot> GetQuestionSlots()
        {
            List<QuestionSlot> result = new();

            ApplicationFlowDefinition flow = applicationState != null
                ? applicationState.FlowDefinition
                : null;

            if (flow != null && flow.Sections != null)
            {
                AddQuestionSlots(flow.Sections, result);
                return result;
            }

            if (applicationState == null || applicationState.Sections == null)
                return result;

            for (int i = 0; i < applicationState.Sections.Count; i++)
            {
                ApplicationSectionRuntimeState runtimeSection = applicationState.Sections[i];
                if (runtimeSection?.Definition == null)
                    continue;

                AddQuestionSlots(runtimeSection.Definition, result);
            }

            return result;
        }

        private List<QuestionSlot> GetMadlibSlots()
        {
            List<QuestionSlot> result = new();

            ApplicationFlowDefinition flow = applicationState != null
                ? applicationState.FlowDefinition
                : null;

            if (flow != null && flow.Sections != null)
            {
                AddMadlibSlots(flow.Sections, result);
                return result;
            }

            if (applicationState == null || applicationState.Sections == null)
                return result;

            for (int i = 0; i < applicationState.Sections.Count; i++)
            {
                ApplicationSectionRuntimeState runtimeSection = applicationState.Sections[i];
                if (runtimeSection?.Definition == null)
                    continue;

                AddMadlibSlots(runtimeSection.Definition, result);
            }

            return result;
        }

        private static void AddQuestionSlots(
            IReadOnlyList<ApplicationSectionDefinition> sections,
            List<QuestionSlot> destination)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                ApplicationSectionDefinition section = sections[i];
                if (section == null)
                    continue;

                AddQuestionSlots(section, destination);
            }
        }

        private static void AddQuestionSlots(
            ApplicationSectionDefinition section,
            List<QuestionSlot> destination)
        {
            if (section.SectionId != ApplicationSectionId.ApplicationQuestionsOne
                && section.SectionId != ApplicationSectionId.ApplicationQuestionsTwo)
            {
                return;
            }

            IReadOnlyList<ApplicationChallengeDefinition> challenges = section.Challenges;
            for (int i = 0; i < challenges.Count; i++)
            {
                ApplicationChallengeDefinition challenge = challenges[i];
                if (challenge == null
                    || string.IsNullOrWhiteSpace(challenge.ChallengeId)
                    || !challenge.ChallengeId.StartsWith(
                        QuestionSlotPrefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                destination.Add(new QuestionSlot(
                    challenge.ChallengeId,
                    section.SectionId,
                    challenge.MaxPoints));
            }
        }

        private static void AddMadlibSlots(
            IReadOnlyList<ApplicationSectionDefinition> sections,
            List<QuestionSlot> destination)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                ApplicationSectionDefinition section = sections[i];
                if (section == null)
                    continue;

                AddMadlibSlots(section, destination);
            }
        }

        private static void AddMadlibSlots(
            ApplicationSectionDefinition section,
            List<QuestionSlot> destination)
        {
            if (section.SectionId != ApplicationSectionId.ApplicationQuestionsTwo)
                return;

            IReadOnlyList<ApplicationChallengeDefinition> challenges = section.Challenges;
            for (int i = 0; i < challenges.Count; i++)
            {
                ApplicationChallengeDefinition challenge = challenges[i];
                if (challenge == null
                    || string.IsNullOrWhiteSpace(challenge.ChallengeId)
                    || !challenge.ChallengeId.StartsWith(
                        MadlibSlotPrefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                destination.Add(new QuestionSlot(
                    challenge.ChallengeId,
                    section.SectionId,
                    challenge.MaxPoints));
            }
        }

        private List<ApplicationClueRuntimeData> ResolveClues(
            ApplicationApplicantRuntimeData applicant,
            IReadOnlyList<ApplicationQuestionRuntimeData> questions)
        {
            List<ApplicationClueRuntimeData> result = new();

            AddMatchingClue(result, "first_name", applicant.FirstName);
            AddMatchingClue(result, "last_name", applicant.LastName);
            AddMatchingClue(result, "username", applicant.Username);
            AddMatchingClue(result, "password", applicant.Password);

            for (int i = 0; i < questions.Count; i++)
            {
                ApplicationQuestionRuntimeData question = questions[i];
                if (string.IsNullOrWhiteSpace(question.PreferredAnswerId))
                    continue;

                AddMatchingClue(
                    result,
                    question.QuestionId,
                    question.PreferredAnswerId,
                    question.SlotId);
            }

            return result;
        }

        private void AddMatchingClue(
            List<ApplicationClueRuntimeData> destination,
            string targetId,
            string associatedAnswer,
            string targetSlotId = null)
        {
            IReadOnlyList<ApplicationClueDefinition> authored = catalog.Clues;
            if (authored == null)
                return;

            List<ApplicationClueDefinition> candidates = new();
            for (int i = 0; i < authored.Count; i++)
            {
                ApplicationClueDefinition clue = authored[i];
                if (clue != null && clue.Matches(targetId, associatedAnswer))
                    candidates.Add(clue);
            }

            if (candidates.Count == 0)
                return;

            destination.Add(new ApplicationClueRuntimeData(candidates[0], targetSlotId));
        }

        private void UpdateInspectorState()
        {
            currentQuestionIds ??= new List<string>();
            currentMadlibIds ??= new List<string>();
            currentClueIds ??= new List<string>();
            currentSessionId = Current != null ? Current.SessionId : string.Empty;
            currentSeed = Current != null ? Current.Seed : 0;
            currentNameId = Current?.Applicant?.NameId ?? string.Empty;
            currentUsernameId = Current?.Applicant?.UsernameId ?? string.Empty;
            currentPasswordId = Current?.Applicant?.PasswordId ?? string.Empty;
            currentTwoFactorCodeId =
                Current?.Applicant?.TwoFactorCodeId ?? string.Empty;

            currentQuestionIds.Clear();
            currentMadlibIds.Clear();
            currentClueIds.Clear();

            if (Current == null)
                return;

            for (int i = 0; i < Current.Questions.Count; i++)
            {
                ApplicationQuestionRuntimeData question = Current.Questions[i];
                currentQuestionIds.Add($"{question.SlotId}: {question.QuestionId}");
            }

            for (int i = 0; i < Current.Madlibs.Count; i++)
            {
                ApplicationMadlibRuntimeData madlib = Current.Madlibs[i];
                currentMadlibIds.Add($"{madlib.SlotId}: {madlib.MadlibId}");
            }

            for (int i = 0; i < Current.Clues.Count; i++)
                currentClueIds.Add(Current.Clues[i].ClueId);
        }

        private static T FindById<T>(
            IReadOnlyList<T> values,
            string id,
            Func<T, string> getId)
        {
            if (values == null || string.IsNullOrWhiteSpace(id) || getId == null)
                return default;

            for (int i = 0; i < values.Count; i++)
            {
                T value = values[i];
                if (value != null && string.Equals(getId(value), id, StringComparison.OrdinalIgnoreCase))
                    return value;
            }

            return default;
        }

        private readonly struct QuestionSlot
        {
            public QuestionSlot(
                string slotId,
                ApplicationSectionId sectionId,
                int weight)
            {
                SlotId = slotId;
                SectionId = sectionId;
                Weight = weight;
            }

            public string SlotId { get; }
            public ApplicationSectionId SectionId { get; }
            public int Weight { get; }
        }
    }
}
