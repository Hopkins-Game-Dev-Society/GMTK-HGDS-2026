using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    public sealed class ApplicationJobApplicationDebugOverlay : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float WindowWidth = 560f;
        private const float WindowHeight = 720f;

        private ApplicationStateModel applicationState;
        private ApplicationSessionManager applicationSession;
        private ApplicationSignInPageView pageView;
        private Rect windowRect = new Rect(24f, 24f, WindowWidth, WindowHeight);
        private Vector2 scrollPosition;
        private bool visible;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateForEditorPlayMode()
        {
            if (!global::UnityEngine.Application.isEditor)
                return;

            if (FindAnyObjectByType<ApplicationJobApplicationDebugOverlay>() != null)
                return;

            GameObject overlayObject = new GameObject("Job Application Debug Overlay");
            DontDestroyOnLoad(overlayObject);
            overlayObject.hideFlags = HideFlags.DontSave;
            overlayObject.AddComponent<ApplicationJobApplicationDebugOverlay>();
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;
            if (currentEvent != null
                && currentEvent.type == EventType.KeyDown
                && currentEvent.keyCode == KeyCode.F1)
            {
                visible = !visible;
                currentEvent.Use();
            }

            if (!visible)
                return;

            ResolveReferences();
            windowRect = GUILayout.Window(
                GetInstanceID(),
                windowRect,
                DrawWindow,
                "Job Application Debug (F1)");
        }

        private void DrawWindow(int windowId)
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Width(WindowWidth - 16f),
                GUILayout.Height(WindowHeight - 48f));

            DrawScreenButtons();
            GUILayout.Space(12f);
            DrawAnswers();

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0f, 0f, WindowWidth, 24f));
        }

        private void DrawScreenButtons()
        {
            GUILayout.Label("Teleport");

            GUI.enabled = applicationState != null;
            if (GUILayout.Button("Job Listing"))
                JumpToJobListing();

            IReadOnlyList<ApplicationSectionRuntimeState> sections = applicationState != null
                ? applicationState.Sections
                : null;

            if (sections != null)
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    ApplicationSectionRuntimeState section = sections[i];
                    if (section == null)
                        continue;

                    if (GUILayout.Button($"{i + 1}. {section.DisplayName}"))
                        JumpToSection(section.SectionId);
                }
            }

            GUI.enabled = applicationSession != null;
            if (GUILayout.Button("Regenerate Run Answers"))
                applicationSession.GenerateNewSession();

            GUI.enabled = true;
        }

        private void DrawAnswers()
        {
            GUILayout.Label("Right Answers This Run");

            string report = BuildAnswerReport();
            GUILayout.TextArea(report, GUILayout.MinHeight(360f));

            if (GUILayout.Button("Copy Answers"))
                GUIUtility.systemCopyBuffer = report;
        }

        private void JumpToJobListing()
        {
            if (pageView != null && pageView.DebugJumpToJobListing())
                return;

            applicationState?.TryGoToSection(ApplicationSectionId.CreateAccountSignIn);
            applicationState?.DebugForceClearCurrentSectionBlock();
        }

        private void JumpToSection(ApplicationSectionId sectionId)
        {
            if (pageView != null && pageView.DebugJumpToSection(sectionId))
                return;

            if (applicationState != null && applicationState.TryGoToSection(sectionId))
                applicationState.DebugForceClearCurrentSectionBlock();
        }

        private string BuildAnswerReport()
        {
            ResolveReferences();

            StringBuilder builder = new StringBuilder();

            if (applicationSession == null || !applicationSession.HasSession)
            {
                builder.AppendLine("No application session has generated yet.");
                builder.AppendLine("Enter Play Mode in the gameplay scene, or press Regenerate Run Answers.");
                return builder.ToString();
            }

            ApplicationSessionData session = applicationSession.Current;
            ApplicationApplicantRuntimeData applicant = session.Applicant;

            builder.AppendLine($"Session: {session.SessionId}");
            builder.AppendLine($"Seed: {session.Seed}");
            builder.AppendLine();

            if (applicant != null)
            {
                builder.AppendLine("Sign In");
                builder.AppendLine($"Username: {GetPageAnswer(pageView?.DebugCorrectUsername, applicant.Username)}");
                builder.AppendLine($"Password: {GetPageAnswer(pageView?.DebugCorrectPassword, applicant.Password)}");
                builder.AppendLine($"2FA / session reauth: {GetPageAnswer(pageView?.DebugCorrectTwoFactorCode, applicant.TwoFactorCode)}");
                builder.AppendLine();

                builder.AppendLine("My Information");
                builder.AppendLine($"First name: {GetPageAnswer(pageView?.DebugCorrectFirstName, applicant.FirstName)}");
                builder.AppendLine($"Last name: {GetPageAnswer(pageView?.DebugCorrectLastName, applicant.LastName)}");
                builder.AppendLine($"DOB now: {GetPageAnswer(pageView?.DebugCorrectDateOfBirth, "Unavailable")} ({GetPageAnswer(pageView?.DebugCurrentDateOfBirthFormat, "current format unavailable")})");
                builder.AppendLine();
            }

            builder.AppendLine("My Experience");
            if (pageView != null)
                builder.AppendLine($"Resume: #{pageView.DebugCorrectResumeIndex + 1} {pageView.DebugCorrectResumeFileName}");
            else
                builder.AppendLine("Resume: page view unavailable");
            builder.AppendLine();

            AppendQuestionAnswers(builder, session.Questions);
            AppendClues(builder, session.Clues);

            return builder.ToString();
        }

        private static void AppendQuestionAnswers(
            StringBuilder builder,
            IReadOnlyList<ApplicationQuestionRuntimeData> questions)
        {
            builder.AppendLine("Application Questions");

            if (questions == null || questions.Count == 0)
            {
                builder.AppendLine("No questions selected for this run.");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < questions.Count; i++)
            {
                ApplicationQuestionRuntimeData question = questions[i];
                string answerText = FindAnswerText(question, question.PreferredAnswerId);
                builder.AppendLine($"{question.SlotId} ({question.SectionId})");
                builder.AppendLine($"Q: {question.Prompt}");
                builder.AppendLine($"A: {answerText} [{question.PreferredAnswerId}]");
                builder.AppendLine();
            }
        }

        private static void AppendClues(
            StringBuilder builder,
            IReadOnlyList<ApplicationClueRuntimeData> clues)
        {
            builder.AppendLine("Selected Clues");

            if (clues == null || clues.Count == 0)
            {
                builder.AppendLine("No clues selected for this run.");
                return;
            }

            for (int i = 0; i < clues.Count; i++)
            {
                ApplicationClueRuntimeData clue = clues[i];
                builder.AppendLine($"{clue.ClueId}: {clue.TargetId} = {clue.AssociatedAnswer}");
                if (!string.IsNullOrWhiteSpace(clue.ResolvedPayload))
                    builder.AppendLine($"  {clue.ResolvedPayload}");
            }
        }

        private static string FindAnswerText(
            ApplicationQuestionRuntimeData question,
            string answerId)
        {
            if (question?.PossibleAnswers == null)
                return answerId;

            for (int i = 0; i < question.PossibleAnswers.Count; i++)
            {
                ApplicationQuestionAnswerOption answer = question.PossibleAnswers[i];
                if (answer != null
                    && string.Equals(answer.AnswerId, answerId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return answer.AnswerText;
                }
            }

            return answerId;
        }

        private void ResolveReferences()
        {
            if (applicationState == null)
                applicationState = FindAnyObjectByType<ApplicationStateModel>();

            if (applicationSession == null)
                applicationSession = FindAnyObjectByType<ApplicationSessionManager>();

            if (pageView == null)
                pageView = FindAnyObjectByType<ApplicationSignInPageView>();
        }

        private static string GetPageAnswer(string pageValue, string fallback)
        {
            return string.IsNullOrWhiteSpace(pageValue) ? fallback : pageValue;
        }
#endif
    }
}
