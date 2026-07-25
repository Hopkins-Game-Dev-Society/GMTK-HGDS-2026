using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //This is where I keep the values picked for this run so the application, score, and clue systems can all use the same ones.
    public sealed class ApplicationSessionData
    {
        private readonly List<ApplicationQuestionRuntimeData> questions;
        private readonly List<ApplicationClueRuntimeData> clues;

        internal ApplicationSessionData(
            string sessionId,
            int seed,
            ApplicationApplicantRuntimeData applicant,
            List<ApplicationQuestionRuntimeData> questions,
            List<ApplicationClueRuntimeData> clues)
        {
            SessionId = sessionId;
            Seed = seed;
            Applicant = applicant;
            this.questions = questions ?? new List<ApplicationQuestionRuntimeData>();
            this.clues = clues ?? new List<ApplicationClueRuntimeData>();
        }

        public string SessionId { get; }
        public int Seed { get; }
        public ApplicationApplicantRuntimeData Applicant { get; }
        public IReadOnlyList<ApplicationQuestionRuntimeData> Questions => questions;
        public IReadOnlyList<ApplicationClueRuntimeData> Clues => clues;

        public ApplicationQuestionRuntimeData FindQuestionBySlot(string slotId)
        {
            if (string.IsNullOrWhiteSpace(slotId))
                return null;

            for (int i = 0; i < questions.Count; i++)
            {
                if (string.Equals(questions[i].SlotId, slotId, StringComparison.OrdinalIgnoreCase))
                    return questions[i];
            }

            return null;
        }
    }

    public sealed class ApplicationApplicantRuntimeData
    {
        internal ApplicationApplicantRuntimeData(
            ApplicationNameDefinition name,
            ApplicationRandomValueDefinition username,
            ApplicationRandomValueDefinition password,
            ApplicationRandomValueDefinition twoFactorCode)
        {
            NameId = name.NameId;
            UsernameId = username.ValueId;
            PasswordId = password.ValueId;
            TwoFactorCodeId = twoFactorCode.ValueId;
            FirstName = name.FirstName;
            LastName = name.LastName;
            Username = username.Value;
            Password = password.Value;
            TwoFactorCode = twoFactorCode.Value;
        }

        public string ApplicantId => NameId;
        public string NameId { get; }
        public string UsernameId { get; }
        public string PasswordId { get; }
        public string TwoFactorCodeId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Password { get; }
        public string TwoFactorCode { get; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public sealed class ApplicationQuestionRuntimeData
    {
        private readonly List<ApplicationQuestionAnswerOption> possibleAnswers = new();

        internal ApplicationQuestionRuntimeData(
            string slotId,
            ApplicationSectionId sectionId,
            int weight,
            ApplicationQuestionDefinition definition)
        {
            SlotId = slotId;
            SectionId = sectionId;
            Weight = Mathf.Max(0, weight);
            QuestionId = definition.QuestionId;
            Prompt = definition.Prompt;
            AuthoredReference = definition.AuthoredReference;
            PreferredAnswerId = definition.PreferredAnswerId;
            Definition = definition;

            IReadOnlyList<ApplicationQuestionAnswerDefinition> answers = definition.PossibleAnswers;
            for (int i = 0; i < answers.Count; i++)
            {
                ApplicationQuestionAnswerDefinition answer = answers[i];
                if (answer == null || string.IsNullOrWhiteSpace(answer.AnswerId))
                    continue;

                possibleAnswers.Add(new ApplicationQuestionAnswerOption(
                    answer.AnswerId,
                    answer.AnswerText));
            }
        }

        public string SlotId { get; }
        public ApplicationSectionId SectionId { get; }
        public int Weight { get; }
        public string QuestionId { get; }
        public string Prompt { get; }
        public UnityEngine.Object AuthoredReference { get; }
        public string PreferredAnswerId { get; }
        public IReadOnlyList<ApplicationQuestionAnswerOption> PossibleAnswers => possibleAnswers;
        internal ApplicationQuestionDefinition Definition { get; }
    }

    public sealed class ApplicationQuestionAnswerOption
    {
        internal ApplicationQuestionAnswerOption(string answerId, string answerText)
        {
            AnswerId = answerId;
            AnswerText = answerText;
        }

        public string AnswerId { get; }
        public string AnswerText { get; }
    }

    public sealed class ApplicationQuestionAnswerRecord
    {
        internal ApplicationQuestionAnswerRecord(
            string slotId,
            string questionId,
            string answerId,
            int rating,
            int weight)
        {
            SlotId = slotId;
            QuestionId = questionId;
            AnswerId = answerId;
            Rating = Mathf.Clamp(rating, 0, 10);
            Weight = Mathf.Max(0, weight);
        }

        public string SlotId { get; }
        public string QuestionId { get; }
        public string AnswerId { get; }
        public int Rating { get; }
        public int Weight { get; }
    }

    public sealed class ApplicationClueRuntimeData
    {
        internal ApplicationClueRuntimeData(
            ApplicationClueDefinition definition,
            string targetSlotId)
        {
            ClueId = definition.ClueId;
            Category = definition.Category;
            TargetId = definition.TargetId;
            TargetSlotId = targetSlotId;
            AssociatedAnswer = definition.AssociatedAnswer;
            VariantId = definition.VariantId;
            AuthoredReference = definition.AuthoredReference;
            SpawnGroupId = definition.SpawnGroupId;
            ResolvedPayload = definition.ResolvedPayload;
        }

        public string ClueId { get; }
        public ApplicationClueCategory Category { get; }
        public string TargetId { get; }
        public string TargetSlotId { get; }
        public string AssociatedAnswer { get; }
        public string VariantId { get; }
        public UnityEngine.Object AuthoredReference { get; }
        public string SpawnGroupId { get; }
        public string ResolvedPayload { get; }
    }
}
