using System.Collections.Generic;
using BirthdayJobJam.Core;
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
        private const string DefaultCorrectUsername = "applicant22";
        private const string DefaultCorrectPassword = "birthday123";
        private const string DefaultCorrectTwoFactorCode = "0422";
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

        private bool hasStartedApplication;
        private int selectedResumeIndex = -1;
        private float lastValidatedSessionDurationSeconds;
        private float lastValidatedSessionSecondsRemaining;

        private void Awake()
        {
            ResolveApplicationState();
            ResetSessionTimer();
            CacheValidatedSessionTime();
        }

        private void OnValidate()
        {
            sessionDurationSeconds = Mathf.Max(1f, sessionDurationSeconds);
            sessionSecondsRemaining = Mathf.Clamp(sessionSecondsRemaining, 0f, sessionDurationSeconds);

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
            Subscribe();
            AddButtonListeners();
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
            bool blocked = section != null && section.IsBlocked;
            bool credentialsComplete = IsChallengeComplete(section, UsernameChallengeId) && IsChallengeComplete(section, PasswordChallengeId);
            bool signInComplete = section != null && section.IsComplete;
            bool namesComplete = IsChallengeComplete(section, FirstNameChallengeId) && IsChallengeComplete(section, LastNameChallengeId);
            bool myInformationComplete = section != null && section.IsComplete;
            bool myExperienceComplete = IsChallengeComplete(section, ResumeChallengeId);

            if (!hasStartedApplication)
            {
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
            RenderSessionTimer();
            RenderSessionReauthentication();

            SetInteractable(nextButton, applicationState.CanAdvanceCurrentSection);
            SetText(nextButtonText, NextButtonLabel);
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
                && section.SectionId >= ApplicationSectionId.MyInformation;
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
            SetText(jobListingMinimumQualificationsBodyText, GetJobListingMinimumQualificationsBody(refreshed));
            SetText(jobListingBenefitsBodyText, GetJobListingBenefitsBody(refreshed));
            SetActive(progressText, false);
            SetActive(progressStepper, false);
            SetActive(signInFormPanel, false);
            SetActive(twoFactorGroup, false);
            SetActive(myInformationPanel, false);
            SetActive(myExperiencePanel, false);
            SetActive(resumePickerPanel, false);
            SetActive(sessionTimerText, false);
            SetActive(sessionExpiredReauthPanel, false);
            SetActive(jobListingPanel, true);
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

            selectedResumeIndex = -1;
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
        private string UsernamePlaceholder => GetContentText(content?.UsernamePlaceholder, "try applicant22");
        private string PasswordPlaceholder => GetContentText(content?.PasswordPlaceholder, "try birthday123");
        private string TwoFactorPlaceholder => GetContentText(content?.TwoFactorPlaceholder, "try 0422");
        private string UsernameChallengeId => GetContentText(content?.UsernameChallengeId, DefaultUsernameChallengeId);
        private string PasswordChallengeId => GetContentText(content?.PasswordChallengeId, DefaultPasswordChallengeId);
        private string TwoFactorChallengeId => GetContentText(content?.TwoFactorChallengeId, DefaultTwoFactorChallengeId);
        private string CorrectUsername => GetContentText(content?.CorrectUsername, DefaultCorrectUsername);
        private string CorrectPassword => GetContentText(content?.CorrectPassword, DefaultCorrectPassword);
        private string CorrectTwoFactorCode => GetContentText(content?.CorrectTwoFactorCode, DefaultCorrectTwoFactorCode);
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

        private string BuildExpectedDateOfBirth(string format)
        {
            string month = CorrectBirthMonth.ToString("00");
            string day = CorrectBirthDay.ToString("00");
            string year = CorrectBirthYear.ToString("0000");
            string shortYear = Mathf.Abs(CorrectBirthYear % 100).ToString("00");

            switch (format)
            {
                case "MM/YYYY/DD":
                    return $"{month}/{year}/{day}";
                case "YYYY/DD/MM":
                    return $"{year}/{day}/{month}";
                case "DD/YYYY/MM":
                    return $"{day}/{year}/{month}";
                case "MM/YY/DD":
                    return $"{month}/{shortYear}/{day}";
                case "YY/DD/MM":
                    return $"{shortYear}/{day}/{month}";
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
        private string CorrectFirstName => GetContentText(myInformationContent?.CorrectFirstName, "Jamie");
        private string CorrectLastName => GetContentText(myInformationContent?.CorrectLastName, "Applicant");
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
