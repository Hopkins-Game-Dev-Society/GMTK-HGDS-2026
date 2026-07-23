using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [CreateAssetMenu(fileName = "ExperiencePageContent_", menuName = "Birthday Job Jam/Application/Experience Page Content")]
    public sealed class ApplicationExperiencePageContent : ScriptableObject
    {
        [Header("Page Copy")]
        [TextArea] [SerializeField] private string introText = "Please upload your resume. Any resume. Ideally the correct one.";
        [SerializeField] private string uploadResumeButtonLabel = "Upload Resume";
        [SerializeField] private string completeStatus = "Resume uploaded. Your experience has been accepted, pending inevitable disappointment.";

        [Header("Finder Copy")]
        [SerializeField] private string pickerTitle = "Choose Resume";
        [SerializeField] private string pickerPath = "Macintosh HD > Users > Applicant > Desktop > resume graveyard";
        [SerializeField] private string openButtonLabel = "Open";
        [SerializeField] private string selectButtonLabel = "Select";
        [SerializeField] private string cancelButtonLabel = "Cancel";
        [TextArea] [SerializeField] private string wordActivationError = "You need to activate Mycosoft Word. Cannot open.";
        [TextArea] [SerializeField] private string incorrectResumeError = "Your experience is rough. It looks like you might be unemployed for a while.";

        [Header("Challenge")]
        [SerializeField] private string resumeChallengeId = "resume_upload";
        [SerializeField, Min(0)] private int correctResumeIndex = 5;
        [SerializeField] private string[] resumeFileNames =
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

        public string IntroText => introText;
        public string UploadResumeButtonLabel => uploadResumeButtonLabel;
        public string CompleteStatus => completeStatus;
        public string PickerTitle => pickerTitle;
        public string PickerPath => pickerPath;
        public string OpenButtonLabel => openButtonLabel;
        public string SelectButtonLabel => selectButtonLabel;
        public string CancelButtonLabel => cancelButtonLabel;
        public string WordActivationError => wordActivationError;
        public string IncorrectResumeError => incorrectResumeError;
        public string ResumeChallengeId => resumeChallengeId;
        public int CorrectResumeIndex => correctResumeIndex;
        public IReadOnlyList<string> ResumeFileNames => resumeFileNames;
    }
}
