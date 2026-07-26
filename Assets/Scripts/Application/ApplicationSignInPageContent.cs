using UnityEngine;

namespace BirthdayJobJam.Application
{
    [CreateAssetMenu(fileName = "SignInPageContent_", menuName = "Birthday Job Jam/Application/Sign In Page Content")]
    public sealed class ApplicationSignInPageContent : ScriptableObject
    {
        [Header("Portal Chrome")]
        [SerializeField] private string portalTitle = "TINY CORP CAREERS PORTAL";
        [SerializeField] private string portalSubtitle = "Application deadline: your 22nd birthday. Please complete all fields before government agents complete you.";
        [SerializeField] private string fallbackPageTitle = "Applicant Portal";

        [Header("Labels")]
        [SerializeField] private string usernameLabel = "Username";
        [SerializeField] private string passwordLabel = "Password";
        [SerializeField] private string loginButtonLabel = "Log In";
        [SerializeField] private string twoFactorTitle = "Two-factor authentication";
        [SerializeField] private string twoFactorBody = "Your phone buzzes somewhere under the disaster drawer. For now, enter the temp code.";
        [SerializeField] private string twoFactorButtonLabel = "Verify";
        [SerializeField] private string refreshButtonLabel = "Refresh";
        [SerializeField] private string nextButtonLabel = "Next >";

        [Header("Exposition Letter")]
        [SerializeField] private string expositionLetterTitle = "Notice of Birthday Employment Assistance";
        [TextArea] [SerializeField] private string expositionLetterBody = "Hello,\n\nHappy early birthday. As your 22nd birthday is coming up in 10 minutes, we're sure you're aware that all unemployed members of society will be executed by gunfire on their 22nd birthday.\n\nTo assist you on your search, we've found an entry-level role that even you might be able to take on.";
        [SerializeField] private string expositionLetterButtonLabel = "See Job";

        [Header("Job Listing")]
        [SerializeField] private string jobListingTitle = "Entry-Level Designer at Workbay Careers";
        [TextArea] [SerializeField] private string jobListingDescription = "Workbay Careers is seeking an entry-level designer to design clear, delightful, compliant things under fast-moving, birthday-adjacent deadlines.";
        [SerializeField] private string jobListingMinimumQualificationsHeading = "Minimum Qualifications";
        [TextArea] [SerializeField] private string jobListingMinimumQualificationsBody = "7+ years of industry design experience.\nAbility to design things really, REALLY, well.";
        [SerializeField] private string jobListingBenefitsHeading = "Our Benefits";
        [TextArea] [SerializeField] private string jobListingBenefitsBody = "A weekly banana.";
        [SerializeField] private string jobListingApplyButtonLabel = "Apply";
        [SerializeField] private string jobListingOtherRolesButtonLabel = "Other Roles";
        [SerializeField] private string jobListingChallengeId = "job_listing";
        [TextArea] [SerializeField] private string jobListingOtherRolesError = "This is our only role.";
        [SerializeField] private string jobListingRefreshQualificationSearchText = "7+";
        [SerializeField] private string jobListingRefreshQualificationReplacementText = "8+";
        [SerializeField] private string jobListingRefreshBenefitSearchText = "weekly";
        [SerializeField] private string jobListingRefreshBenefitReplacementText = "monthly";

        [Header("Application Questions")]
        [SerializeField] private string questionsIntroText = "answer these questions";
        [SerializeField] private string questionsCompleteStatus = "all questions accepted";
        [SerializeField] private string questionAnsweredStatusFormat = "Answer accepted. {0} remaining.";
        [TextArea] [SerializeField] private string wrongQuestionAnswerError = "That answer doesn't look right. Refresh to retry.";
        [TextArea] [SerializeField] private string madlibIntroText = "We want to know a little more about yourself in your own words. However, because of AI abuse, we're writing most of it for you.";
        [SerializeField] private string madlibCompleteStatus = "Personal statement accepted.";

        [Header("Review")]
        [TextArea] [SerializeField] private string reviewPromptText = "To submit, we need to check if it's you.";
        [SerializeField] private string reviewPasswordLabel = "Password";
        [SerializeField] private string reviewPasswordPlaceholder = "...";
        [SerializeField] private string reviewSubmitButtonLabel = "Verify Identity";
        [SerializeField] private string reviewPasswordChallengeId = "review_password";
        [TextArea] [SerializeField] private string reviewWrongPasswordError = "Identity check failed. Refresh to retry.";
        [TextArea] [SerializeField] private string reviewCompleteStatus = "Identity verified. You may submit your application.";
        [TextArea] [SerializeField] private string submittedText = "submission complete.";

