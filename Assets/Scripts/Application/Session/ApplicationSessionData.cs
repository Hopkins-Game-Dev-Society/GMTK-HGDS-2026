using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //This is where I keep the values picked for this run so the application, score, and clue systems can all use the same ones.
    public sealed class ApplicationSessionData
    {
        private readonly List<ApplicationQuestionRuntimeData> questions;
        private readonly List<ApplicationMadlibRuntimeData> madlibs;
        private readonly List<ApplicationClueRuntimeData> clues;

        internal ApplicationSessionData(
            string sessionId,
            int seed,
            ApplicationApplicantRuntimeData applicant,
            List<ApplicationQuestionRuntimeData> questions,
            List<ApplicationMadlibRuntimeData> madlibs,
            List<ApplicationClueRuntimeData> clues)
        {
            SessionId = sessionId;
            Seed = seed;
            Applicant = applicant;
            this.questions = questions ?? new List<ApplicationQuestionRuntimeData>();
            this.madlibs = madlibs ?? new List<ApplicationMadlibRuntimeData>();
            this.clues = clues ?? new List<ApplicationClueRuntimeData>();
        }

        public string SessionId { get; }
        public int Seed { get; }
        public ApplicationApplicantRuntimeData Applicant { get; }
        public IReadOnlyList<ApplicationQuestionRuntimeData> Questions => questions;
        public IReadOnlyList<ApplicationMadlibRuntimeData> Madlibs => madlibs;
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

        public ApplicationMadlibRuntimeData FindMadlibBySlot(string slotId)
        {
            if (string.IsNullOrWhiteSpace(slotId))
                return null;

            for (int i = 0; i < madlibs.Count; i++)
            {
                if (string.Equals(madlibs[i].SlotId, slotId, StringComparison.OrdinalIgnoreCase))
                    return madlibs[i];
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
            ApplicationQuestionDefinition definition,
            string preferredAnswerId)
        {
            SlotId = slotId;
            SectionId = sectionId;
            Weight = Mathf.Max(0, weight);
            QuestionId = definition.QuestionId;
            Prompt = definition.Prompt;
            AuthoredReference = definition.AuthoredReference;
            PreferredAnswerId = preferredAnswerId;
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

    public sealed class ApplicationMadlibRuntimeData
    {
        private readonly List<ApplicationMadlibBlankRuntimeData> blanks = new();
        private readonly List<ApplicationMadlibWordOption> wordBank = new();

        internal ApplicationMadlibRuntimeData(
            string slotId,
            ApplicationSectionId sectionId,
            int weight,
            ApplicationMadlibDefinition definition)
        {
            SlotId = slotId;
            SectionId = sectionId;
            Weight = Mathf.Max(0, weight);
            MadlibId = definition.MadlibId;
            Prompt = definition.Prompt;
            SentenceFormat = definition.SentenceFormat;

            IReadOnlyList<ApplicationMadlibBlankDefinition> authoredBlanks = definition.Blanks;
            for (int i = 0; i < authoredBlanks.Count; i++)
            {
                ApplicationMadlibBlankDefinition blank = authoredBlanks[i];
                if (blank == null || string.IsNullOrWhiteSpace(blank.CorrectWordId))
                    continue;

                blanks.Add(new ApplicationMadlibBlankRuntimeData(
                    string.IsNullOrWhiteSpace(blank.Label) ? $"Blank {i + 1}" : blank.Label,
                    blank.CorrectWordId));
            }

            IReadOnlyList<ApplicationMadlibWordDefinition> authoredWords = definition.WordBank;
            for (int i = 0; i < authoredWords.Count; i++)
            {
                ApplicationMadlibWordDefinition word = authoredWords[i];
                if (word == null || string.IsNullOrWhiteSpace(word.WordId))
                    continue;

                wordBank.Add(new ApplicationMadlibWordOption(
                    word.WordId,
                    string.IsNullOrWhiteSpace(word.Text) ? word.WordId : word.Text));
            }

        }

        public string SlotId { get; }
        public ApplicationSectionId SectionId { get; }
        public int Weight { get; }
        public string MadlibId { get; }
        public string Prompt { get; }
        public string SentenceFormat { get; }
        public IReadOnlyList<ApplicationMadlibBlankRuntimeData> Blanks => blanks;
        public IReadOnlyList<ApplicationMadlibWordOption> WordBank => wordBank;

        public bool IsCorrect(IReadOnlyList<string> selectedWordIds)
        {
            if (selectedWordIds == null || selectedWordIds.Count != blanks.Count)
                return false;

            for (int i = 0; i < blanks.Count; i++)
            {
                if (!string.Equals(
                        selectedWordIds[i],
                        blanks[i].CorrectWordId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public sealed class ApplicationMadlibBlankRuntimeData
    {
        internal ApplicationMadlibBlankRuntimeData(string label, string correctWordId)
        {
            Label = label;
            CorrectWordId = correctWordId;
        }

        public string Label { get; }
        public string CorrectWordId { get; }
    }

    public sealed class ApplicationMadlibWordOption
    {
        internal ApplicationMadlibWordOption(string wordId, string text)
        {
            WordId = wordId;
            Text = text;
        }

        public string WordId { get; }
        public string Text { get; }
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
