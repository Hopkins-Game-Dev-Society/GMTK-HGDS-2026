using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //So here the plan is to build one randomized run and keep it the same when the player changes application views.
    [DisallowMultipleComponent]
    public sealed class ApplicationSessionManager : MonoBehaviour
    {
        private const string QuestionSlotPrefix = "question_";

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
        [SerializeField] private List<string> currentClueIds = new();

        public event Action<ApplicationSessionData> SessionGenerated;

        public ApplicationSessionData Current { get; private set; }
        public bool HasSession => Current != null;
        public IReadOnlyList<ApplicationQuestionRuntimeData> Questions =>
            Current != null
                ? Current.Questions
                : Array.Empty<ApplicationQuestionRuntimeData>();
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
            List<QuestionSlot> slots = GetQuestionSlots();

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

            if (slots.Count == 0)
            {
                Debug.LogError("ApplicationSessionManager: the application flow has no question slots.", this);
                return;
            }

            if (questionBank.Count < slots.Count)
            {
                Debug.LogError(
                    $"ApplicationSessionManager: the question bank has {questionBank.Count} valid questions, "
                    + $"but the active application flow requires {slots.Count}.",
                    this);
                return;
            }

            int seed = useFixedSeed ? fixedSeed : CreateSeed();
            System.Random random = new(seed);

            //I pick one value from each randomization pool for this run.
            ApplicationNameDefinition selectedName =
                names[random.Next(names.Count)];
            ApplicationRandomValueDefinition selectedUsername =
                usernames[random.Next(usernames.Count)];
            ApplicationRandomValueDefinition selectedPassword =
                passwords[random.Next(passwords.Count)];
            ApplicationRandomValueDefinition selectedTwoFactorCode =
                twoFactorCodes[random.Next(twoFactorCodes.Count)];
            ApplicationApplicantRuntimeData applicant =
                new(
                    selectedName,
                    selectedUsername,
                    selectedPassword,
                    selectedTwoFactorCode);

            Shuffle(questionBank, random);

            List<ApplicationQuestionRuntimeData> selectedQuestions = new(slots.Count);
            for (int i = 0; i < slots.Count; i++)
            {
                QuestionSlot slot = slots[i];
                selectedQuestions.Add(new ApplicationQuestionRuntimeData(
                    slot.SlotId,
                    slot.SectionId,
                    slot.Weight,
                    questionBank[i]));
            }

            List<ApplicationClueRuntimeData> selectedClues = ResolveClues(
                applicant,
                selectedQuestions,
                random);

            Current = new ApplicationSessionData(
                Guid.NewGuid().ToString("N"),
                seed,
                applicant,
                selectedQuestions,
                selectedClues);

            UpdateInspectorState();
            SessionGenerated?.Invoke(Current);
        }

        public ApplicationQuestionRuntimeData FindQuestionBySlot(string slotId)
        {
            return Current?.FindQuestionBySlot(slotId);
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

        private List<ApplicationClueRuntimeData> ResolveClues(
            ApplicationApplicantRuntimeData applicant,
            IReadOnlyList<ApplicationQuestionRuntimeData> questions,
            System.Random random)
        {
            //I match clues using the target and selected value so the randomized result still points to the right clue.
            List<ApplicationClueRuntimeData> result = new();

            AddRandomMatchingClue(result, random, "first_name", applicant.FirstName);
            AddRandomMatchingClue(result, random, "last_name", applicant.LastName);
            AddRandomMatchingClue(result, random, "username", applicant.Username);
            AddRandomMatchingClue(result, random, "password", applicant.Password);

            for (int i = 0; i < questions.Count; i++)
            {
                ApplicationQuestionRuntimeData question = questions[i];
                if (string.IsNullOrWhiteSpace(question.PreferredAnswerId))
                    continue;

                AddRandomMatchingClue(
                    result,
                    random,
                    question.QuestionId,
                    question.PreferredAnswerId,
                    question.SlotId);
            }

            return result;
        }

        private void AddRandomMatchingClue(
            List<ApplicationClueRuntimeData> destination,
            System.Random random,
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

            destination.Add(new ApplicationClueRuntimeData(
                candidates[random.Next(candidates.Count)],
                targetSlotId));
        }

        private static int CreateSeed()
        {
            return Guid.NewGuid().GetHashCode() ^ Environment.TickCount;
        }

        private void UpdateInspectorState()
        {
            currentQuestionIds ??= new List<string>();
            currentClueIds ??= new List<string>();
            currentSessionId = Current != null ? Current.SessionId : string.Empty;
            currentSeed = Current != null ? Current.Seed : 0;
            currentNameId = Current?.Applicant?.NameId ?? string.Empty;
            currentUsernameId = Current?.Applicant?.UsernameId ?? string.Empty;
            currentPasswordId = Current?.Applicant?.PasswordId ?? string.Empty;
            currentTwoFactorCodeId =
                Current?.Applicant?.TwoFactorCodeId ?? string.Empty;

            currentQuestionIds.Clear();
            currentClueIds.Clear();

            if (Current == null)
                return;

            for (int i = 0; i < Current.Questions.Count; i++)
            {
                ApplicationQuestionRuntimeData question = Current.Questions[i];
                currentQuestionIds.Add($"{question.SlotId}: {question.QuestionId}");
            }

            for (int i = 0; i < Current.Clues.Count; i++)
                currentClueIds.Add(Current.Clues[i].ClueId);
        }

        private static void Shuffle<T>(IList<T> values, System.Random random)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
            }
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