        [Header("Placeholders")]
        [SerializeField] private string usernamePlaceholder = "try big.boss@outerheaven.com";
        [SerializeField] private string passwordPlaceholder = "try banana_protocol";
        [SerializeField] private string twoFactorPlaceholder = "try 0422";

        [Header("Challenge Ids")]
        [SerializeField] private string usernameChallengeId = "username";
        [SerializeField] private string passwordChallengeId = "password";
        [SerializeField] private string twoFactorChallengeId = "two_factor_code";

        [Header("Temporary Correct Answers")]
        [SerializeField] private string correctUsername = "big.boss@outerheaven.com";
        [SerializeField] private string correctPassword = "banana_protocol";
        [SerializeField] private string correctTwoFactorCode = "0422";

        [Header("Status Copy")]
        [TextArea] [SerializeField] private string initialStatus = "Log in to begin becoming professionally acceptable.";
        [TextArea] [SerializeField] private string credentialsAcceptedStatus = "Credentials accepted. Two-factor authentication required, because of course it is.";
        [TextArea] [SerializeField] private string signInCompleteStatus = "Sign-in complete. You may proceed to My Information.";
        [TextArea] [SerializeField] private string pageRefreshedStatus = "Page refreshed. Try to behave more employably this time.";
        [TextArea] [SerializeField] private string sectionAdvancedStatus = "Advanced to the next application section.";
        [TextArea] [SerializeField] private string pageBlockedStatus = "Page error. Refresh required.";
        [TextArea] [SerializeField] private string delicatePortalStatus = "The portal has entered a delicate emotional state.";
        [TextArea] [SerializeField] private string sectionLoadedStatusFormat = "Loaded {0}.";
        [TextArea] [SerializeField] private string unavailableStatus = "Applicant portal unavailable.";

        [Header("Error Copy")]
        [TextArea] [SerializeField] private string wrongUsernameError = "Username not recognized. Please refresh the page to continue.";
        [TextArea] [SerializeField] private string wrongPasswordError = "Password rejected. Please refresh the page before trying again.";
        [TextArea] [SerializeField] private string wrongTwoFactorError = "Authentication code invalid. Please refresh the page to request a new code.";

        [Header("Formatting")]
        [SerializeField] private string progressFormat = "{0}/{1} required items complete";
        [SerializeField] private string noActiveSectionProgress = "No active application section.";
        [SerializeField] private string refreshCooldownFormat = "Refresh ({0:0.0}s)";

        [Header("Session Expiry")]
        [SerializeField] private string sessionTimerFormat = "Session expires in {0:00}:{1:00}";
        [TextArea] [SerializeField] private string sessionExpiredError = "Session expired. Please complete two-factor authentication before refreshing.";
        [SerializeField] private string sessionExpiredTitle = "Session expired, 2FA Required";
        [TextArea] [SerializeField] private string sessionExpiredBody = "For your security, Workbay has forgotten who you are. Enter the authentication code to unlock refresh.";
        [SerializeField] private string sessionReauthPlaceholder = "2FA code";
        [SerializeField] private string sessionReauthSubmitButtonLabel = "Verify";
        [TextArea] [SerializeField] private string sessionReauthWrongCodeError = "Incorrect code.";
        [TextArea] [SerializeField] private string sessionReauthSuccessStatus = "Two-factor authentication accepted. You may refresh the expired page.";

