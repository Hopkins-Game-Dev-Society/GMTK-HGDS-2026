using BirthdayJobJam.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationSignInPageView : MonoBehaviour
    {
        private const string CorrectUsername = "applicant22";
        private const string CorrectPassword = "birthday123";
        private const string CorrectTwoFactorCode = "0422";

        [Header("Model")]
        [SerializeField] private ApplicationStateModel applicationState;

        [Header("Page Text")]
        [SerializeField] private Text pageTitleText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text errorText;

        [Header("Login Controls")]
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;

        [Header("Two-Factor Controls")]
        [SerializeField] private GameObject twoFactorGroup;
        [SerializeField] private InputField twoFactorInput;
        [SerializeField] private Button twoFactorButton;

        [Header("Navigation")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Text refreshButtonText;
        [SerializeField] private Button nextButton;

        private void Awake()
        {
            ResolveApplicationState();
        }

        private void OnEnable()
        {
            ResolveApplicationState();
            Subscribe();
            AddButtonListeners();
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
                Fail("username", "Username not recognized. Please refresh the page to continue.");
                return;
            }

            if (password != CorrectPassword)
            {
                Fail("password", "Password rejected. Please refresh the page before trying again.");
                return;
            }

            applicationState.MarkChallengeComplete("username");
            applicationState.MarkChallengeComplete("password");
            SetStatus("Credentials accepted. Two-factor authentication required, because of course it is.");
            Render();
        }

        public void SubmitTwoFactorCode()
        {
            if (!IsOnSignInPageAndInteractive())
                return;

            string code = twoFactorInput != null ? twoFactorInput.text.Trim() : string.Empty;

            if (code != CorrectTwoFactorCode)
            {
                Fail("two_factor_code", "Authentication code invalid. Please refresh the page to request a new code.");
                return;
            }

            applicationState.MarkChallengeComplete("two_factor_code");
            SetStatus("Sign-in complete. You may proceed to My Information.");
            Render();
        }

        public void RefreshPage()
        {
            if (applicationState == null)
                return;

            if (!applicationState.TryRefreshCurrentSection())
                return;

            ClearInputs();
            SetStatus("Page refreshed. Try to behave more employably this time.");
            Render();
        }

        public void NextPage()
        {
            if (applicationState == null)
                return;

            if (applicationState.TryAdvanceSection())
                SetStatus("Advanced to the next application section.");

            Render();
        }

        private void Render()
        {
            if (applicationState == null)
                return;

            ApplicationSectionRuntimeState section = applicationState.CurrentSection;
            bool isSignIn = section != null && section.SectionId == ApplicationSectionId.CreateAccountSignIn;
            bool blocked = section != null && section.IsBlocked;
            bool credentialsComplete = IsChallengeComplete(section, "username") && IsChallengeComplete(section, "password");
            bool signInComplete = section != null && section.IsComplete;

            SetText(pageTitleText, section != null ? section.DisplayName : "Applicant Portal");
            SetText(progressText, BuildProgressText(section));

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
                    refreshButtonText.text = "Refresh";
                else if (cooldown > 0f)
                    refreshButtonText.text = $"Refresh ({cooldown:0.0}s)";
                else
                    refreshButtonText.text = "Refresh";
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

            SetStatus("The portal has entered a delicate emotional state.");
            Render();
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
                ? $"Loaded {section.DisplayName}."
                : "Applicant portal unavailable.");

            Render();
        }

        private void HandlePageBlocked(ApplicationSectionRuntimeState section)
        {
            SetStatus("Page error. Refresh required.");
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

        private static string BuildProgressText(ApplicationSectionRuntimeState section)
        {
            if (section == null)
                return "No active application section.";

            return $"{section.CompletedRequiredChallengeCount}/{section.RequiredChallengeCount} required items complete";
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
                target.text = value;
        }
    }
}
