using System.Collections;
using System.Collections.Generic;
using BirthdayJobJam.Core;
using BirthdayJobJam.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationSignInPageView : MonoBehaviour
    {
        private const string DefaultUsernameChallengeId = "username";
        private const string DefaultPasswordChallengeId = "password";
        private const string DefaultTwoFactorChallengeId = "two_factor_code";
        private const string DefaultCorrectUsername = "big.boss@outerheaven.com";
        private const string DefaultCorrectPassword = "banana_protocol";
        private const string DefaultCorrectTwoFactorCode = "0422";
        private const string TimeLeftSecondsQuestionId = "time_left_seconds";
        private const string CorrectTimeLeftAnswerId = "birthday_timer_seconds";
        private static readonly string[] DefaultResumeFileNames =
        {
            "resume-new.doc",
            "resume-final.doc",
            "resume-final-final.doc",
            "resume-true-final.doc",
            "resume-1.doc",
            "resume-true-final-FINAL.doc",
            "resume-2.doc",
            "resume-use-this-one-maybe.doc",
            "resume-DONT-use.doc",
            "resume-real-actual-last.doc"
        };

        [Header("Model")]
        [SerializeField] private ApplicationStateModel applicationState;
        [SerializeField] private ApplicationSignInPageContent content;
        [SerializeField] private ApplicationMyInformationPageContent myInformationContent;
        [SerializeField] private ApplicationExperiencePageContent experienceContent;
        private ApplicationSessionManager applicationSession;
        private ApplicationScoreManager applicationScore;
        private GameplayTimer gameplayTimer;

        [Header("Portal Chrome")]
        [SerializeField] private TMP_Text portalTitleText;
        [SerializeField] private TMP_Text portalSubtitleText;
        [SerializeField] private TMP_Text[] progressStepLabelTexts;
        [SerializeField] private Image[] progressStepDotImages;
        [SerializeField] private Color activeProgressStepColor = new Color(0.13f, 0.42f, 0.86f, 1f);
        [SerializeField] private Color inactiveProgressStepColor = new Color(0.68f, 0.68f, 0.68f, 1f);
        [SerializeField] private Vector2 activeProgressStepSize = new Vector2(28f, 28f);
        [SerializeField] private Vector2 inactiveProgressStepSize = new Vector2(18f, 18f);

        [Header("Page Text")]
        [SerializeField] private TMP_Text pageTitleText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMP_Text errorText;

        [Header("Job Listing")]
        [SerializeField] private GameObject progressStepper;
        [SerializeField] private GameObject jobListingPanel;
        [SerializeField] private TMP_Text jobListingTitleText;
        [SerializeField] private TMP_Text jobListingDescriptionText;
        [SerializeField] private TMP_Text jobListingMinimumQualificationsHeadingText;
        [SerializeField] private TMP_Text jobListingMinimumQualificationsBodyText;
        [SerializeField] private TMP_Text jobListingBenefitsHeadingText;
        [SerializeField] private TMP_Text jobListingBenefitsBodyText;
        [SerializeField] private Button jobListingOtherRolesButton;
        [SerializeField] private TMP_Text jobListingOtherRolesButtonText;
        [SerializeField] private Color jobListingApplyButtonColor = new Color(0.13f, 0.42f, 0.86f, 1f);
        [SerializeField] private Color applicationNextButtonColor = new Color(0.62f, 0.65f, 0.72f, 1f);

        [Header("Login Controls")]
        [SerializeField] private GameObject signInFormPanel;
        [SerializeField] private TMP_Text usernameLabelText;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_Text passwordLabelText;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private TMP_Text loginButtonText;

        [Header("Two-Factor Controls")]
        [SerializeField] private GameObject twoFactorGroup;
        [SerializeField] private TMP_Text twoFactorTitleText;
        [SerializeField] private TMP_Text twoFactorBodyText;
        [SerializeField] private TMP_InputField twoFactorInput;
        [SerializeField] private Button twoFactorButton;
        [SerializeField] private TMP_Text twoFactorButtonText;

        [Header("My Information Controls")]
        [SerializeField] private GameObject myInformationPanel;
        [SerializeField] private TMP_Text myInformationIntroText;
        [SerializeField] private TMP_Text firstNameLabelText;
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private TMP_Text lastNameLabelText;
        [SerializeField] private TMP_InputField lastNameInput;
        [SerializeField] private Button confirmNameButton;
        [SerializeField] private TMP_Text confirmNameButtonText;
        [SerializeField] private GameObject dateOfBirthGroup;
        [SerializeField] private TMP_Text dateOfBirthLabelText;
        [SerializeField] private TMP_InputField dateOfBirthInput;
        [SerializeField] private TMP_Text dateOfBirthHintText;
        [SerializeField] private Button confirmDateOfBirthButton;
        [SerializeField] private TMP_Text confirmDateOfBirthButtonText;

        [Header("My Experience Controls")]
        [SerializeField] private GameObject myExperiencePanel;
        [SerializeField] private TMP_Text myExperienceIntroText;
        [SerializeField] private Button uploadResumeButton;
        [SerializeField] private TMP_Text uploadResumeButtonText;
        [SerializeField] private GameObject resumePickerPanel;
        [SerializeField] private TMP_Text resumePickerTitleText;
        [SerializeField] private TMP_Text resumePickerPathText;
        [SerializeField] private TMP_Text resumePickerStatusText;
        [SerializeField] private Button[] resumeFileButtons;
        [SerializeField] private TMP_Text[] resumeFileNameTexts;
        [SerializeField] private Image[] resumeFileIconImages;
        [SerializeField] private Button resumePickerOpenButton;
        [SerializeField] private TMP_Text resumePickerOpenButtonText;
        [SerializeField] private Button resumePickerSelectButton;
        [SerializeField] private TMP_Text resumePickerSelectButtonText;
        [SerializeField] private Button resumePickerCancelButton;
        [SerializeField] private TMP_Text resumePickerCancelButtonText;
        [SerializeField] private Color resumeFileNormalColor = new Color(0.92f, 0.92f, 0.9f, 0f);
        [SerializeField] private Color resumeFileSelectedColor = new Color(0.2f, 0.48f, 0.9f, 0.35f);

        [Header("Application Questions Controls")]
        [SerializeField] private GameObject applicationQuestionsPanel;
        [SerializeField] private TMP_Text applicationQuestionsIntroText;
        [SerializeField] private TMP_Text applicationQuestionCounterText;
        [SerializeField] private TMP_Text applicationQuestionPromptText;
        [SerializeField] private Button[] applicationQuestionAnswerButtons;
        [SerializeField] private TMP_Text[] applicationQuestionAnswerButtonTexts;
        [SerializeField] private Color questionAnswerButtonColor = new Color(0.86f, 0.88f, 0.92f, 1f);
        [SerializeField] private Color questionAnswerButtonDisabledColor = new Color(0.62f, 0.65f, 0.72f, 1f);

        [Header("Application Madlibs Controls")]
        [SerializeField] private GameObject applicationMadlibsPanel;
        [SerializeField] private TMP_Text applicationMadlibsIntroText;
        [SerializeField] private TMP_Text applicationMadlibCounterText;
        [SerializeField] private TMP_Text applicationMadlibPromptText;
        [SerializeField] private TMP_Text applicationMadlibSentenceText;
        [SerializeField] private Button[] applicationMadlibBlankButtons;
        [SerializeField] private TMP_Text[] applicationMadlibBlankButtonTexts;
        [SerializeField] private Button[] applicationMadlibWordButtons;
        [SerializeField] private TMP_Text[] applicationMadlibWordButtonTexts;
        [SerializeField] private Color madlibBlankSelectedColor = new Color(0.13f, 0.42f, 0.86f, 1f);
        [SerializeField] private Color madlibBlankNormalColor = new Color(0.86f, 0.88f, 0.92f, 1f);

        [Header("Review Controls")]
        [SerializeField] private GameObject reviewPasswordPanel;
        [SerializeField] private TMP_Text reviewPromptText;
        [SerializeField] private TMP_Text reviewPasswordLabelText;
        [SerializeField] private TMP_InputField reviewPasswordInput;
        [SerializeField] private Button reviewPasswordSubmitButton;
        [SerializeField] private TMP_Text reviewPasswordSubmitButtonText;

        [Header("Submitted Controls")]
        [SerializeField] private GameObject submittedPanel;
        [SerializeField] private TMP_Text submittedMessageText;
        [SerializeField] private bool loadWinSceneAfterSubmission = true;
        [SerializeField] private string winSceneName = "win";
        [SerializeField, Min(0f)] private float winSceneDelaySeconds = 2f;
        [SerializeField, Min(0f)] private float winSceneFadeOutSeconds = 1f;
        [SerializeField, Min(0f)] private float winSceneBlackHoldSeconds = 0.25f;
        [SerializeField] private int winSceneFadeSortingOrder = 5000;

        [Header("Web Session Timer")]
        [SerializeField] private TMP_Text sessionTimerText;
        [SerializeField, Min(1f)] private float sessionDurationSeconds = 120f;
        [SerializeField, Min(0f)] private float sessionSecondsRemaining = 120f;
        [SerializeField] private bool sessionTimerRunning;
        [SerializeField] private bool allowSessionTimerInspectorEditsInPlayMode = true;

        [Header("Session Reauthentication")]
        [SerializeField] private GameObject sessionExpiredReauthPanel;
        [SerializeField] private TMP_Text sessionExpiredTitleText;
        [SerializeField] private TMP_Text sessionExpiredBodyText;
        [SerializeField] private TMP_InputField sessionReauthInput;
        [SerializeField] private TMP_Text sessionReauthErrorText;
        [SerializeField] private Button sessionReauthSubmitButton;
        [SerializeField] private TMP_Text sessionReauthSubmitButtonText;

        [Header("Navigation")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_Text refreshButtonText;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text nextButtonText;

        private bool hasSeenExpositionLetter;
        private bool hasStartedApplication;
        private int selectedResumeIndex = -1;
        private string currentMadlibSlotId;
        private int selectedMadlibBlankIndex;
        private readonly List<string> selectedMadlibWordIds = new();
        private float lastValidatedSessionDurationSeconds;
        private float lastValidatedSessionSecondsRemaining;
        private Coroutine winSceneLoadRoutine;

        private void Awake()
        {
            ResolveApplicationState();
            ResolveApplicationSession();
            ResolveApplicationScore();
            ResolveGameplayTimer();
            ResetSessionTimer();
            CacheValidatedSessionTime();
        }

        private void OnValidate()
        {
            sessionDurationSeconds = Mathf.Max(1f, sessionDurationSeconds);
            sessionSecondsRemaining = Mathf.Clamp(sessionSecondsRemaining, 0f, sessionDurationSeconds);
            winSceneFadeOutSeconds = Mathf.Max(0f, winSceneFadeOutSeconds);
            winSceneBlackHoldSeconds = Mathf.Max(0f, winSceneBlackHoldSeconds);

            if (!global::UnityEngine.Application.isPlaying || !allowSessionTimerInspectorEditsInPlayMode)
            {
                CacheValidatedSessionTime();
                return;
            }

            bool durationChanged = !Mathf.Approximately(sessionDurationSeconds, lastValidatedSessionDurationSeconds);
            bool remainingChanged = !Mathf.Approximately(sessionSecondsRemaining, lastValidatedSessionSecondsRemaining);

            if (!durationChanged && !remainingChanged)
                return;

            if (durationChanged)
                sessionSecondsRemaining = Mathf.Min(sessionSecondsRemaining, sessionDurationSeconds);

            CacheValidatedSessionTime();
            RenderSessionTimer();

            if (sessionSecondsRemaining <= 0f && sessionTimerRunning)
                ExpireWebSession();
        }

        private void OnEnable()
        {
            ResolveApplicationState();
            ResolveApplicationSession();
            ResolveApplicationScore();
            ResolveGameplayTimer();
            Subscribe();
            AddButtonListeners();
            EnsureApplicationQuestionsPanel();
            EnsureApplicationMadlibsPanel();
            EnsureReviewPasswordPanel();
            EnsureSubmittedPanel();
            RenderStaticCopy();
            Render();
        }

        private void OnDisable()
        {
            Unsubscribe();
            RemoveButtonListeners();
        }

        private void Update()
        {
            RenderRefreshButton();
            UpdateSessionTimer();
            UpdateVisibleQuestionAnswerLabels();
        }

        public float SessionDurationSeconds
        {
            get => sessionDurationSeconds;
            set => SetSessionDurationSeconds(value, resetRemaining: false);
        }

        public float SessionSecondsRemaining
        {
            get => sessionSecondsRemaining;
            set => SetSessionSecondsRemaining(value);
        }

        public bool SessionTimerRunning => sessionTimerRunning;

        public void StartSessionTimer()
        {
            if (!ShouldRunSessionTimer(applicationState != null ? applicationState.CurrentSection : null))
                return;

            sessionTimerRunning = true;
            RenderSessionTimer();
        }

        public void StopSessionTimer()
        {
            sessionTimerRunning = false;
            RenderSessionTimer();
        }

        public void ResetSessionTimer()
        {
            sessionSecondsRemaining = Mathf.Clamp(sessionDurationSeconds, 0f, sessionDurationSeconds);
            sessionTimerRunning = false;
            CacheValidatedSessionTime();
            RenderSessionTimer();
        }

        public void SetSessionDurationSeconds(float value, bool resetRemaining = true)
        {
            sessionDurationSeconds = Mathf.Max(1f, value);

            if (resetRemaining)
                sessionSecondsRemaining = sessionDurationSeconds;
            else
                sessionSecondsRemaining = Mathf.Min(sessionSecondsRemaining, sessionDurationSeconds);

            CacheValidatedSessionTime();
            RenderSessionTimer();
        }

        public void SetSessionSecondsRemaining(float value)
        {
            sessionSecondsRemaining = Mathf.Clamp(value, 0f, sessionDurationSeconds);
            CacheValidatedSessionTime();
            RenderSessionTimer();

            if (sessionSecondsRemaining <= 0f && sessionTimerRunning)
                ExpireWebSession();
        }

        [ContextMenu("Session Timer/Set Remaining To 10 Seconds")]
        public void SetSessionRemainingToTenSeconds()
        {
            SetSessionSecondsRemaining(10f);
            StartSessionTimer();
        }

        [ContextMenu("Session Timer/Expire Now")]
        public void ExpireSessionNow()
        {
            SetSessionSecondsRemaining(0f);
            ExpireWebSession();
        }

        [ContextMenu("Session Timer/Reset And Start")]
        public void ResetAndStartSessionTimer()
        {
            ResetSessionTimer();
            StartSessionTimer();
        }

#if UNITY_EDITOR
        public string DebugCorrectUsername => CorrectUsername;
        public string DebugCorrectPassword => CorrectPassword;
        public string DebugCorrectTwoFactorCode => CorrectTwoFactorCode;
        public string DebugCorrectFirstName => CorrectFirstName;
        public string DebugCorrectLastName => CorrectLastName;
        public string DebugCurrentDateOfBirthFormat => CurrentDateOfBirthFormat;
        public string DebugCorrectDateOfBirth => BuildExpectedDateOfBirth(CurrentDateOfBirthFormat);
        public int DebugCorrectResumeIndex => CorrectResumeIndex;
        public string DebugCorrectResumeFileName => ResumeFileName(CorrectResumeIndex);

        public bool DebugJumpToJobListing()
        {
            ResolveApplicationState();
            if (applicationState == null)
                return false;

            hasSeenExpositionLetter = true;
            hasStartedApplication = false;
            applicationState.TryGoToSection(ApplicationSectionId.CreateAccountSignIn);
            applicationState.DebugForceClearCurrentSectionBlock();
            ClearInputs();
            StopSessionTimer();
            SetStatus(string.Empty);
            Render();
            return true;
        }

        public bool DebugJumpToSection(ApplicationSectionId sectionId)
        {
            ResolveApplicationState();
            if (applicationState == null || !applicationState.TryGoToSection(sectionId))
                return false;

            hasStartedApplication = true;
            applicationState.DebugForceClearCurrentSectionBlock();
            ClearInputs();
            RestartSessionTimerForCurrentSection();
            SetStatus(applicationState.CurrentSection != null
                ? Format(SectionLoadedStatusFormat, applicationState.CurrentSection.DisplayName)
                : UnavailableStatus);
            Render();
            return true;
        }
#endif

        public void SubmitLogin()
        {
            if (!IsOnSignInPageAndInteractive())
                return;

            string username = usernameInput != null ? usernameInput.text.Trim() : string.Empty;
            string password = passwordInput != null ? passwordInput.text : string.Empty;

            if (username != CorrectUsername)
            {
                Fail(UsernameChallengeId, WrongUsernameError);
                return;
            }

            if (password != CorrectPassword)
            {
                Fail(PasswordChallengeId, WrongPasswordError);
                return;
            }

            applicationState.MarkChallengeComplete(UsernameChallengeId);
            applicationState.MarkChallengeComplete(PasswordChallengeId);
            SetStatus(CredentialsAcceptedStatus);
            Render();
        }

        public void SubmitTwoFactorCode()
        {
            if (!IsOnSignInPageAndInteractive())
                return;

            string code = twoFactorInput != null ? twoFactorInput.text.Trim() : string.Empty;

            if (code != CorrectTwoFactorCode)
            {
                Fail(TwoFactorChallengeId, WrongTwoFactorError);
                return;
            }

            applicationState.MarkChallengeComplete(TwoFactorChallengeId);
            SetStatus(SignInCompleteStatus);
            Render();
        }

        public void SubmitNameInformation()
        {
            if (!IsOnMyInformationPageAndInteractive())
                return;

            string firstName = firstNameInput != null ? firstNameInput.text.Trim() : string.Empty;
            string lastName = lastNameInput != null ? lastNameInput.text.Trim() : string.Empty;

            if (!EqualsAuthoringAnswer(firstName, CorrectFirstName) || !EqualsAuthoringAnswer(lastName, CorrectLastName))
            {
                Fail(FirstNameChallengeId, IdentityMismatchError);
                return;
            }

            applicationState.MarkChallengeComplete(FirstNameChallengeId);
            applicationState.MarkChallengeComplete(LastNameChallengeId);
            SetStatus(NamesAcceptedStatus);
            Render();
        }

        public void SubmitDateOfBirth()
        {
            if (!IsOnMyInformationPageAndInteractive())
                return;

            string dateOfBirth = dateOfBirthInput != null ? dateOfBirthInput.text.Trim() : string.Empty;
            string expectedDateOfBirth = BuildExpectedDateOfBirth(CurrentDateOfBirthFormat);

            if (!string.Equals(dateOfBirth, expectedDateOfBirth, System.StringComparison.Ordinal))
            {
                Fail(DateOfBirthChallengeId, DateOfBirthMismatchError);
                return;
            }

            applicationState.MarkChallengeComplete(DateOfBirthChallengeId);
            SetStatus(MyInformationCompleteStatus);
            Render();
        }

        public void RefreshPage()
        {
            if (applicationState == null)
                return;

            if (!applicationState.TryRefreshCurrentSection())
                return;

            ClearInputs();
            RestartSessionTimerForCurrentSection();
            SetStatus(PageRefreshedStatus);
            Render();
        }

        public void NextPage()
        {
            if (!hasSeenExpositionLetter)
            {
                hasSeenExpositionLetter = true;
                SetStatus(string.Empty);
                Render();
                return;
            }

            if (!hasStartedApplication)
            {
                ApplicationSectionRuntimeState listingSection = applicationState != null ? applicationState.CurrentSection : null;
                if (listingSection != null && listingSection.IsBlocked)
                    return;

                hasStartedApplication = true;
                SetStatus(InitialStatus);
                Render();
                return;
            }

            if (applicationState == null)
                return;

            if (applicationState.TryAdvanceSection())
                SetStatus(SectionAdvancedStatus);

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            if (section != null && section.SectionId == ApplicationSectionId.MyInformation)
                SetStatus(MyInformationInitialStatus);
            else if (section != null && section.SectionId == ApplicationSectionId.MyExperience)
                SetStatus(MyExperienceInitialStatus);
            else if (section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsOne)
                SetStatus(QuestionsIntroText);
            else if (section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsTwo)
                SetStatus(MadlibIntroText);
            else if (section != null && section.SectionId == ApplicationSectionId.VoluntaryDisclosures)
                SetStatus(ReviewPromptText);
            else if (section != null && section.SectionId == ApplicationSectionId.Review)
                SetStatus(SubmittedText);

            RestartSessionTimerForCurrentSection();
            Render();
        }

        public void SelectOtherRole()
        {
            if (hasStartedApplication || applicationState == null)
                return;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            if (section != null && section.IsBlocked)
                return;

            applicationState.ReportWrongAnswer(
                JobListingChallengeId,
                JobListingOtherRolesError,
                ApplicationWrongAnswerConsequence.RequireRefresh);

            SetStatus(DelicatePortalStatus);
            Render();
        }

        public void OpenResumePicker()
        {
            if (!IsOnMyExperiencePageAndInteractive() || IsChallengeComplete(applicationState.CurrentSection, ResumeChallengeId))
                return;

            selectedResumeIndex = -1;
            SetText(resumePickerStatusText, string.Empty);
            SetActive(resumePickerPanel, true);
            RenderResumePickerButtons(canUsePicker: true);
        }

        public void CloseResumePicker()
        {
            selectedResumeIndex = -1;
            SetText(resumePickerStatusText, string.Empty);
            SetActive(resumePickerPanel, false);
            Render();
        }

        public void SelectResumeFile(int index)
        {
            if (!IsOnMyExperiencePageAndInteractive() || index < 0 || index >= ResumeFileCount)
                return;

            selectedResumeIndex = index;
            SetText(resumePickerStatusText, ResumeFileName(index));
            RenderResumePickerButtons(canUsePicker: true);
        }

        public void OpenSelectedResume()
        {
            if (!IsOnMyExperiencePageAndInteractive() || selectedResumeIndex < 0)
                return;

            SetActive(resumePickerPanel, false);
            Fail(ResumeChallengeId, WordActivationError);
        }

        public void SubmitSelectedResume()
        {
            if (!IsOnMyExperiencePageAndInteractive() || selectedResumeIndex < 0)
                return;

            if (selectedResumeIndex != CorrectResumeIndex)
            {
                SetActive(resumePickerPanel, false);
                Fail(ResumeChallengeId, IncorrectResumeError);
                return;
            }

            applicationState.MarkChallengeComplete(ResumeChallengeId);
            selectedResumeIndex = -1;
            SetActive(resumePickerPanel, false);
            SetStatus(MyExperienceCompleteStatus);
            Render();
        }

        public void SubmitQuestionAnswer(string slotId, string answerId)
        {
            if (!IsOnApplicationQuestionsOnePageAndInteractive())
                return;

            ApplicationQuestionRuntimeData question = FindQuestion(slotId);
            if (question == null)
                return;

            if (!string.Equals(answerId, question.PreferredAnswerId, System.StringComparison.OrdinalIgnoreCase))
            {
                Fail(slotId, WrongQuestionAnswerError);
                return;
            }

            ResolveApplicationScore();
            applicationScore?.RecordQuestionAnswer(slotId, answerId);
            applicationState.MarkChallengeComplete(slotId);

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            int remaining = section != null
                ? Mathf.Max(0, section.RequiredChallengeCount - section.CompletedRequiredChallengeCount)
                : 0;

            SetStatus(remaining > 0
                ? Format(QuestionAnsweredStatusFormat, remaining)
                : QuestionsCompleteStatus);

            Render();
        }

        public void SelectMadlibBlank(int blankIndex)
        {
            ApplicationMadlibRuntimeData madlib = FindCurrentMadlib(applicationState != null ? applicationState.CurrentSection : null);
            if (madlib == null || blankIndex < 0 || blankIndex >= madlib.Blanks.Count)
                return;

            EnsureMadlibSelection(madlib);
            selectedMadlibBlankIndex = blankIndex;
            RenderApplicationMadlibsSection(isApplicationQuestionsTwo: true, blocked: false);
        }

        public void SelectMadlibWord(string slotId, string wordId)
        {
            if (!IsOnApplicationQuestionsTwoPageAndInteractive())
                return;

            ApplicationMadlibRuntimeData madlib = FindMadlib(slotId);
            if (madlib == null)
                return;

            EnsureMadlibSelection(madlib);

            if (selectedMadlibBlankIndex < 0 || selectedMadlibBlankIndex >= selectedMadlibWordIds.Count)
                selectedMadlibBlankIndex = FindFirstEmptyMadlibBlank();

            if (selectedMadlibBlankIndex < 0 || selectedMadlibBlankIndex >= selectedMadlibWordIds.Count)
                return;

            selectedMadlibWordIds[selectedMadlibBlankIndex] = wordId;

            int nextEmpty = FindFirstEmptyMadlibBlank();
            if (nextEmpty >= 0)
            {
                selectedMadlibBlankIndex = nextEmpty;
                Render();
                return;
            }

            if (!madlib.IsCorrect(selectedMadlibWordIds))
            {
                Fail(slotId, WrongQuestionAnswerError);
                return;
            }

            applicationState.MarkChallengeComplete(slotId);
            ResetMadlibSelection();
            SetStatus(MadlibCompleteStatus);
            Render();
        }

        public void SubmitReviewPassword()
        {
            if (!IsOnReviewPageAndInteractive())
                return;

            string password = reviewPasswordInput != null ? reviewPasswordInput.text : string.Empty;
            if (password != CorrectPassword)
            {
                Fail(ReviewPasswordChallengeId, ReviewWrongPasswordError);
                return;
            }

            applicationState.MarkChallengeComplete(ReviewPasswordChallengeId);
            SetStatus(ReviewCompleteStatus);
            Render();
        }

        public void SubmitSessionReauthentication()
        {
            if (applicationState == null || !applicationState.CurrentSectionRequiresReauthentication)
                return;

            string code = sessionReauthInput != null ? sessionReauthInput.text.Trim() : string.Empty;

            if (code != CorrectTwoFactorCode)
            {
                if (sessionReauthInput != null)
                    sessionReauthInput.text = string.Empty;

                SetText(sessionReauthErrorText, SessionReauthWrongCodeError);
                return;
            }

            applicationState.CompleteReauthenticationForCurrentSection();
            if (sessionReauthInput != null)
                sessionReauthInput.text = string.Empty;

            SetText(sessionReauthErrorText, string.Empty);
            SetActive(sessionExpiredReauthPanel, false);
            SetStatus(SessionReauthSuccessStatus);
            Render();
        }

        private void Render()
        {
            if (applicationState == null)
                return;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            bool isSignIn = section != null && section.SectionId == ApplicationSectionId.CreateAccountSignIn;
            bool isMyInformation = section != null && section.SectionId == ApplicationSectionId.MyInformation;
            bool isMyExperience = section != null && section.SectionId == ApplicationSectionId.MyExperience;
            bool isApplicationQuestionsOne = section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsOne;
            bool isApplicationQuestionsTwo = section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsTwo;
            bool isReview = section != null && section.SectionId == ApplicationSectionId.VoluntaryDisclosures;
            bool isSubmitted = section != null && section.SectionId == ApplicationSectionId.Review;
            bool blocked = section != null && section.IsBlocked;
            bool credentialsComplete = IsChallengeComplete(section, UsernameChallengeId) && IsChallengeComplete(section, PasswordChallengeId);
            bool signInComplete = section != null && section.IsComplete;
            bool namesComplete = IsChallengeComplete(section, FirstNameChallengeId) && IsChallengeComplete(section, LastNameChallengeId);
            bool myInformationComplete = section != null && section.IsComplete;
            bool myExperienceComplete = IsChallengeComplete(section, ResumeChallengeId);
            bool reviewComplete = IsChallengeComplete(section, ReviewPasswordChallengeId);

            if (!hasStartedApplication)
            {
                if (!hasSeenExpositionLetter)
                    RenderExpositionLetter();
                else
                    RenderJobListing();

                return;
            }

            RenderApplicationChrome(section);
            RenderProgressStepper();
            RenderDateOfBirthLabel();
            RenderError(section, blocked);
            RenderSignInSection(isSignIn, blocked, credentialsComplete, signInComplete);
            RenderMyInformationSection(isMyInformation, blocked, namesComplete, myInformationComplete);
            RenderMyExperienceSection(isMyExperience, blocked, myExperienceComplete);
            RenderApplicationQuestionsOneSection(isApplicationQuestionsOne, blocked);
            RenderApplicationMadlibsSection(isApplicationQuestionsTwo, blocked);
            RenderReviewSection(isReview, blocked, reviewComplete);
            RenderSubmittedSection(isSubmitted);
            RenderSessionTimer();
            RenderSessionReauthentication();

            SetActive(nextButton, !isSubmitted);
            SetActive(refreshButton, !isSubmitted);
            SetInteractable(nextButton, !isSubmitted && applicationState.CanAdvanceCurrentSection);
            SetText(nextButtonText, isReview ? "Submit" : NextButtonLabel);
            RenderRefreshButton();
        }

        private void RenderRefreshButton()
        {
            if (applicationState == null || refreshButton == null)
                return;

            float cooldown = applicationState.RefreshCooldownRemaining;
            bool canRefresh = applicationState.CanRefreshCurrentSection;

            refreshButton.interactable = canRefresh;

            if (refreshButtonText != null)
            {
                if (applicationState.CurrentSection == null || !applicationState.CurrentSection.IsBlocked)
                    refreshButtonText.text = RefreshButtonLabel;
                else if (cooldown > 0f)
                    refreshButtonText.text = Format(RefreshCooldownFormat, cooldown);
                else
                    refreshButtonText.text = RefreshButtonLabel;
            }
        }

        private void RenderApplicationChrome(ApplicationSectionRuntimeState section)
        {
            SetText(pageTitleText, section != null ? section.DisplayName : FallbackPageTitle);
            SetText(progressText, BuildProgressText(section));
            SetActive(progressText, true);
            SetActive(progressStepper, true);
            SetActive(statusText, true);
            SetActive(refreshButton, true);
            SetActive(jobListingPanel, false);
            SetActive(jobListingOtherRolesButton, false);
            SetButtonGraphicColor(nextButton, applicationNextButtonColor);
        }

        private void RenderExpositionLetter()
        {
            SetText(pageTitleText, ExpositionLetterTitle);
            SetText(jobListingTitleText, ExpositionLetterTitle);
            SetText(jobListingDescriptionText, ExpositionLetterBody);
            SetActive(progressText, false);
            SetActive(progressStepper, false);
            SetActive(signInFormPanel, false);
            SetActive(twoFactorGroup, false);
            SetActive(myInformationPanel, false);
            SetActive(myExperiencePanel, false);
            SetActive(applicationQuestionsPanel, false);
            SetActive(applicationMadlibsPanel, false);
            SetActive(reviewPasswordPanel, false);
            SetActive(submittedPanel, false);
            SetActive(resumePickerPanel, false);
            SetActive(sessionTimerText, false);
            SetActive(sessionExpiredReauthPanel, false);
            SetActive(jobListingPanel, true);
            SetActive(jobListingMinimumQualificationsHeadingText, false);
            SetActive(jobListingMinimumQualificationsBodyText, false);
            SetActive(jobListingBenefitsHeadingText, false);
            SetActive(jobListingBenefitsBodyText, false);
            SetActive(statusText, false);
            SetActive(errorPanel, false);
            SetActive(refreshButton, false);
            SetActive(jobListingOtherRolesButton, false);
            SetActive(nextButton, true);
            SetInteractable(nextButton, true);
            SetText(nextButtonText, ExpositionLetterButtonLabel);
            SetButtonGraphicColor(nextButton, jobListingApplyButtonColor);
        }

        private void RenderSessionTimer()
        {
            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            bool show = ShouldShowSessionTimer(section);
            SetActive(sessionTimerText, show);

            if (!show)
                return;

            int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, sessionSecondsRemaining));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            SetText(sessionTimerText, Format(SessionTimerFormat, minutes, seconds));
        }

        private void RenderSessionReauthentication()
        {
            bool requiresReauthentication = applicationState != null && applicationState.CurrentSectionRequiresReauthentication;
            SetActive(sessionExpiredReauthPanel, requiresReauthentication);
            SetInteractable(sessionReauthInput, requiresReauthentication);
            SetInteractable(sessionReauthSubmitButton, requiresReauthentication);

            if (requiresReauthentication && sessionExpiredReauthPanel != null)
                sessionExpiredReauthPanel.transform.SetAsLastSibling();
        }

        private void UpdateSessionTimer()
        {
            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            if (!ShouldRunSessionTimer(section))
            {
                sessionTimerRunning = false;
                RenderSessionTimer();
                return;
            }

            if (!sessionTimerRunning)
                return;

            sessionSecondsRemaining = Mathf.Max(0f, sessionSecondsRemaining - Time.deltaTime);
            CacheValidatedSessionTime();
            RenderSessionTimer();

            if (sessionSecondsRemaining <= 0f)
                ExpireWebSession();
        }

        private void ExpireWebSession()
        {
            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            if (!ShouldRunSessionTimer(section))
                return;

            sessionTimerRunning = false;
            sessionSecondsRemaining = 0f;
            CacheValidatedSessionTime();

            if (!applicationState.ReportWrongAnswer(
                    FindSessionExpiryChallengeId(section),
                    SessionExpiredError,
                    ApplicationWrongAnswerConsequence.RequireRefresh,
                    refreshCooldownOverrideSeconds: 0f,
                    requireReauthenticationBeforeRefresh: true))
                return;

            selectedResumeIndex = -1;
            SetActive(resumePickerPanel, false);
            SetText(sessionReauthErrorText, string.Empty);

            if (sessionReauthInput != null)
                sessionReauthInput.text = string.Empty;

            SetStatus(PageBlockedStatus);
            Render();
        }

        private void RestartSessionTimerForCurrentSection()
        {
            ResetSessionTimer();

            if (ShouldRunSessionTimer(applicationState != null ? applicationState.CurrentSection : null))
                StartSessionTimer();
        }

        private bool ShouldShowSessionTimer(ApplicationSectionRuntimeState section)
        {
            return hasStartedApplication
                && section != null
                && section.SectionId >= ApplicationSectionId.MyInformation
                && section.SectionId < ApplicationSectionId.Review;
        }

        private bool ShouldRunSessionTimer(ApplicationSectionRuntimeState section)
        {
            return ShouldShowSessionTimer(section)
                && !section.IsBlocked;
        }

        private string FindSessionExpiryChallengeId(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return "session_expired";

            for (int i = 0; i < section.Challenges.Count; i++)
            {
                ApplicationChallengeRuntimeState challenge = section.Challenges[i];
                if (challenge.Required && !challenge.IsComplete)
                    return challenge.ChallengeId;
            }

            return section.Challenges.Count > 0
                ? section.Challenges[0].ChallengeId
                : "session_expired";
        }

        private void RenderError(ApplicationSectionRuntimeState section, bool blocked)
        {
            SetActive(errorPanel, blocked);
            SetActive(errorText, blocked);
            SetText(errorText, blocked && section != null ? section.ErrorMessage : string.Empty);

            if (blocked && errorPanel != null)
                errorPanel.transform.SetAsLastSibling();
        }

        private void RenderSignInSection(bool isSignIn, bool blocked, bool credentialsComplete, bool signInComplete)
        {
            bool canEditCredentials = isSignIn && !blocked && !credentialsComplete;
            bool canEditTwoFactor = isSignIn && !blocked && credentialsComplete && !signInComplete;

            SetActive(signInFormPanel, isSignIn);
            SetActive(twoFactorGroup, isSignIn && credentialsComplete);
            SetInteractable(usernameInput, canEditCredentials);
            SetInteractable(passwordInput, canEditCredentials);
            SetInteractable(loginButton, canEditCredentials);
            SetInteractable(twoFactorInput, canEditTwoFactor);
            SetInteractable(twoFactorButton, canEditTwoFactor);
        }

        private void RenderMyInformationSection(bool isMyInformation, bool blocked, bool namesComplete, bool myInformationComplete)
        {
            bool canEditName = isMyInformation && !blocked && !namesComplete;
            bool canEditDateOfBirth = isMyInformation && !blocked && namesComplete && !myInformationComplete;

            SetActive(myInformationPanel, isMyInformation);
            SetActive(dateOfBirthGroup, isMyInformation && namesComplete);
            SetInteractable(firstNameInput, canEditName);
            SetInteractable(lastNameInput, canEditName);
            SetInteractable(confirmNameButton, canEditName);
            SetInteractable(dateOfBirthInput, canEditDateOfBirth);
            SetInteractable(confirmDateOfBirthButton, canEditDateOfBirth);
        }

        private void RenderMyExperienceSection(bool isMyExperience, bool blocked, bool myExperienceComplete)
        {
            bool canUpload = isMyExperience && !blocked && !myExperienceComplete;

            SetActive(myExperiencePanel, isMyExperience);
            SetActive(uploadResumeButton, isMyExperience && !myExperienceComplete);
            SetInteractable(uploadResumeButton, canUpload);
            SetText(myExperienceIntroText, myExperienceComplete ? MyExperienceCompleteStatus : MyExperienceIntroText);

            if (!isMyExperience || blocked || myExperienceComplete)
            {
                selectedResumeIndex = -1;
                SetActive(resumePickerPanel, false);
            }

            RenderResumePickerButtons(canUpload);
        }

        private void RenderApplicationQuestionsOneSection(bool isApplicationQuestionsOne, bool blocked)
        {
            EnsureApplicationQuestionsPanel();

            SetActive(applicationQuestionsPanel, isApplicationQuestionsOne);
            if (!isApplicationQuestionsOne)
                return;

            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            ApplicationQuestionRuntimeData question = FindCurrentQuestion(section);
            bool complete = section != null && section.IsComplete;
            bool canAnswer = !blocked && !complete && question != null;

            SetText(applicationQuestionsIntroText, QuestionsIntroText);
            SetText(applicationQuestionCounterText, BuildQuestionCounterText(section));
            SetText(applicationQuestionPromptText, complete
                ? QuestionsCompleteStatus
                : question != null
                    ? question.Prompt
                    : "No question found for this slot.");

            RenderQuestionAnswerButtons(question, canAnswer);
        }

        private void RenderApplicationMadlibsSection(bool isApplicationQuestionsTwo, bool blocked)
        {
            EnsureApplicationMadlibsPanel();

            SetActive(applicationMadlibsPanel, isApplicationQuestionsTwo);
            if (!isApplicationQuestionsTwo)
                return;

            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            ApplicationMadlibRuntimeData madlib = FindCurrentMadlib(section);
            bool complete = section != null && section.IsComplete;
            bool canAnswer = !blocked && !complete && madlib != null;

            if (madlib != null)
                EnsureMadlibSelection(madlib);

            SetText(applicationMadlibsIntroText, MadlibIntroText);
            SetText(applicationMadlibCounterText, BuildMadlibCounterText(section));
            SetText(applicationMadlibPromptText, complete
                ? MadlibCompleteStatus
                : madlib != null
                    ? madlib.Prompt
                    : "No personal statement prompt found.");
            SetText(applicationMadlibSentenceText, complete
                ? string.Empty
                : BuildMadlibSentence(madlib));

            RenderMadlibBlankButtons(madlib, canAnswer);
            RenderMadlibWordButtons(madlib, canAnswer);
        }

        private void RenderReviewSection(bool isReview, bool blocked, bool complete)
        {
            EnsureReviewPasswordPanel();

            SetActive(reviewPasswordPanel, isReview);
            if (!isReview)
                return;

            bool canAnswer = !blocked && !complete;
            SetText(reviewPromptText, complete ? ReviewCompleteStatus : ReviewPromptText);
            SetText(reviewPasswordLabelText, ReviewPasswordLabel);
            SetText(reviewPasswordSubmitButtonText, ReviewSubmitButtonLabel);
            SetInteractable(reviewPasswordInput, canAnswer);
            SetInteractable(reviewPasswordSubmitButton, canAnswer);
        }

        private void RenderSubmittedSection(bool isSubmitted)
        {
            EnsureSubmittedPanel();

            SetActive(submittedPanel, isSubmitted);
            if (!isSubmitted)
                return;

            SetText(submittedMessageText, SubmittedText);
            StopSessionTimer();
            gameplayTimer?.StopTimer();
            BeginWinSceneLoad();
        }

        private void RenderQuestionAnswerButtons(ApplicationQuestionRuntimeData question, bool canAnswer)
        {
            EnsureApplicationQuestionsPanel();

            IReadOnlyList<ApplicationQuestionAnswerOption> answers = question?.PossibleAnswers;
            int answerCount = answers != null ? answers.Count : 0;

            if (applicationQuestionAnswerButtons == null)
                return;

            for (int i = 0; i < applicationQuestionAnswerButtons.Length; i++)
            {
                Button button = applicationQuestionAnswerButtons[i];
                bool active = i < answerCount;
                SetActive(button, active);

                if (button == null)
                    continue;

                button.onClick.RemoveAllListeners();
                button.interactable = canAnswer && active;
                SetButtonGraphicColor(button, button.interactable ? questionAnswerButtonColor : questionAnswerButtonDisabledColor);

                if (!active)
                    continue;

                ApplicationQuestionAnswerOption answer = answers[i];
                SetText(GetQuestionAnswerText(i), GetQuestionAnswerDisplayText(question, answer, i));

                string slotId = question.SlotId;
                string answerId = answer.AnswerId;
                button.onClick.AddListener(() => SubmitQuestionAnswer(slotId, answerId));
            }
        }

        private void RenderMadlibBlankButtons(ApplicationMadlibRuntimeData madlib, bool canAnswer)
        {
            EnsureApplicationMadlibsPanel();

            int blankCount = madlib != null ? madlib.Blanks.Count : 0;
            if (applicationMadlibBlankButtons == null)
                return;

            for (int i = 0; i < applicationMadlibBlankButtons.Length; i++)
            {
                Button button = applicationMadlibBlankButtons[i];
                bool active = i < blankCount;
                SetActive(button, active);

                if (button == null)
                    continue;

                button.onClick.RemoveAllListeners();
                button.interactable = canAnswer && active;
                SetButtonGraphicColor(button, i == selectedMadlibBlankIndex ? madlibBlankSelectedColor : madlibBlankNormalColor);

                if (!active)
                    continue;

                string chosenText = FindMadlibWordText(madlib, selectedMadlibWordIds[i]);
                SetText(GetMadlibBlankText(i), string.IsNullOrWhiteSpace(chosenText)
                    ? $"{madlib.Blanks[i].Label}: ____"
                    : $"{madlib.Blanks[i].Label}: {chosenText}");

                int blankIndex = i;
                button.onClick.AddListener(() => SelectMadlibBlank(blankIndex));
            }
        }

        private void RenderMadlibWordButtons(ApplicationMadlibRuntimeData madlib, bool canAnswer)
        {
            EnsureApplicationMadlibsPanel();

            IReadOnlyList<ApplicationMadlibWordOption> words = madlib?.WordBank;
            int wordCount = words != null ? words.Count : 0;

            if (applicationMadlibWordButtons == null)
                return;

            for (int i = 0; i < applicationMadlibWordButtons.Length; i++)
            {
                Button button = applicationMadlibWordButtons[i];
                bool active = i < wordCount;
                SetActive(button, active);

                if (button == null)
                    continue;

                button.onClick.RemoveAllListeners();
                button.interactable = canAnswer && active;
                SetButtonGraphicColor(button, questionAnswerButtonColor);

                if (!active)
                    continue;

                ApplicationMadlibWordOption word = words[i];
                SetText(GetMadlibWordText(i), word.Text);

                string slotId = madlib.SlotId;
                string wordId = word.WordId;
                button.onClick.AddListener(() => SelectMadlibWord(slotId, wordId));
            }
        }

        private void RenderResumePickerButtons(bool canUsePicker)
        {
            bool hasSelection = selectedResumeIndex >= 0;
            SetInteractable(resumePickerOpenButton, canUsePicker && hasSelection);
            SetInteractable(resumePickerSelectButton, canUsePicker && hasSelection);
            SetInteractable(resumePickerCancelButton, canUsePicker);

            if (resumeFileButtons == null)
                return;

            int fileCount = ResumeFileCount;
            for (int i = 0; i < resumeFileButtons.Length; i++)
            {
                Button button = resumeFileButtons[i];
                bool active = i < fileCount;
                SetActive(button, active);

                if (button == null || !active)
                    continue;

                button.interactable = canUsePicker;
                SetButtonGraphicColor(button, i == selectedResumeIndex ? resumeFileSelectedColor : resumeFileNormalColor);

                if (resumeFileNameTexts != null && i < resumeFileNameTexts.Length)
                    SetText(resumeFileNameTexts[i], ResumeFileName(i));

                if (resumeFileIconImages != null && i < resumeFileIconImages.Length)
                    SetActive(resumeFileIconImages[i], true);
            }
        }

        private bool IsOnSignInPageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.CreateAccountSignIn
                && !section.IsBlocked;
        }

        private bool IsOnMyInformationPageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.MyInformation
                && !section.IsBlocked;
        }

        private bool IsOnMyExperiencePageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.MyExperience
                && !section.IsBlocked;
        }

        private bool IsOnApplicationQuestionsOnePageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.ApplicationQuestionsOne
                && !section.IsBlocked;
        }

        private bool IsOnApplicationQuestionsTwoPageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.ApplicationQuestionsTwo
                && !section.IsBlocked;
        }

        private bool IsOnReviewPageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.VoluntaryDisclosures
                && !section.IsBlocked;
        }

        private void Fail(string challengeId, string message)
        {
            applicationState.ReportWrongAnswer(
                challengeId,
                message,
                ApplicationWrongAnswerConsequence.RequireRefresh);

            SetStatus(DelicatePortalStatus);
            Render();
        }

        private void RenderStaticCopy()
        {
            SetText(portalTitleText, PortalTitle);
            SetText(portalSubtitleText, PortalSubtitle);
            SetText(usernameLabelText, UsernameLabel);
            SetText(passwordLabelText, PasswordLabel);
            SetText(loginButtonText, LoginButtonLabel);
            SetText(twoFactorTitleText, TwoFactorTitle);
            SetText(twoFactorBodyText, TwoFactorBody);
            SetText(twoFactorButtonText, TwoFactorButtonLabel);
            SetText(myInformationIntroText, MyInformationIntroText);
            SetText(firstNameLabelText, FirstNameLabel);
            SetText(lastNameLabelText, LastNameLabel);
            SetText(confirmNameButtonText, ConfirmNameButtonLabel);
            SetText(confirmDateOfBirthButtonText, ConfirmDateOfBirthButtonLabel);
            SetText(dateOfBirthHintText, DateOfBirthHintText);
            SetText(myExperienceIntroText, MyExperienceIntroText);
            SetText(uploadResumeButtonText, UploadResumeButtonLabel);
            SetText(resumePickerTitleText, ResumePickerTitle);
            SetText(resumePickerPathText, ResumePickerPath);
            SetText(resumePickerOpenButtonText, ResumePickerOpenButtonLabel);
            SetText(resumePickerSelectButtonText, ResumePickerSelectButtonLabel);
            SetText(resumePickerCancelButtonText, ResumePickerCancelButtonLabel);
            SetText(resumePickerStatusText, string.Empty);
            SetText(sessionExpiredTitleText, SessionExpiredTitle);
            SetText(sessionExpiredBodyText, SessionExpiredBody);
            SetText(sessionReauthSubmitButtonText, SessionReauthSubmitButtonLabel);
            SetText(sessionReauthErrorText, string.Empty);
            SetText(reviewPromptText, ReviewPromptText);
            SetText(reviewPasswordLabelText, ReviewPasswordLabel);
            SetText(reviewPasswordSubmitButtonText, ReviewSubmitButtonLabel);
            SetText(submittedMessageText, SubmittedText);
            SetText(jobListingTitleText, JobListingTitle);
            SetText(jobListingDescriptionText, JobListingDescription);
            SetText(jobListingMinimumQualificationsHeadingText, JobListingMinimumQualificationsHeading);
            SetText(jobListingMinimumQualificationsBodyText, GetJobListingMinimumQualificationsBody(refreshed: false));
            SetText(jobListingBenefitsHeadingText, JobListingBenefitsHeading);
            SetText(jobListingBenefitsBodyText, GetJobListingBenefitsBody(refreshed: false));
            SetText(jobListingOtherRolesButtonText, JobListingOtherRolesButtonLabel);
            SetText(refreshButtonText, RefreshButtonLabel);
            SetText(nextButtonText, NextButtonLabel);
            SetInputPlaceholder(usernameInput, UsernamePlaceholder);
            SetInputPlaceholder(passwordInput, PasswordPlaceholder);
            SetInputPlaceholder(twoFactorInput, TwoFactorPlaceholder);
            SetInputPlaceholder(firstNameInput, FirstNamePlaceholder);
            SetInputPlaceholder(lastNameInput, LastNamePlaceholder);
            SetInputPlaceholder(dateOfBirthInput, DateOfBirthPlaceholder);
            SetInputPlaceholder(sessionReauthInput, SessionReauthPlaceholder);
            SetInputPlaceholder(reviewPasswordInput, ReviewPasswordPlaceholder);
            RenderDateOfBirthLabel();
            RenderResumePickerButtons(canUsePicker: false);
            RenderSessionTimer();
            RenderSessionReauthentication();

            if (statusText != null && string.IsNullOrWhiteSpace(statusText.text))
                SetStatus(InitialStatus);
        }

        private void RenderProgressStepper()
        {
            if (applicationState == null)
                return;

            if (progressStepLabelTexts != null)
            {
                int count = Mathf.Min(progressStepLabelTexts.Length, applicationState.Sections.Count);
                for (int i = 0; i < count; i++)
                    SetText(progressStepLabelTexts[i], applicationState.Sections[i].ProgressLabel);
            }

            if (progressStepDotImages == null)
                return;

            int dotCount = Mathf.Min(progressStepDotImages.Length, applicationState.Sections.Count);
            int activeIndex = applicationState.CurrentSectionIndex;
            for (int i = 0; i < dotCount; i++)
            {
                if (progressStepDotImages[i] == null)
                    continue;

                progressStepDotImages[i].color = i == activeIndex
                    ? activeProgressStepColor
                    : inactiveProgressStepColor;

                RectTransform dotRectTransform = progressStepDotImages[i].rectTransform;
                dotRectTransform.sizeDelta = i == activeIndex
                    ? activeProgressStepSize
                    : inactiveProgressStepSize;
            }
        }

        private void RenderJobListing()
        {
            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            bool blocked = section != null && section.IsBlocked;
            bool refreshed = section != null && section.RefreshCount > 0;

            SetText(pageTitleText, JobListingTitle);
            SetText(jobListingTitleText, JobListingTitle);
            SetText(jobListingDescriptionText, JobListingDescription);
            SetText(jobListingMinimumQualificationsHeadingText, JobListingMinimumQualificationsHeading);
            SetText(jobListingMinimumQualificationsBodyText, GetJobListingMinimumQualificationsBody(refreshed));
            SetText(jobListingBenefitsHeadingText, JobListingBenefitsHeading);
            SetText(jobListingBenefitsBodyText, GetJobListingBenefitsBody(refreshed));
            SetActive(progressText, false);
            SetActive(progressStepper, false);
            SetActive(signInFormPanel, false);
            SetActive(twoFactorGroup, false);
            SetActive(myInformationPanel, false);
            SetActive(myExperiencePanel, false);
            SetActive(applicationQuestionsPanel, false);
            SetActive(applicationMadlibsPanel, false);
            SetActive(reviewPasswordPanel, false);
            SetActive(submittedPanel, false);
            SetActive(resumePickerPanel, false);
            SetActive(sessionTimerText, false);
            SetActive(sessionExpiredReauthPanel, false);
            SetActive(jobListingPanel, true);
            SetActive(jobListingMinimumQualificationsHeadingText, true);
            SetActive(jobListingMinimumQualificationsBodyText, true);
            SetActive(jobListingBenefitsHeadingText, true);
            SetActive(jobListingBenefitsBodyText, true);
            SetActive(statusText, false);
            SetActive(refreshButton, true);
            SetActive(jobListingOtherRolesButton, true);
            RenderError(section, blocked);

            SetInteractable(nextButton, !blocked);
            SetInteractable(jobListingOtherRolesButton, !blocked);
            SetText(nextButtonText, JobListingApplyButtonLabel);
            SetButtonGraphicColor(nextButton, jobListingApplyButtonColor);
            SetButtonGraphicColor(jobListingOtherRolesButton, jobListingApplyButtonColor);

            SetStatus(string.Empty);
            RenderRefreshButton();
        }

        private void RenderDateOfBirthLabel()
        {
            SetText(dateOfBirthLabelText, Format(DateOfBirthLabelFormat, CurrentDateOfBirthFormat));
        }

        private void ClearInputs()
        {
            if (usernameInput != null)
                usernameInput.text = string.Empty;

            if (passwordInput != null)
                passwordInput.text = string.Empty;

            if (twoFactorInput != null)
                twoFactorInput.text = string.Empty;

            if (firstNameInput != null)
                firstNameInput.text = string.Empty;

            if (lastNameInput != null)
                lastNameInput.text = string.Empty;

            if (dateOfBirthInput != null)
                dateOfBirthInput.text = string.Empty;

            if (reviewPasswordInput != null)
                reviewPasswordInput.text = string.Empty;

            selectedResumeIndex = -1;
            ResetMadlibSelection();
            SetText(resumePickerStatusText, string.Empty);
            SetActive(resumePickerPanel, false);
            SetText(sessionReauthErrorText, string.Empty);

            if (sessionReauthInput != null)
                sessionReauthInput.text = string.Empty;

            SetActive(sessionExpiredReauthPanel, false);
        }

        private void ResolveApplicationState()
        {
            if (applicationState != null)
                return;

            if (Game.Ctx != null)
                applicationState = Game.Ctx.ApplicationState;

            if (applicationState == null)
                applicationState = FindAnyObjectByType<ApplicationStateModel>();
        }

        private void ResolveApplicationSession()
        {
            if (applicationSession != null)
                return;

            if (Game.Ctx != null)
                applicationSession = Game.Ctx.ApplicationSession;

            if (applicationSession == null)
                applicationSession = FindAnyObjectByType<ApplicationSessionManager>();
        }

        private void ResolveApplicationScore()
        {
            if (applicationScore != null)
                return;

            if (Game.Ctx != null)
                applicationScore = Game.Ctx.Score;

            if (applicationScore == null)
                applicationScore = FindAnyObjectByType<ApplicationScoreManager>();
        }

        private void ResolveGameplayTimer()
        {
            if (gameplayTimer != null)
                return;

            if (Game.Ctx != null)
                gameplayTimer = Game.Ctx.Timer;

            if (gameplayTimer == null)
                gameplayTimer = FindAnyObjectByType<GameplayTimer>();
        }

        private ApplicationQuestionRuntimeData FindCurrentQuestion(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return null;

            for (int i = 0; i < section.Challenges.Count; i++)
            {
                ApplicationChallengeRuntimeState challenge = section.Challenges[i];
                if (challenge == null
                    || challenge.IsComplete
                    || !IsQuestionChallengeId(challenge.ChallengeId))
                {
                    continue;
                }

                return FindQuestion(challenge.ChallengeId);
            }

            return null;
        }

        private ApplicationQuestionRuntimeData FindQuestion(string slotId)
        {
            ResolveApplicationSession();
            return applicationSession != null
                ? applicationSession.FindQuestionBySlot(slotId)
                : null;
        }

        private ApplicationMadlibRuntimeData FindCurrentMadlib(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return null;

            for (int i = 0; i < section.Challenges.Count; i++)
            {
                ApplicationChallengeRuntimeState challenge = section.Challenges[i];
                if (challenge == null
                    || challenge.IsComplete
                    || !IsMadlibChallengeId(challenge.ChallengeId))
                {
                    continue;
                }

                return FindMadlib(challenge.ChallengeId);
            }

            return null;
        }

        private ApplicationMadlibRuntimeData FindMadlib(string slotId)
        {
            ResolveApplicationSession();
            return applicationSession != null
                ? applicationSession.FindMadlibBySlot(slotId)
                : null;
        }

        private string BuildQuestionCounterText(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return string.Empty;

            int completed = section.CompletedRequiredChallengeCount;
            int total = section.RequiredChallengeCount;
            int current = Mathf.Min(completed + 1, total);

            return section.IsComplete
                ? $"{total}/{total} questions complete"
                : $"Question {current} of {total}";
        }

        private static bool IsQuestionChallengeId(string challengeId)
        {
            return !string.IsNullOrWhiteSpace(challengeId)
                && challengeId.StartsWith("question_", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMadlibChallengeId(string challengeId)
        {
            return !string.IsNullOrWhiteSpace(challengeId)
                && challengeId.StartsWith("madlib_", System.StringComparison.OrdinalIgnoreCase);
        }

        private string BuildMadlibCounterText(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return string.Empty;

            int completed = section.CompletedRequiredChallengeCount;
            int total = section.RequiredChallengeCount;
            int current = Mathf.Min(completed + 1, total);

            return section.IsComplete
                ? $"{total}/{total} statements complete"
                : $"Statement {current} of {total}";
        }

        private string BuildMadlibSentence(ApplicationMadlibRuntimeData madlib)
        {
            if (madlib == null)
                return string.Empty;

            EnsureMadlibSelection(madlib);

            object[] values = new object[madlib.Blanks.Count];
            for (int i = 0; i < values.Length; i++)
            {
                string selectedWordId = i < selectedMadlibWordIds.Count
                    ? selectedMadlibWordIds[i]
                    : string.Empty;
                string word = FindMadlibWordText(madlib, selectedWordId);
                values[i] = string.IsNullOrWhiteSpace(word) ? "____" : word;
            }

            return Format(madlib.SentenceFormat, values);
        }

        private void EnsureMadlibSelection(ApplicationMadlibRuntimeData madlib)
        {
            if (madlib == null)
                return;

            if (currentMadlibSlotId == madlib.SlotId
                && selectedMadlibWordIds.Count == madlib.Blanks.Count)
            {
                return;
            }

            currentMadlibSlotId = madlib.SlotId;
            selectedMadlibWordIds.Clear();
            for (int i = 0; i < madlib.Blanks.Count; i++)
                selectedMadlibWordIds.Add(string.Empty);

            selectedMadlibBlankIndex = selectedMadlibWordIds.Count > 0 ? 0 : -1;
        }

        private void ResetMadlibSelection()
        {
            currentMadlibSlotId = string.Empty;
            selectedMadlibBlankIndex = 0;
            selectedMadlibWordIds.Clear();
        }

        private int FindFirstEmptyMadlibBlank()
        {
            for (int i = 0; i < selectedMadlibWordIds.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(selectedMadlibWordIds[i]))
                    return i;
            }

            return -1;
        }

        private static string FindMadlibWordText(ApplicationMadlibRuntimeData madlib, string wordId)
        {
            if (madlib == null || string.IsNullOrWhiteSpace(wordId))
                return string.Empty;

            IReadOnlyList<ApplicationMadlibWordOption> words = madlib.WordBank;
            for (int i = 0; i < words.Count; i++)
            {
                if (string.Equals(words[i].WordId, wordId, System.StringComparison.OrdinalIgnoreCase))
                    return words[i].Text;
            }

            return string.Empty;
        }

        private void UpdateVisibleQuestionAnswerLabels()
        {
            if (applicationQuestionsPanel == null || !applicationQuestionsPanel.activeInHierarchy)
                return;

            ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
            ApplicationQuestionRuntimeData question = FindCurrentQuestion(section);
            if (question == null || question.QuestionId != TimeLeftSecondsQuestionId)
                return;

            IReadOnlyList<ApplicationQuestionAnswerOption> answers = question.PossibleAnswers;
            int count = Mathf.Min(
                answers != null ? answers.Count : 0,
                applicationQuestionAnswerButtons != null ? applicationQuestionAnswerButtons.Length : 0);

            for (int i = 0; i < count; i++)
                SetText(GetQuestionAnswerText(i), GetQuestionAnswerDisplayText(question, answers[i], i));
        }

        private string GetQuestionAnswerDisplayText(
            ApplicationQuestionRuntimeData question,
            ApplicationQuestionAnswerOption answer,
            int answerIndex)
        {
            if (question == null
                || answer == null
                || question.QuestionId != TimeLeftSecondsQuestionId)
            {
                return answer != null ? answer.AnswerText : string.Empty;
            }

            return GetDisplayedCountdownSeconds(answer.AnswerId, answerIndex).ToString();
        }

        private int GetDisplayedCountdownSeconds(string answerId, int answerIndex)
        {
            ResolveGameplayTimer();

            float actualRemaining = gameplayTimer != null
                ? gameplayTimer.SecondsRemaining
                : 0f;

            int actualSeconds = Mathf.Max(0, Mathf.FloorToInt(actualRemaining));
            if (string.Equals(answerId, CorrectTimeLeftAnswerId, System.StringComparison.OrdinalIgnoreCase))
                return actualSeconds;

            int[] offsets = { 17, -23, 41, -9 };
            int offset = offsets[Mathf.Abs(answerIndex) % offsets.Length];
            return Mathf.Max(0, actualSeconds + offset);
        }

        private ApplicationApplicantRuntimeData CurrentApplicant
        {
            get
            {
                //I use the randomized values for this run first and the existing page content if there is no session.
                ResolveApplicationSession();
                return applicationSession != null
                    ? applicationSession.Current?.Applicant
                    : null;
            }
        }

        private void Subscribe()
        {
            if (applicationState == null)
                return;

            applicationState.StateChanged += Render;
            applicationState.SectionChanged += HandleSectionChanged;
            applicationState.PageBlocked += HandlePageBlocked;
            applicationState.PageRefreshed += HandlePageRefreshed;
        }

        private void Unsubscribe()
        {
            if (applicationState == null)
                return;

            applicationState.StateChanged -= Render;
            applicationState.SectionChanged -= HandleSectionChanged;
            applicationState.PageBlocked -= HandlePageBlocked;
            applicationState.PageRefreshed -= HandlePageRefreshed;
        }

        private void AddButtonListeners()
        {
            if (loginButton != null)
                loginButton.onClick.AddListener(SubmitLogin);

            if (twoFactorButton != null)
                twoFactorButton.onClick.AddListener(SubmitTwoFactorCode);

            if (confirmNameButton != null)
                confirmNameButton.onClick.AddListener(SubmitNameInformation);

            if (confirmDateOfBirthButton != null)
                confirmDateOfBirthButton.onClick.AddListener(SubmitDateOfBirth);

            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshPage);

            if (nextButton != null)
                nextButton.onClick.AddListener(NextPage);

            if (jobListingOtherRolesButton != null)
                jobListingOtherRolesButton.onClick.AddListener(SelectOtherRole);

            if (uploadResumeButton != null)
                uploadResumeButton.onClick.AddListener(OpenResumePicker);

            if (resumePickerOpenButton != null)
                resumePickerOpenButton.onClick.AddListener(OpenSelectedResume);

            if (resumePickerSelectButton != null)
                resumePickerSelectButton.onClick.AddListener(SubmitSelectedResume);

            if (resumePickerCancelButton != null)
                resumePickerCancelButton.onClick.AddListener(CloseResumePicker);

            if (sessionReauthSubmitButton != null)
                sessionReauthSubmitButton.onClick.AddListener(SubmitSessionReauthentication);

            if (reviewPasswordSubmitButton != null)
                reviewPasswordSubmitButton.onClick.AddListener(SubmitReviewPassword);

            if (resumeFileButtons != null)
            {
                for (int i = 0; i < resumeFileButtons.Length; i++)
                {
                    if (resumeFileButtons[i] == null)
                        continue;

                    int fileIndex = i;
                    resumeFileButtons[i].onClick.RemoveAllListeners();
                    resumeFileButtons[i].onClick.AddListener(() => SelectResumeFile(fileIndex));
                }
            }
        }

        private void RemoveButtonListeners()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(SubmitLogin);

            if (twoFactorButton != null)
                twoFactorButton.onClick.RemoveListener(SubmitTwoFactorCode);

            if (confirmNameButton != null)
                confirmNameButton.onClick.RemoveListener(SubmitNameInformation);

            if (confirmDateOfBirthButton != null)
                confirmDateOfBirthButton.onClick.RemoveListener(SubmitDateOfBirth);

            if (refreshButton != null)
                refreshButton.onClick.RemoveListener(RefreshPage);

            if (nextButton != null)
                nextButton.onClick.RemoveListener(NextPage);

            if (jobListingOtherRolesButton != null)
                jobListingOtherRolesButton.onClick.RemoveListener(SelectOtherRole);

            if (uploadResumeButton != null)
                uploadResumeButton.onClick.RemoveListener(OpenResumePicker);

            if (resumePickerOpenButton != null)
                resumePickerOpenButton.onClick.RemoveListener(OpenSelectedResume);

            if (resumePickerSelectButton != null)
                resumePickerSelectButton.onClick.RemoveListener(SubmitSelectedResume);

            if (resumePickerCancelButton != null)
                resumePickerCancelButton.onClick.RemoveListener(CloseResumePicker);

            if (sessionReauthSubmitButton != null)
                sessionReauthSubmitButton.onClick.RemoveListener(SubmitSessionReauthentication);

            if (reviewPasswordSubmitButton != null)
                reviewPasswordSubmitButton.onClick.RemoveListener(SubmitReviewPassword);

            if (applicationQuestionAnswerButtons != null)
            {
                foreach (Button button in applicationQuestionAnswerButtons)
                {
                    if (button != null)
                        button.onClick.RemoveAllListeners();
                }
            }

            if (applicationMadlibBlankButtons != null)
            {
                foreach (Button button in applicationMadlibBlankButtons)
                {
                    if (button != null)
                        button.onClick.RemoveAllListeners();
                }
            }

            if (applicationMadlibWordButtons != null)
            {
                foreach (Button button in applicationMadlibWordButtons)
                {
                    if (button != null)
                        button.onClick.RemoveAllListeners();
                }
            }

            if (resumeFileButtons != null)
            {
                foreach (Button button in resumeFileButtons)
                {
                    if (button != null)
                        button.onClick.RemoveAllListeners();
                }
            }
        }

        private void HandleSectionChanged(ApplicationSectionRuntimeState section)
        {
            SetStatus(section != null
                ? Format(SectionLoadedStatusFormat, section.DisplayName)
                : UnavailableStatus);

            if (section != null && section.SectionId == ApplicationSectionId.MyInformation)
                SetStatus(MyInformationInitialStatus);
            else if (section != null && section.SectionId == ApplicationSectionId.MyExperience)
                SetStatus(MyExperienceInitialStatus);
            else if (section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsOne)
                SetStatus(QuestionsIntroText);
            else if (section != null && section.SectionId == ApplicationSectionId.ApplicationQuestionsTwo)
                SetStatus(MadlibIntroText);
            else if (section != null && section.SectionId == ApplicationSectionId.VoluntaryDisclosures)
                SetStatus(ReviewPromptText);
            else if (section != null && section.SectionId == ApplicationSectionId.Review)
                SetStatus(SubmittedText);

            Render();
        }

        private void HandlePageBlocked(ApplicationSectionRuntimeState section)
        {
            SetStatus(PageBlockedStatus);
            Render();
        }

        private void HandlePageRefreshed(ApplicationSectionRuntimeState section)
        {
            RestartSessionTimerForCurrentSection();
            Render();
        }

        private void SetStatus(string status)
        {
            SetText(statusText, status);
        }

        private static bool IsChallengeComplete(ApplicationSectionRuntimeState section, string challengeId)
        {
            ApplicationChallengeRuntimeState challenge = section?.FindChallenge(challengeId);
            return challenge != null && challenge.IsComplete;
        }

        private string BuildProgressText(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return NoActiveSectionProgress;

            return Format(ProgressFormat, section.CompletedRequiredChallengeCount, section.RequiredChallengeCount);
        }

        private static void SetText(TMP_Text target, string value)
        {
            if (target != null)
                target.text = value;
        }

        private static void SetActive(Component target, bool active)
        {
            if (target != null)
                target.gameObject.SetActive(active);
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
                target.SetActive(active);
        }

        private static void SetInteractable(Selectable target, bool interactable)
        {
            if (target != null)
                target.interactable = interactable;
        }

        private static void SetButtonGraphicColor(Button button, Color color)
        {
            if (button != null && button.targetGraphic != null)
                button.targetGraphic.color = color;
        }

        private static void SetInputPlaceholder(TMP_InputField input, string value)
        {
            if (input == null || input.placeholder == null)
                return;

            TMP_Text placeholder = input.placeholder.GetComponent<TMP_Text>();
            if (placeholder != null)
                placeholder.text = value;
        }

        private void BeginWinSceneLoad()
        {
            if (!loadWinSceneAfterSubmission || string.IsNullOrWhiteSpace(winSceneName) || winSceneLoadRoutine != null)
                return;

            winSceneLoadRoutine = StartCoroutine(LoadWinSceneAfterDelay());
        }

        private IEnumerator LoadWinSceneAfterDelay()
        {
            if (winSceneDelaySeconds > 0f)
                yield return new WaitForSecondsRealtime(winSceneDelaySeconds);

            SceneTransitioner.LoadScene(
                winSceneName,
                winSceneFadeOutSeconds,
                winSceneBlackHoldSeconds,
                winSceneFadeOutSeconds,
                winSceneFadeSortingOrder);
        }

        private void EnsureReviewPasswordPanel()
        {
            if (reviewPasswordPanel != null
                && reviewPasswordInput != null
                && reviewPasswordSubmitButton != null)
            {
                return;
            }

            RectTransform parent = signInFormPanel != null
                ? signInFormPanel.transform.parent as RectTransform
                : transform as RectTransform;

            if (parent == null)
                return;

            GameObject panel = new GameObject("Review Password Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            MatchSmallPanelToExperiencePanel(panelRect);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(1f, 0.98f, 0.9f, 0.92f);

            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            reviewPasswordPanel = panel;
            reviewPromptText = CreateQuestionText(panel.transform, "Prompt", 16f, FontStyles.Bold, 42f);
            reviewPasswordLabelText = CreateQuestionText(panel.transform, "Password Label", 13f, FontStyles.Bold, 18f);
            reviewPasswordInput = CreateReviewInputField(panel.transform, "Password Input");
            reviewPasswordSubmitButton = CreateReviewButton(panel.transform, "Submit Button", out reviewPasswordSubmitButtonText);
            reviewPasswordSubmitButton.onClick.AddListener(SubmitReviewPassword);

            SetActive(reviewPasswordPanel, false);
        }

        private void EnsureSubmittedPanel()
        {
            if (submittedPanel != null && submittedMessageText != null)
                return;

            RectTransform parent = signInFormPanel != null
                ? signInFormPanel.transform.parent as RectTransform
                : transform as RectTransform;

            if (parent == null)
                return;

            GameObject panel = new GameObject("Submitted Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            MatchSmallPanelToExperiencePanel(panelRect);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(1f, 0.98f, 0.9f, 0.92f);

            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 36, 18);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            submittedMessageText = CreateQuestionText(panel.transform, "Message", 24f, FontStyles.Bold, 90f);
            submittedMessageText.alignment = TextAlignmentOptions.Center;

            submittedPanel = panel;
            SetActive(submittedPanel, false);
        }

        private void MatchSmallPanelToExperiencePanel(RectTransform panelRect)
        {
            RectTransform sourceRect = myExperiencePanel != null
                ? myExperiencePanel.GetComponent<RectTransform>()
                : null;

            if (sourceRect == null)
            {
                panelRect.anchorMin = new Vector2(0f, 1f);
                panelRect.anchorMax = new Vector2(0f, 1f);
                panelRect.pivot = new Vector2(0f, 1f);
                panelRect.anchoredPosition = new Vector2(92f, -230f);
                panelRect.sizeDelta = new Vector2(560f, 170f);
                return;
            }

            panelRect.anchorMin = sourceRect.anchorMin;
            panelRect.anchorMax = sourceRect.anchorMax;
            panelRect.pivot = sourceRect.pivot;
            panelRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(0f, 36f);
            panelRect.sizeDelta = new Vector2(sourceRect.sizeDelta.x, 170f);
        }

        private TMP_InputField CreateReviewInputField(Transform parent, string name)
        {
            GameObject inputObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField), typeof(LayoutElement));
            inputObject.transform.SetParent(parent, false);

            Image image = inputObject.GetComponent<Image>();
            image.color = new Color(0.86f, 0.86f, 0.82f, 1f);

            LayoutElement layoutElement = inputObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 36f;
            layoutElement.preferredHeight = 36f;

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(inputObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10f, 6f);
            textRect.offsetMax = new Vector2(-10f, -6f);

            TMP_Text text = textObject.GetComponent<TMP_Text>();
            text.font = pageTitleText != null ? pageTitleText.font : text.font;
            text.fontSize = 16f;
            text.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.alignment = TextAlignmentOptions.MidlineLeft;

            GameObject placeholderObject = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
            placeholderObject.transform.SetParent(inputObject.transform, false);
            RectTransform placeholderRect = placeholderObject.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10f, 6f);
            placeholderRect.offsetMax = new Vector2(-10f, -6f);

            TMP_Text placeholder = placeholderObject.GetComponent<TMP_Text>();
            placeholder.font = text.font;
            placeholder.fontSize = 16f;
            placeholder.color = new Color(0.45f, 0.45f, 0.45f, 1f);
            placeholder.enableWordWrapping = false;
            placeholder.overflowMode = TextOverflowModes.Ellipsis;
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;
            placeholder.text = ReviewPasswordPlaceholder;

            TMP_InputField input = inputObject.GetComponent<TMP_InputField>();
            input.textComponent = text;
            input.placeholder = placeholder;
            input.targetGraphic = image;
            input.contentType = TMP_InputField.ContentType.Password;
            input.lineType = TMP_InputField.LineType.SingleLine;

            return input;
        }

        private Button CreateReviewButton(
            Transform parent,
            string name,
            out TMP_Text buttonText)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = jobListingApplyButtonColor;

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 36f;
            layoutElement.preferredHeight = 36f;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(buttonObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);

            buttonText = textObject.GetComponent<TMP_Text>();
            buttonText.font = pageTitleText != null ? pageTitleText.font : buttonText.font;
            buttonText.fontSize = 15f;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.white;
            buttonText.enableWordWrapping = false;
            buttonText.overflowMode = TextOverflowModes.Ellipsis;
            buttonText.alignment = TextAlignmentOptions.Center;

            return button;
        }

        private void EnsureApplicationQuestionsPanel()
        {
            if (applicationQuestionsPanel != null
                && applicationQuestionPromptText != null
                && applicationQuestionAnswerButtons != null
                && applicationQuestionAnswerButtons.Length > 0)
            {
                return;
            }

            RectTransform parent = signInFormPanel != null
                ? signInFormPanel.transform.parent as RectTransform
                : transform as RectTransform;

            if (parent == null)
                return;

            GameObject panel = new GameObject("Application Questions Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            MatchQuestionsPanelToExperiencePanel(panelRect);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(1f, 0.98f, 0.9f, 0.92f);

            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 10, 10);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            applicationQuestionsPanel = panel;
            applicationQuestionsIntroText = CreateQuestionText(panel.transform, "Intro", 13f, FontStyles.Normal, 18f);
            applicationQuestionCounterText = CreateQuestionText(panel.transform, "Counter", 13f, FontStyles.Bold, 18f);
            applicationQuestionPromptText = CreateQuestionText(panel.transform, "Prompt", 15f, FontStyles.Bold, 46f);

            Transform answerParent = CreateQuestionAnswerGrid(panel.transform);

            applicationQuestionAnswerButtons = new Button[4];
            applicationQuestionAnswerButtonTexts = new TMP_Text[4];
            for (int i = 0; i < applicationQuestionAnswerButtons.Length; i++)
            {
                Button button = CreateQuestionButton(answerParent, $"Answer {i + 1}", out TMP_Text buttonText);
                applicationQuestionAnswerButtons[i] = button;
                applicationQuestionAnswerButtonTexts[i] = buttonText;
            }

            SetActive(applicationQuestionsPanel, false);
        }

        private void MatchQuestionsPanelToExperiencePanel(RectTransform panelRect)
        {
            RectTransform sourceRect = myExperiencePanel != null
                ? myExperiencePanel.GetComponent<RectTransform>()
                : null;

            if (sourceRect == null)
            {
                panelRect.anchorMin = new Vector2(0f, 1f);
                panelRect.anchorMax = new Vector2(0f, 1f);
                panelRect.pivot = new Vector2(0f, 1f);
                panelRect.anchoredPosition = new Vector2(92f, -230f);
                panelRect.sizeDelta = new Vector2(620f, 230f);
                return;
            }

            panelRect.anchorMin = sourceRect.anchorMin;
            panelRect.anchorMax = sourceRect.anchorMax;
            panelRect.pivot = sourceRect.pivot;
            panelRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(0f, 36f);
            panelRect.sizeDelta = sourceRect.sizeDelta + new Vector2(60f, 50f);
        }

        private TMP_Text CreateQuestionText(
            Transform parent,
            string name,
            float fontSize,
            FontStyles fontStyle,
            float preferredHeight)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            TMP_Text text = textObject.GetComponent<TMP_Text>();
            text.font = pageTitleText != null ? pageTitleText.font : text.font;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.alignment = TextAlignmentOptions.Left;

            LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = Mathf.Min(preferredHeight, fontSize + 8f);
            layoutElement.preferredHeight = preferredHeight;

            return text;
        }

        private Transform CreateQuestionAnswerGrid(Transform parent)
        {
            GameObject gridObject = new GameObject("Answer Grid", typeof(RectTransform), typeof(GridLayoutGroup), typeof(LayoutElement));
            gridObject.transform.SetParent(parent, false);

            GridLayoutGroup grid = gridObject.GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.spacing = new Vector2(6f, 5f);
            grid.cellSize = new Vector2(288f, 31f);
            grid.childAlignment = TextAnchor.UpperLeft;

            LayoutElement layoutElement = gridObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 67f;
            layoutElement.preferredHeight = 67f;

            return gridObject.transform;
        }

        private void EnsureApplicationMadlibsPanel()
        {
            if (applicationMadlibsPanel != null
                && applicationMadlibSentenceText != null
                && applicationMadlibWordButtons != null
                && applicationMadlibWordButtons.Length > 0)
            {
                return;
            }

            RectTransform parent = signInFormPanel != null
                ? signInFormPanel.transform.parent as RectTransform
                : transform as RectTransform;

            if (parent == null)
                return;

            GameObject panel = new GameObject("Application Madlibs Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            MatchQuestionsPanelToExperiencePanel(panelRect);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(1f, 0.98f, 0.9f, 0.92f);

            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 3f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            applicationMadlibsPanel = panel;
            applicationMadlibsIntroText = CreateQuestionText(panel.transform, "Intro", 11f, FontStyles.Normal, 36f);
            applicationMadlibCounterText = CreateQuestionText(panel.transform, "Counter", 12f, FontStyles.Bold, 18f);
            applicationMadlibPromptText = CreateQuestionText(panel.transform, "Prompt", 13f, FontStyles.Bold, 26f);
            applicationMadlibSentenceText = CreateQuestionText(panel.transform, "Sentence", 14f, FontStyles.Bold, 32f);

            Transform blankParent = CreateMadlibGrid(panel.transform, "Blank Grid", 2, new Vector2(293f, 24f), 25f);
            applicationMadlibBlankButtons = new Button[4];
            applicationMadlibBlankButtonTexts = new TMP_Text[4];
            for (int i = 0; i < applicationMadlibBlankButtons.Length; i++)
            {
                Button button = CreateMadlibButton(blankParent, $"Blank {i + 1}", 10f, out TMP_Text buttonText);
                applicationMadlibBlankButtons[i] = button;
                applicationMadlibBlankButtonTexts[i] = buttonText;
            }

            Transform wordParent = CreateMadlibGrid(panel.transform, "Word Bank", 5, new Vector2(113f, 22f), 47f);
            applicationMadlibWordButtons = new Button[10];
            applicationMadlibWordButtonTexts = new TMP_Text[10];
            for (int i = 0; i < applicationMadlibWordButtons.Length; i++)
            {
                Button button = CreateMadlibButton(wordParent, $"Word {i + 1}", 10f, out TMP_Text buttonText);
                applicationMadlibWordButtons[i] = button;
                applicationMadlibWordButtonTexts[i] = buttonText;
            }

            SetActive(applicationMadlibsPanel, false);
        }

        private Transform CreateMadlibGrid(
            Transform parent,
            string name,
            int columns,
            Vector2 cellSize,
            float preferredHeight)
        {
            GameObject gridObject = new GameObject(name, typeof(RectTransform), typeof(GridLayoutGroup), typeof(LayoutElement));
            gridObject.transform.SetParent(parent, false);

            GridLayoutGroup grid = gridObject.GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.spacing = new Vector2(4f, 3f);
            grid.cellSize = cellSize;
            grid.childAlignment = TextAnchor.UpperLeft;

            LayoutElement layoutElement = gridObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            return gridObject.transform;
        }

        private Button CreateMadlibButton(
            Transform parent,
            string name,
            float fontSize,
            out TMP_Text buttonText)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = questionAnswerButtonColor;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(buttonObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5f, 2f);
            textRect.offsetMax = new Vector2(-5f, -2f);

            buttonText = textObject.GetComponent<TMP_Text>();
            buttonText.font = pageTitleText != null ? pageTitleText.font : buttonText.font;
            buttonText.fontSize = fontSize;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            buttonText.enableWordWrapping = false;
            buttonText.overflowMode = TextOverflowModes.Ellipsis;
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;

            return button;
        }

        private Button CreateQuestionButton(
            Transform parent,
            string name,
            out TMP_Text buttonText)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = questionAnswerButtonColor;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 27f;
            layoutElement.preferredHeight = 27f;

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(buttonObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(9f, 3f);
            textRect.offsetMax = new Vector2(-9f, -3f);

            buttonText = textObject.GetComponent<TMP_Text>();
            buttonText.font = pageTitleText != null ? pageTitleText.font : buttonText.font;
            buttonText.fontSize = 13f;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            buttonText.enableWordWrapping = true;
            buttonText.overflowMode = TextOverflowModes.Ellipsis;
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;

            return button;
        }

        private TMP_Text GetQuestionAnswerText(int index)
        {
            if (applicationQuestionAnswerButtonTexts != null
                && index >= 0
                && index < applicationQuestionAnswerButtonTexts.Length)
            {
                return applicationQuestionAnswerButtonTexts[index];
            }

            if (applicationQuestionAnswerButtons != null
                && index >= 0
                && index < applicationQuestionAnswerButtons.Length
                && applicationQuestionAnswerButtons[index] != null)
            {
                return applicationQuestionAnswerButtons[index].GetComponentInChildren<TMP_Text>(true);
            }

            return null;
        }

        private TMP_Text GetMadlibBlankText(int index)
        {
            if (applicationMadlibBlankButtonTexts != null
                && index >= 0
                && index < applicationMadlibBlankButtonTexts.Length)
            {
                return applicationMadlibBlankButtonTexts[index];
            }

            if (applicationMadlibBlankButtons != null
                && index >= 0
                && index < applicationMadlibBlankButtons.Length
                && applicationMadlibBlankButtons[index] != null)
            {
                return applicationMadlibBlankButtons[index].GetComponentInChildren<TMP_Text>(true);
            }

            return null;
        }

        private TMP_Text GetMadlibWordText(int index)
        {
            if (applicationMadlibWordButtonTexts != null
                && index >= 0
                && index < applicationMadlibWordButtonTexts.Length)
            {
                return applicationMadlibWordButtonTexts[index];
            }

            if (applicationMadlibWordButtons != null
                && index >= 0
                && index < applicationMadlibWordButtons.Length
                && applicationMadlibWordButtons[index] != null)
            {
                return applicationMadlibWordButtons[index].GetComponentInChildren<TMP_Text>(true);
            }

            return null;
        }

        private void CacheValidatedSessionTime()
        {
            lastValidatedSessionDurationSeconds = sessionDurationSeconds;
            lastValidatedSessionSecondsRemaining = sessionSecondsRemaining;
        }

        private static string Format(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
                return string.Empty;

            try
            {
                return string.Format(format, args);
            }
            catch (System.FormatException)
            {
                return format;
            }
        }

        private string PortalTitle => GetContentText(content?.PortalTitle, "TINY CORP CAREERS PORTAL");
        private string PortalSubtitle => GetContentText(content?.PortalSubtitle, "Application deadline: your 22nd birthday. Please complete all fields before government agents complete you.");
        private string FallbackPageTitle => GetContentText(content?.FallbackPageTitle, "Applicant Portal");
        private string UsernameLabel => GetContentText(content?.UsernameLabel, "Username");
        private string PasswordLabel => GetContentText(content?.PasswordLabel, "Password");
        private string LoginButtonLabel => GetContentText(content?.LoginButtonLabel, "Log In");
        private string TwoFactorTitle => GetContentText(content?.TwoFactorTitle, "Two-factor authentication");
        private string TwoFactorBody => GetContentText(content?.TwoFactorBody, "Your phone buzzes somewhere under the disaster drawer. For now, enter the temp code.");
        private string TwoFactorButtonLabel => GetContentText(content?.TwoFactorButtonLabel, "Verify");
        private string RefreshButtonLabel => GetContentText(content?.RefreshButtonLabel, "Refresh");
        private string NextButtonLabel => GetContentText(content?.NextButtonLabel, "Next >");
        private string ExpositionLetterTitle => GetContentText(content?.ExpositionLetterTitle, "Notice of Birthday Employment Assistance");
        private string ExpositionLetterBody => GetContentText(content?.ExpositionLetterBody, "Hello,\n\nHappy early birthday. As your 22nd birthday is coming up in 10 minutes, we're sure you're aware that all unemployed members of society will be executed by gunfire on their 22nd birthday.\n\nTo assist you on your search, we've found an entry-level role that even you might be able to take on.");
        private string ExpositionLetterButtonLabel => GetContentText(content?.ExpositionLetterButtonLabel, "See Job");
        private string JobListingTitle => GetContentText(content?.JobListingTitle, "Entry-Level Designer at Workbay Careers");
        private string JobListingDescription => GetContentText(content?.JobListingDescription, "Workbay Careers is seeking an entry-level designer to design clear, delightful, compliant things under fast-moving, birthday-adjacent deadlines.");
        private string JobListingMinimumQualificationsHeading => GetContentText(content?.JobListingMinimumQualificationsHeading, "Minimum Qualifications");
        private string JobListingMinimumQualificationsBody => GetContentText(content?.JobListingMinimumQualificationsBody, "7+ years of industry design experience.\nAbility to design things really, REALLY, well.");
        private string JobListingBenefitsHeading => GetContentText(content?.JobListingBenefitsHeading, "Our Benefits");
        private string JobListingBenefitsBody => GetContentText(content?.JobListingBenefitsBody, "A weekly banana.");
        private string JobListingApplyButtonLabel => GetContentText(content?.JobListingApplyButtonLabel, "Apply");
        private string JobListingOtherRolesButtonLabel => GetContentText(content?.JobListingOtherRolesButtonLabel, "Other Roles");
        private string JobListingChallengeId => GetContentText(content?.JobListingChallengeId, "job_listing");
        private string JobListingOtherRolesError => GetContentText(content?.JobListingOtherRolesError, "This is our only role.");
        private string JobListingRefreshQualificationSearchText => GetContentText(content?.JobListingRefreshQualificationSearchText, "7+");
        private string JobListingRefreshQualificationReplacementText => GetContentText(content?.JobListingRefreshQualificationReplacementText, "8+");
        private string JobListingRefreshBenefitSearchText => GetContentText(content?.JobListingRefreshBenefitSearchText, "weekly");
        private string JobListingRefreshBenefitReplacementText => GetContentText(content?.JobListingRefreshBenefitReplacementText, "monthly");
        private string QuestionsIntroText => GetContentText(content?.QuestionsIntroText, "answer these questions");
        private string QuestionsCompleteStatus => GetContentText(content?.QuestionsCompleteStatus, "all questions accepted");
        private string QuestionAnsweredStatusFormat => GetContentText(content?.QuestionAnsweredStatusFormat, "Answer accepted. {0} remaining.");
        private string WrongQuestionAnswerError => GetContentText(content?.WrongQuestionAnswerError, "That answer doesn't look right. Refresh to retry.");
        private string MadlibIntroText => GetContentText(content?.MadlibIntroText, "We want to know a little more about yourself in your own words. However, because of AI abuse, we're writing most of it for you.");
        private string MadlibCompleteStatus => GetContentText(content?.MadlibCompleteStatus, "Personal statement accepted.");
        private string ReviewPromptText => GetContentText(content?.ReviewPromptText, "To submit, we need to check if it's you.");
        private string ReviewPasswordLabel => GetContentText(content?.ReviewPasswordLabel, "Password");
        private string ReviewPasswordPlaceholder => GetContentText(content?.ReviewPasswordPlaceholder, "...");
        private string ReviewSubmitButtonLabel => GetContentText(content?.ReviewSubmitButtonLabel, "Verify Identity");
        private string ReviewPasswordChallengeId => GetContentText(content?.ReviewPasswordChallengeId, "review_password");
        private string ReviewWrongPasswordError => GetContentText(content?.ReviewWrongPasswordError, "Identity check failed. Refresh to retry.");
        private string ReviewCompleteStatus => GetContentText(content?.ReviewCompleteStatus, "Identity verified. You may submit your application.");
        private string SubmittedText => GetContentText(content?.SubmittedText, "submission complete.");
        private string UsernamePlaceholder => GetContentText(content?.UsernamePlaceholder, "try big.boss@outerheaven.com");
        private string PasswordPlaceholder => GetContentText(content?.PasswordPlaceholder, "try banana_protocol");
        private string TwoFactorPlaceholder => GetContentText(content?.TwoFactorPlaceholder, "try 0422");
        private string UsernameChallengeId => GetContentText(content?.UsernameChallengeId, DefaultUsernameChallengeId);
        private string PasswordChallengeId => GetContentText(content?.PasswordChallengeId, DefaultPasswordChallengeId);
        private string TwoFactorChallengeId => GetContentText(content?.TwoFactorChallengeId, DefaultTwoFactorChallengeId);
        private string CorrectUsername => GetContentText(
            CurrentApplicant?.Username,
            GetContentText(content?.CorrectUsername, DefaultCorrectUsername));
        private string CorrectPassword => GetContentText(
            CurrentApplicant?.Password,
            GetContentText(content?.CorrectPassword, DefaultCorrectPassword));
        private string CorrectTwoFactorCode => GetContentText(
            CurrentApplicant?.TwoFactorCode,
            GetContentText(content?.CorrectTwoFactorCode, DefaultCorrectTwoFactorCode));
        private string InitialStatus => GetContentText(content?.InitialStatus, "Log in to begin becoming professionally acceptable.");
        private string CredentialsAcceptedStatus => GetContentText(content?.CredentialsAcceptedStatus, "Credentials accepted. Two-factor authentication required, because of course it is.");
        private string SignInCompleteStatus => GetContentText(content?.SignInCompleteStatus, "Sign-in complete. You may proceed to My Information.");
        private string PageRefreshedStatus => GetContentText(content?.PageRefreshedStatus, "Page refreshed. Try to behave more employably this time.");
        private string SectionAdvancedStatus => GetContentText(content?.SectionAdvancedStatus, "Advanced to the next application section.");
        private string PageBlockedStatus => GetContentText(content?.PageBlockedStatus, "Page error. Refresh required.");
        private string DelicatePortalStatus => GetContentText(content?.DelicatePortalStatus, "The portal has entered a delicate emotional state.");
        private string SectionLoadedStatusFormat => GetContentText(content?.SectionLoadedStatusFormat, "Loaded {0}.");
        private string UnavailableStatus => GetContentText(content?.UnavailableStatus, "Applicant portal unavailable.");
        private string WrongUsernameError => GetContentText(content?.WrongUsernameError, "Username not recognized. Please refresh the page to continue.");
        private string WrongPasswordError => GetContentText(content?.WrongPasswordError, "Password rejected. Please refresh the page before trying again.");
        private string WrongTwoFactorError => GetContentText(content?.WrongTwoFactorError, "Authentication code invalid. Please refresh the page to request a new code.");
        private string ProgressFormat => GetContentText(content?.ProgressFormat, "{0}/{1} required items complete");
        private string NoActiveSectionProgress => GetContentText(content?.NoActiveSectionProgress, "No active application section.");
        private string RefreshCooldownFormat => GetContentText(content?.RefreshCooldownFormat, "Refresh ({0:0.0}s)");
        private string SessionTimerFormat => GetContentText(content?.SessionTimerFormat, "Session expires in {0:00}:{1:00}");
        private string SessionExpiredError => GetContentText(content?.SessionExpiredError, "Session expired. Please complete two-factor authentication before refreshing.");
        private string SessionExpiredTitle => GetContentText(content?.SessionExpiredTitle, "Session expired, 2FA Required");
        private string SessionExpiredBody => GetContentText(content?.SessionExpiredBody, "For your security, Workbay has forgotten who you are. Enter the authentication code to unlock refresh.");
        private string SessionReauthPlaceholder => GetContentText(content?.SessionReauthPlaceholder, "2FA code");
        private string SessionReauthSubmitButtonLabel => GetContentText(content?.SessionReauthSubmitButtonLabel, "Verify");
        private string SessionReauthWrongCodeError => GetContentText(content?.SessionReauthWrongCodeError, "Incorrect code. Please try being the correct applicant.");
        private string SessionReauthSuccessStatus => GetContentText(content?.SessionReauthSuccessStatus, "Two-factor authentication accepted. You may refresh the expired page.");
        private string MyExperienceIntroText => GetContentText(experienceContent?.IntroText, "Please upload your resume. Any resume. Ideally the correct one.");
        private string UploadResumeButtonLabel => GetContentText(experienceContent?.UploadResumeButtonLabel, "Upload Resume");
        private string MyExperienceInitialStatus => GetContentText(experienceContent?.IntroText, "Please upload your resume. Any resume. Ideally the correct one.");
        private string MyExperienceCompleteStatus => GetContentText(experienceContent?.CompleteStatus, "Resume uploaded. Your experience has been accepted, pending inevitable disappointment.");
        private string ResumePickerTitle => GetContentText(experienceContent?.PickerTitle, "Choose Resume");
        private string ResumePickerPath => GetContentText(experienceContent?.PickerPath, "Macintosh HD > Users > Applicant > Desktop > resume graveyard");
        private string ResumePickerOpenButtonLabel => GetContentText(experienceContent?.OpenButtonLabel, "Open");
        private string ResumePickerSelectButtonLabel => GetContentText(experienceContent?.SelectButtonLabel, "Select");
        private string ResumePickerCancelButtonLabel => GetContentText(experienceContent?.CancelButtonLabel, "Cancel");
        private string WordActivationError => GetContentText(experienceContent?.WordActivationError, "You need to activate Mycosoft Word. Cannot open.");
        private string IncorrectResumeError => GetContentText(experienceContent?.IncorrectResumeError, "Your experience is rough. It looks like you might be unemployed for a while.");
        private string ResumeChallengeId => GetContentText(experienceContent?.ResumeChallengeId, "resume_upload");
        private int CorrectResumeIndex => ResumeFileCount <= 0 ? -1 : Mathf.Clamp(experienceContent != null ? experienceContent.CorrectResumeIndex : 5, 0, ResumeFileCount - 1);
        private int ResumeFileCount
        {
            get
            {
                IReadOnlyList<string> fileNames = experienceContent?.ResumeFileNames;
                return fileNames != null && fileNames.Count > 0 ? fileNames.Count : DefaultResumeFileNames.Length;
            }
        }

        private static string GetContentText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private string ResumeFileName(int index)
        {
            IReadOnlyList<string> fileNames = experienceContent?.ResumeFileNames;
            if (fileNames != null && index >= 0 && index < fileNames.Count)
                return GetContentText(fileNames[index], DefaultResumeFileNames[Mathf.Clamp(index, 0, DefaultResumeFileNames.Length - 1)]);

            if (index >= 0 && index < DefaultResumeFileNames.Length)
                return DefaultResumeFileNames[index];

            return "resume-unknown.doc";
        }

        private string GetJobListingMinimumQualificationsBody(bool refreshed)
        {
            if (!refreshed)
                return JobListingMinimumQualificationsBody;

            return ReplaceFirst(
                JobListingMinimumQualificationsBody,
                JobListingRefreshQualificationSearchText,
                JobListingRefreshQualificationReplacementText);
        }

        private string GetJobListingBenefitsBody(bool refreshed)
        {
            if (!refreshed)
                return JobListingBenefitsBody;

            return ReplaceFirst(
                JobListingBenefitsBody,
                JobListingRefreshBenefitSearchText,
                JobListingRefreshBenefitReplacementText);
        }

        private static string ReplaceFirst(string text, string search, string replacement)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(search))
                return text;

            int index = text.IndexOf(search, System.StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return text;

            return text.Remove(index, search.Length).Insert(index, replacement ?? string.Empty);
        }

        private string CurrentDateOfBirthFormat
        {
            get
            {
                ApplicationSectionRuntimeState section = applicationState != null ? applicationState.CurrentSection : null;
                int refreshCount = section != null ? section.RefreshCount : 0;
                IReadOnlyList<string> formats = myInformationContent?.DateOfBirthFormats;

                if (formats == null || formats.Count == 0)
                    return "MM/YYYY/DD";

                return GetContentText(formats[Mathf.Abs(refreshCount) % formats.Count], "MM/YYYY/DD");
            }
        }

        //Simplifying the Date format for now, incredibly easy to change back if necessary
        private string BuildExpectedDateOfBirth(string format)
        {
            string month = CorrectBirthMonth.ToString("00");
            string day = CorrectBirthDay.ToString("00");
            string year = CorrectBirthYear.ToString("0000");
            string shortYear = Mathf.Abs(CorrectBirthYear % 100).ToString("00");

            switch (format)
            {
                case "DD/YY/MM":
                    return $"{day}/{shortYear}/{month}";
                default:
                    return $"{month}/{year}/{day}";
            }
        }

        private static bool EqualsAuthoringAnswer(string actual, string expected)
        {
            return string.Equals(
                actual?.Trim(),
                expected?.Trim(),
                System.StringComparison.OrdinalIgnoreCase);
        }

        private string MyInformationIntroText => GetContentText(myInformationContent?.IntroText, "Confirm your legal identity exactly as it appears in The File.");
        private string FirstNameLabel => GetContentText(myInformationContent?.FirstNameLabel, "First name");
        private string LastNameLabel => GetContentText(myInformationContent?.LastNameLabel, "Last name");
        private string ConfirmNameButtonLabel => GetContentText(myInformationContent?.ConfirmNameButtonLabel, "Confirm Name");
        private string DateOfBirthLabelFormat => GetContentText(myInformationContent?.DateOfBirthLabelFormat, "Date of birth ({0})");
        private string DateOfBirthHintText => GetContentText(myInformationContent?.DateOfBirthHintText, "The required date format changes after every refresh.");
        private string ConfirmDateOfBirthButtonLabel => GetContentText(myInformationContent?.ConfirmDateOfBirthButtonLabel, "Confirm Birth Date");
        private string FirstNamePlaceholder => GetContentText(myInformationContent?.FirstNamePlaceholder, "...");
        private string LastNamePlaceholder => GetContentText(myInformationContent?.LastNamePlaceholder, "...");
        private string DateOfBirthPlaceholder => GetContentText(myInformationContent?.DateOfBirthPlaceholder, "...");
        private string FirstNameChallengeId => GetContentText(myInformationContent?.FirstNameChallengeId, "first_name");
        private string LastNameChallengeId => GetContentText(myInformationContent?.LastNameChallengeId, "last_name");
        private string DateOfBirthChallengeId => GetContentText(myInformationContent?.DateOfBirthChallengeId, "date_of_birth");
        private string CorrectFirstName => GetContentText(
            CurrentApplicant?.FirstName,
            GetContentText(myInformationContent?.CorrectFirstName, "Bartholomew"));
        private string CorrectLastName => GetContentText(
            CurrentApplicant?.LastName,
            GetContentText(myInformationContent?.CorrectLastName, "Huang"));
        private int CorrectBirthMonth => myInformationContent != null ? myInformationContent.CorrectBirthMonth : 4;
        private int CorrectBirthDay => myInformationContent != null ? myInformationContent.CorrectBirthDay : 22;
        private int CorrectBirthYear => myInformationContent != null ? myInformationContent.CorrectBirthYear : 2004;
        private string MyInformationInitialStatus => GetContentText(myInformationContent?.InitialStatus, "Enter your personal information. It already knows, but it wants to watch.");
        private string NamesAcceptedStatus => GetContentText(myInformationContent?.NamesAcceptedStatus, "Name confirmed. The File reluctantly agrees you exist.");
        private string MyInformationCompleteStatus => GetContentText(myInformationContent?.CompleteStatus, "Personal information confirmed. Proceed before the format changes again somehow.");
        private string IdentityMismatchError => GetContentText(myInformationContent?.IdentityMismatchError, "These details do not match what we have on file. Please refresh the page to continue.");
        private string DateOfBirthMismatchError => GetContentText(myInformationContent?.DateOfBirthMismatchError, "This birth date does not match what we have on file. Please refresh the page to continue.");
    }
}