        public string PortalTitle => portalTitle;
        public string PortalSubtitle => portalSubtitle;
        public string FallbackPageTitle => fallbackPageTitle;
        public string UsernameLabel => usernameLabel;
        public string PasswordLabel => passwordLabel;
        public string LoginButtonLabel => loginButtonLabel;
        public string TwoFactorTitle => twoFactorTitle;
        public string TwoFactorBody => twoFactorBody;
        public string TwoFactorButtonLabel => twoFactorButtonLabel;
        public string RefreshButtonLabel => refreshButtonLabel;
        public string NextButtonLabel => nextButtonLabel;
        public string ExpositionLetterTitle => expositionLetterTitle;
        public string ExpositionLetterBody => expositionLetterBody;
        public string ExpositionLetterButtonLabel => expositionLetterButtonLabel;
        public string JobListingTitle => jobListingTitle;
        public string JobListingDescription => jobListingDescription;
        public string JobListingMinimumQualificationsHeading => jobListingMinimumQualificationsHeading;
        public string JobListingMinimumQualificationsBody => jobListingMinimumQualificationsBody;
        public string JobListingBenefitsHeading => jobListingBenefitsHeading;
        public string JobListingBenefitsBody => jobListingBenefitsBody;
        public string JobListingApplyButtonLabel => jobListingApplyButtonLabel;
        public string JobListingOtherRolesButtonLabel => jobListingOtherRolesButtonLabel;
        public string JobListingChallengeId => jobListingChallengeId;
        public string JobListingOtherRolesError => jobListingOtherRolesError;
        public string JobListingRefreshQualificationSearchText => jobListingRefreshQualificationSearchText;
        public string JobListingRefreshQualificationReplacementText => jobListingRefreshQualificationReplacementText;
        public string JobListingRefreshBenefitSearchText => jobListingRefreshBenefitSearchText;
        public string JobListingRefreshBenefitReplacementText => jobListingRefreshBenefitReplacementText;
        public string QuestionsIntroText => questionsIntroText;
        public string QuestionsCompleteStatus => questionsCompleteStatus;
        public string QuestionAnsweredStatusFormat => questionAnsweredStatusFormat;
        public string WrongQuestionAnswerError => wrongQuestionAnswerError;
        public string MadlibIntroText => madlibIntroText;
        public string MadlibCompleteStatus => madlibCompleteStatus;
        public string ReviewPromptText => reviewPromptText;
        public string ReviewPasswordLabel => reviewPasswordLabel;
        public string ReviewPasswordPlaceholder => reviewPasswordPlaceholder;
        public string ReviewSubmitButtonLabel => reviewSubmitButtonLabel;
        public string ReviewPasswordChallengeId => reviewPasswordChallengeId;
        public string ReviewWrongPasswordError => reviewWrongPasswordError;
        public string ReviewCompleteStatus => reviewCompleteStatus;
        public string SubmittedText => submittedText;
        public string UsernamePlaceholder => usernamePlaceholder;
        public string PasswordPlaceholder => passwordPlaceholder;
        public string TwoFactorPlaceholder => twoFactorPlaceholder;
        public string UsernameChallengeId => usernameChallengeId;
        public string PasswordChallengeId => passwordChallengeId;
        public string TwoFactorChallengeId => twoFactorChallengeId;
        public string CorrectUsername => correctUsername;
        public string CorrectPassword => correctPassword;
        public string CorrectTwoFactorCode => correctTwoFactorCode;
        public string InitialStatus => initialStatus;
        public string CredentialsAcceptedStatus => credentialsAcceptedStatus;
        public string SignInCompleteStatus => signInCompleteStatus;
        public string PageRefreshedStatus => pageRefreshedStatus;
        public string SectionAdvancedStatus => sectionAdvancedStatus;
        public string PageBlockedStatus => pageBlockedStatus;
        public string DelicatePortalStatus => delicatePortalStatus;
        public string SectionLoadedStatusFormat => sectionLoadedStatusFormat;
        public string UnavailableStatus => unavailableStatus;
        public string WrongUsernameError => wrongUsernameError;
        public string WrongPasswordError => wrongPasswordError;
        public string WrongTwoFactorError => wrongTwoFactorError;
        public string ProgressFormat => progressFormat;
        public string NoActiveSectionProgress => noActiveSectionProgress;
        public string RefreshCooldownFormat => refreshCooldownFormat;
        public string SessionTimerFormat => sessionTimerFormat;
        public string SessionExpiredError => sessionExpiredError;
        public string SessionExpiredTitle => sessionExpiredTitle;
        public string SessionExpiredBody => sessionExpiredBody;
        public string SessionReauthPlaceholder => sessionReauthPlaceholder;
        public string SessionReauthSubmitButtonLabel => sessionReauthSubmitButtonLabel;
        public string SessionReauthWrongCodeError => sessionReauthWrongCodeError;
        public string SessionReauthSuccessStatus => sessionReauthSuccessStatus;
    }
}
