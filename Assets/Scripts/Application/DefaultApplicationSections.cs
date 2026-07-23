using System.Collections.Generic;

namespace BirthdayJobJam.Application
{
    internal static class DefaultApplicationSections
    {
        public static List<ApplicationSectionDefinition> Create()
        {
            return new List<ApplicationSectionDefinition>
            {
                Section(ApplicationSectionId.CreateAccountSignIn, "Sign In", "Sign In",
                    Challenge("username", "Username"),
                    Challenge("password", "Password"),
                    Challenge("two_factor_code", "Two-Factor Authentication Code")),

                Section(ApplicationSectionId.MyInformation, "My Information", "My\nInformation",
                    Challenge("first_name", "First Name"),
                    Challenge("last_name", "Last Name"),
                    Challenge("date_of_birth", "Date of Birth")),

                Section(ApplicationSectionId.MyExperience, "My Experience", "My\nExperience",
                    Challenge("previous_job", "Previous Job Experience"),
                    Challenge("employment_gap", "Explain Employment Gap"),
                    Challenge("why_not_working_at_12", "Why Weren't You Working At 12?")),

                Section(ApplicationSectionId.ApplicationQuestionsOne, "Application Questions 1 of 2", "Questions\n1 of 2",
                    Challenge("question_1", "Question 1"),
                    Challenge("question_2", "Question 2"),
                    Challenge("question_3", "Question 3")),

                Section(ApplicationSectionId.ApplicationQuestionsTwo, "Application Questions 2 of 2", "Questions\n2 of 2",
                    Challenge("madlibs_personal_statement", "Mad-Libs Personal Statement"),
                    Challenge("question_4", "Question 4"),
                    Challenge("question_5", "Question 5")),

                Section(ApplicationSectionId.VoluntaryDisclosures, "Voluntary Disclosures", "Voluntary\nDisclosures",
                    Challenge("visa_sponsorship", "Visa Sponsorship Disclosure"),
                    Challenge("future_sponsorship", "Future Sponsorship Disclosure"),
                    Challenge("voluntary_certification", "Voluntary Disclosure Certification")),

                Section(ApplicationSectionId.Review, "Review", "Review",
                    Challenge("review_all_fields", "Review All Fields"),
                    Challenge("final_certification", "Final Certification"))
            };
        }

        private static ApplicationSectionDefinition Section(
            ApplicationSectionId id,
            string displayName,
            string progressLabel,
            params ApplicationChallengeDefinition[] challenges)
        {
            return new ApplicationSectionDefinition(
                id,
                displayName,
                progressLabel,
                refreshCooldownSeconds: 5f,
                resetChallengesOnRefresh: true,
                challenges: new List<ApplicationChallengeDefinition>(challenges));
        }

        private static ApplicationChallengeDefinition Challenge(
            string id,
            string displayName,
            bool required = true,
            int maxPoints = 1)
        {
            return new ApplicationChallengeDefinition(id, displayName, required, maxPoints);
        }
    }
}
