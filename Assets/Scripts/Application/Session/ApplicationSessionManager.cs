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
        private const string MadlibSlotPrefix = "madlib_";

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

            int seed = useFixedSeed ? fixedSeed : CreateSeed();
            System.Random random = new(seed);

            List<ApplicationQuestionDefinition> selectedQuestionDefinitions =
                SelectQuestionsForSlots(questionBank, questionSlots.Count, random);

            if (selectedQuestionDefinitions.Count < questionSlots.Count)
            {
                Debug.LogError(
                    $"ApplicationSessionManager: the question bank could only provide {selectedQuestionDefinitions.Count} compatible questions, "
                    + $"but the active application flow requires {questionSlots.Count}. Check incompatible question settings.",
                    this);
                return;
            }

            List<ApplicationMadlibDefinition> selectedMadlibDefinitions =
                SelectMadlibsForSlots(madlibBank, madlibSlots.Count, random);

            if (selectedMadlibDefinitions.Count < madlibSlots.Count)
            {
                Debug.LogError(
                    $"ApplicationSessionManager: the madlib bank has {selectedMadlibDefinitions.Count} valid madlibs, "
                    + $"but the active application flow requires {madlibSlots.Count}.",
                    this);
                return;
            }

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
                    ResolvePreferredAnswerId(selectedQuestion, random),
                    random));
            }

            List<ApplicationMadlibRuntimeData> selectedMadlibs = new(madlibSlots.Count);
            for (int i = 0; i < madlibSlots.Count; i++)
            {
                QuestionSlot slot = madlibSlots[i];
                selectedMadlibs.Add(new ApplicationMadlibRuntimeData(
                    slot.SlotId,
                    slot.SectionId,
                    slot.Weight,
                    selectedMadlibDefinitions[i],
                    random));
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

        private static List<ApplicationQuestionDefinition> SelectQuestionsForSlots(
            List<ApplicationQuestionDefinition> questionBank,
            int slotCount,
            System.Random random)
        {
            List<ApplicationQuestionDefinition> shuffled = new(questionBank);
            List<ApplicationQuestionDefinition> best = new(slotCount);

            for (int attempt = 0; attempt < 24; attempt++)
            {
                Shuffle(shuffled, random);
                List<ApplicationQuestionDefinition> selected = new(slotCount);

                for (int i = 0; i < shuffled.Count && selected.Count < slotCount; i++)
                {
                    ApplicationQuestionDefinition candidate = shuffled[i];
                    if (!IsCompatibleWithSelected(candidate, selected))
                        continue;

                    selected.Add(candidate);
                }

                if (selected.Count > best.Count)
                    best = selected;

                if (selected.Count == slotCount)
                    return selected;
            }

            return best;
        }

        private static bool IsCompatibleWithSelected(
            ApplicationQuestionDefinition candidate,
            IReadOnlyList<ApplicationQuestionDefinition> selected)
        {
            for (int i = 0; i < selected.Count; i++)
            {
                if (!candidate.IsCompatibleWith(selected[i]))
                    return false;
            }

            return true;
        }

        private static string ResolvePreferredAnswerId(
            ApplicationQuestionDefinition question,
            System.Random random)
        {
            if (question == null)
                return string.Empty;

            if (!question.RandomizePreferredAnswer)
                return question.PreferredAnswerId;

            IReadOnlyList<ApplicationQuestionAnswerDefinition> answers = question.PossibleAnswers;
            if (answers == null || answers.Count == 0)
                return question.PreferredAnswerId;

            List<ApplicationQuestionAnswerDefinition> validAnswers = new();
            for (int i = 0; i < answers.Count; i++)
            {
                ApplicationQuestionAnswerDefinition answer = answers[i];
                if (answer != null && !string.IsNullOrWhiteSpace(answer.AnswerId))
                    validAnswers.Add(answer);
            }

            return validAnswers.Count > 0
                ? validAnswers[random.Next(validAnswers.Count)].AnswerId
                : question.PreferredAnswerId;
        }

        private static List<ApplicationMadlibDefinition> SelectMadlibsForSlots(
            List<ApplicationMadlibDefinition> madlibBank,
            int slotCount,
            System.Random random)
        {
            List<ApplicationMadlibDefinition> shuffled = new(madlibBank);
            Shuffle(shuffled, random);

            if (shuffled.Count > slotCount)
                shuffled.RemoveRange(slotCount, shuffled.Count - slotCount);

            return shuffled;
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
