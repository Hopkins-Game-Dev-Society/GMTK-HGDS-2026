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

        [Header("Placeholders")]
        [SerializeField] private string usernamePlaceholder = "try applicant22";
        [SerializeField] private string passwordPlaceholder = "try birthday123";
        [SerializeField] private string twoFactorPlaceholder = "try 0422";

        [Header("Challenge Ids")]
        [SerializeField] private string usernameChallengeId = "username";
        [SerializeField] private string passwordChallengeId = "password";
        [SerializeField] private string twoFactorChallengeId = "two_factor_code";

        [Header("Temporary Correct Answers")]
        [SerializeField] private string correctUsername = "applicant22";
        [SerializeField] private string correctPassword = "birthday123";
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
    }
}
