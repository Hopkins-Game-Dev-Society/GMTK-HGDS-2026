using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [CreateAssetMenu(fileName = "MyInformationPageContent_", menuName = "Birthday Job Jam/Application/My Information Page Content")]
    public sealed class ApplicationMyInformationPageContent : ScriptableObject
    {
        [Header("Labels")]
        [SerializeField] private string introText = "Confirm your legal identity exactly as it appears in The File.";
        [SerializeField] private string firstNameLabel = "First name";
        [SerializeField] private string lastNameLabel = "Last name";
        [SerializeField] private string confirmNameButtonLabel = "Confirm Name";
        [SerializeField] private string dateOfBirthLabelFormat = "Date of birth ({0})";
        [TextArea] [SerializeField] private string dateOfBirthHintText = "The required date format changes after every refresh.";
        [SerializeField] private string confirmDateOfBirthButtonLabel = "Confirm Birth Date";

        [Header("Placeholders")]
        [SerializeField] private string firstNamePlaceholder = "...";
        [SerializeField] private string lastNamePlaceholder = "...";
        [SerializeField] private string dateOfBirthPlaceholder = "...";

        [Header("Challenge Ids")]
        [SerializeField] private string firstNameChallengeId = "first_name";
        [SerializeField] private string lastNameChallengeId = "last_name";
        [SerializeField] private string dateOfBirthChallengeId = "date_of_birth";

        [Header("Temporary Correct Answers")]
        [SerializeField] private string correctFirstName = "Jamie";
        [SerializeField] private string correctLastName = "Applicant";
        [SerializeField] private int correctBirthMonth = 4;
        [SerializeField] private int correctBirthDay = 22;
        [SerializeField] private int correctBirthYear = 2004;

        [Header("DOB Format Rotation")]
        [SerializeField] private string[] dateOfBirthFormats =
        {
            "MM/YYYY/DD",
            "YYYY/DD/MM",
            "DD/YYYY/MM",
            "MM/YY/DD",
            "YY/DD/MM",
            "DD/YY/MM"
        };

        [Header("Status Copy")]
        [TextArea] [SerializeField] private string initialStatus = "Enter your personal information. It already knows, but it wants to watch.";
        [TextArea] [SerializeField] private string namesAcceptedStatus = "Name confirmed. The File reluctantly agrees you exist.";
        [TextArea] [SerializeField] private string completeStatus = "Personal information confirmed. Proceed before the format changes again somehow.";

        [Header("Error Copy")]
        [TextArea] [SerializeField] private string identityMismatchError = "These details do not match what we have on file. Please refresh the page to continue.";
        [TextArea] [SerializeField] private string dateOfBirthMismatchError = "This birth date does not match what we have on file. Please refresh the page to continue.";

        public string IntroText => introText;
        public string FirstNameLabel => firstNameLabel;
        public string LastNameLabel => lastNameLabel;
        public string ConfirmNameButtonLabel => confirmNameButtonLabel;
        public string DateOfBirthLabelFormat => dateOfBirthLabelFormat;
        public string DateOfBirthHintText => dateOfBirthHintText;
        public string ConfirmDateOfBirthButtonLabel => confirmDateOfBirthButtonLabel;
        public string FirstNamePlaceholder => firstNamePlaceholder;
        public string LastNamePlaceholder => lastNamePlaceholder;
        public string DateOfBirthPlaceholder => dateOfBirthPlaceholder;
        public string FirstNameChallengeId => firstNameChallengeId;
        public string LastNameChallengeId => lastNameChallengeId;
        public string DateOfBirthChallengeId => dateOfBirthChallengeId;
        public string CorrectFirstName => correctFirstName;
        public string CorrectLastName => correctLastName;
        public int CorrectBirthMonth => correctBirthMonth;
        public int CorrectBirthDay => correctBirthDay;
        public int CorrectBirthYear => correctBirthYear;
        public IReadOnlyList<string> DateOfBirthFormats => dateOfBirthFormats;
        public string InitialStatus => initialStatus;
        public string NamesAcceptedStatus => namesAcceptedStatus;
        public string CompleteStatus => completeStatus;
        public string IdentityMismatchError => identityMismatchError;
        public string DateOfBirthMismatchError => dateOfBirthMismatchError;
    }
}
