using BirthdayJobJam.Core;
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

        [Header("Model")]
        [SerializeField] private ApplicationStateModel applicationState;
        [SerializeField] private ApplicationSignInPageContent content;

        [Header("Portal Chrome")]
        [SerializeField] private Text portalTitleText;
        [SerializeField] private Text portalSubtitleText;
        [SerializeField] private Text[] progressStepLabelTexts;

        [Header("Page Text")]
        [SerializeField] private Text pageTitleText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text statusText;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private Text errorText;

        [Header("Login Controls")]
        [SerializeField] private Text usernameLabelText;
        [SerializeField] private InputField usernameInput;
        [SerializeField] private Text passwordLabelText;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Text loginButtonText;

        [Header("Two-Factor Controls")]
        [SerializeField] private GameObject twoFactorGroup;
        [SerializeField] private Text twoFactorTitleText;
        [SerializeField] private Text twoFactorBodyText;
        [SerializeField] private InputField twoFactorInput;
        [SerializeField] private Button twoFactorButton;
        [SerializeField] private Text twoFactorButtonText;

        [Header("Navigation")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Text refreshButtonText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Text nextButtonText;

        private void Awake()
        {
            ResolveApplicationState();
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

        public void RefreshPage()
        {
            if (applicationState == null)
                return;

            if (!applicationState.TryRefreshCurrentSection())
                return;

            ClearInputs();
            SetStatus(PageRefreshedStatus);
            Render();
        }

        public void NextPage()
        {
            if (applicationState == null)
                return;

            if (applicationState.TryAdvanceSection())
                SetStatus(SectionAdvancedStatus);

            Render();
        }

        private void Render()
        {
            if (applicationState == null)
                return;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            bool isSignIn = section != null && section.SectionId == ApplicationSectionId.CreateAccountSignIn;
            bool blocked = section != null && section.IsBlocked;
            bool credentialsComplete = IsChallengeComplete(section, UsernameChallengeId) && IsChallengeComplete(section, PasswordChallengeId);
            bool signInComplete = section != null && section.IsComplete;

            SetText(pageTitleText, section != null ? section.DisplayName : FallbackPageTitle);
            SetText(progressText, BuildProgressText(section));
            RenderProgressStepper();

            if (errorPanel != null)
                errorPanel.SetActive(blocked);

            if (errorText != null)
            {
                errorText.gameObject.SetActive(blocked);
                errorText.text = blocked ? section.ErrorMessage : string.Empty;
            }

            if (usernameInput != null)
                usernameInput.interactable = isSignIn && !blocked && !credentialsComplete;

            if (passwordInput != null)
                passwordInput.interactable = isSignIn && !blocked && !credentialsComplete;

            if (loginButton != null)
                loginButton.interactable = isSignIn && !blocked && !credentialsComplete;

            if (twoFactorGroup != null)
                twoFactorGroup.SetActive(isSignIn && credentialsComplete);

            if (twoFactorInput != null)
                twoFactorInput.interactable = isSignIn && !blocked && credentialsComplete && !signInComplete;

            if (twoFactorButton != null)
                twoFactorButton.interactable = isSignIn && !blocked && credentialsComplete && !signInComplete;

            if (nextButton != null)
                nextButton.interactable = applicationState.CanAdvanceCurrentSection;

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

        private bool IsOnSignInPageAndInteractive()
        {
            if (applicationState == null)
                return false;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            return section != null
                && section.SectionId == ApplicationSectionId.CreateAccountSignIn
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
            SetText(refreshButtonText, RefreshButtonLabel);
            SetText(nextButtonText, NextButtonLabel);
            SetInputPlaceholder(usernameInput, UsernamePlaceholder);
            SetInputPlaceholder(passwordInput, PasswordPlaceholder);
            SetInputPlaceholder(twoFactorInput, TwoFactorPlaceholder);

            if (statusText != null && string.IsNullOrWhiteSpace(statusText.text))
                SetStatus(InitialStatus);
        }

        private void RenderProgressStepper()
        {
            if (applicationState == null || progressStepLabelTexts == null)
                return;

            int count = Mathf.Min(progressStepLabelTexts.Length, applicationState.Sections.Count);
            for (int i = 0; i < count; i++)
                SetText(progressStepLabelTexts[i], applicationState.Sections[i].ProgressLabel);
        }

        private void ClearInputs()
        {
            if (usernameInput != null)
                usernameInput.text = string.Empty;

            if (passwordInput != null)
                passwordInput.text = string.Empty;

            if (twoFactorInput != null)
                twoFactorInput.text = string.Empty;
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

            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshPage);

            if (nextButton != null)
                nextButton.onClick.AddListener(NextPage);
        }

        private void RemoveButtonListeners()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(SubmitLogin);

            if (twoFactorButton != null)
                twoFactorButton.onClick.RemoveListener(SubmitTwoFactorCode);

            if (refreshButton != null)
                refreshButton.onClick.RemoveListener(RefreshPage);

            if (nextButton != null)
                nextButton.onClick.RemoveListener(NextPage);
        }

        private void HandleSectionChanged(ApplicationSectionRuntimeState section)
        {
            SetStatus(section != null
                ? Format(SectionLoadedStatusFormat, section.DisplayName)
                : UnavailableStatus);

            Render();
        }

        private void HandlePageBlocked(ApplicationSectionRuntimeState section)
        {
            SetStatus(PageBlockedStatus);
            Render();
        }

        private void HandlePageRefreshed(ApplicationSectionRuntimeState section)
        {
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

        private static void SetText(Text target, string value)
        {
            if (target != null)
                target.text = value;
        }

        private static void SetInputPlaceholder(InputField input, string value)
        {
            if (input == null || input.placeholder == null)
                return;

            Text placeholder = input.placeholder.GetComponent<Text>();
            if (placeholder != null)
                placeholder.text = value;
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

        private static string GetContentText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
