using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //So here I am keeping the separate lists that are used when making a randomized application run.
    [CreateAssetMenu(
        fileName = "ApplicationSessionCatalog",
        menuName = "Birthday Job Jam/Application/Session Catalog")]
    public sealed class ApplicationSessionCatalog : ScriptableObject
    {
        [SerializeField] private List<ApplicationNameDefinition> names = new();
        [SerializeField] private List<ApplicationRandomValueDefinition> usernames = new();
        [SerializeField] private List<ApplicationRandomValueDefinition> passwords = new();
        [SerializeField] private List<ApplicationRandomValueDefinition> twoFactorCodes = new();
        [SerializeField] private List<ApplicationQuestionDefinition> questions = new();
        [SerializeField] private List<ApplicationMadlibDefinition> madlibs = new();
        [SerializeField] private List<ApplicationClueDefinition> clues = new();

        public IReadOnlyList<ApplicationNameDefinition> Names => names;
        public IReadOnlyList<ApplicationRandomValueDefinition> Usernames => usernames;
        public IReadOnlyList<ApplicationRandomValueDefinition> Passwords => passwords;
        public IReadOnlyList<ApplicationRandomValueDefinition> TwoFactorCodes => twoFactorCodes;
        public IReadOnlyList<ApplicationQuestionDefinition> Questions => questions;
        public IReadOnlyList<ApplicationMadlibDefinition> Madlibs => madlibs;
        public IReadOnlyList<ApplicationClueDefinition> Clues => clues;
    }

    [Serializable]
    public sealed class ApplicationNameDefinition
    {
        [SerializeField] private string nameId;
        [SerializeField] private string firstName;
        [SerializeField] private string lastName;

        public string NameId => nameId;
        public string FirstName => firstName;
        public string LastName => lastName;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(nameId)
            && !string.IsNullOrWhiteSpace(firstName)
            && !string.IsNullOrWhiteSpace(lastName);
    }

    [Serializable]
    public sealed class ApplicationRandomValueDefinition
    {
        [SerializeField] private string valueId;
        [SerializeField] private string value;

        public string ValueId => valueId;
        public string Value => value;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(valueId)
            && !string.IsNullOrWhiteSpace(value);
    }

    [Serializable]
    public sealed class ApplicationQuestionDefinition
    {
        [SerializeField] private string questionId;
        [TextArea] [SerializeField] private string prompt;
        [SerializeField] private UnityEngine.Object authoredReference;
        [SerializeField] private string preferredAnswerId;
        [SerializeField] private bool randomizePreferredAnswer;
        [SerializeField] private List<string> incompatibleQuestionIds = new();
        [SerializeField] private List<ApplicationQuestionAnswerDefinition> possibleAnswers = new();

        public string QuestionId => questionId;
        public string Prompt => prompt;
        public UnityEngine.Object AuthoredReference => authoredReference;
        public string PreferredAnswerId => preferredAnswerId;
        public bool RandomizePreferredAnswer => randomizePreferredAnswer;
        public IReadOnlyList<string> IncompatibleQuestionIds => incompatibleQuestionIds;
        public IReadOnlyList<ApplicationQuestionAnswerDefinition> PossibleAnswers => possibleAnswers;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(questionId)
            && !string.IsNullOrWhiteSpace(prompt)
            && possibleAnswers != null
            && possibleAnswers.Count > 0;

        public bool TryGetAnswer(string answerId, out ApplicationQuestionAnswerDefinition answer)
        {
            answer = null;

            if (string.IsNullOrWhiteSpace(answerId) || possibleAnswers == null)
                return false;

            for (int i = 0; i < possibleAnswers.Count; i++)
            {
                ApplicationQuestionAnswerDefinition candidate = possibleAnswers[i];
                if (candidate == null)
                    continue;

                if (!string.Equals(candidate.AnswerId, answerId, StringComparison.OrdinalIgnoreCase))
                    continue;

                answer = candidate;
                return true;
            }

            return false;
        }

        public bool IsCompatibleWith(ApplicationQuestionDefinition other)
        {
            if (other == null)
                return true;

            return !ContainsQuestionId(incompatibleQuestionIds, other.QuestionId)
                && !ContainsQuestionId(other.incompatibleQuestionIds, questionId);
        }

        private static bool ContainsQuestionId(IReadOnlyList<string> questionIds, string questionId)
        {
            if (questionIds == null || string.IsNullOrWhiteSpace(questionId))
                return false;

            for (int i = 0; i < questionIds.Count; i++)
            {
                if (string.Equals(questionIds[i], questionId, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    public sealed class ApplicationQuestionAnswerDefinition
    {
        [SerializeField] private string answerId;
        [TextArea] [SerializeField] private string answerText;
        [SerializeField, Range(0, 10)] private int rating;

        public string AnswerId => answerId;
        public string AnswerText => answerText;
        public int Rating => Mathf.Clamp(rating, 0, 10);
    }

    [Serializable]
    public sealed class ApplicationMadlibDefinition
    {
        [SerializeField] private string madlibId;
        [TextArea] [SerializeField] private string prompt;
        [TextArea] [SerializeField] private string sentenceFormat;
        [SerializeField] private List<ApplicationMadlibBlankDefinition> blanks = new();
        [SerializeField] private List<ApplicationMadlibWordDefinition> wordBank = new();

        public string MadlibId => madlibId;
        public string Prompt => prompt;
        public string SentenceFormat => sentenceFormat;
        public IReadOnlyList<ApplicationMadlibBlankDefinition> Blanks => blanks;
        public IReadOnlyList<ApplicationMadlibWordDefinition> WordBank => wordBank;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(madlibId)
            && !string.IsNullOrWhiteSpace(prompt)
            && !string.IsNullOrWhiteSpace(sentenceFormat)
            && blanks != null
            && blanks.Count > 0
            && wordBank != null
            && wordBank.Count > 0;
    }

    [Serializable]
    public sealed class ApplicationMadlibBlankDefinition
    {
        [SerializeField] private string label;
        [SerializeField] private string correctWordId;

        public string Label => label;
        public string CorrectWordId => correctWordId;
    }

    [Serializable]
    public sealed class ApplicationMadlibWordDefinition
    {
        [SerializeField] private string wordId;
        [SerializeField] private string text;

        public string WordId => wordId;
        public string Text => text;
    }

    public enum ApplicationClueCategory
    {
        Identity = 0,
        Credentials = 10,
        Question = 20
    }

    [Serializable]
    public sealed class ApplicationClueDefinition
    {
        [SerializeField] private string clueId;
        [SerializeField] private ApplicationClueCategory category;
        [SerializeField] private string targetId;
        [SerializeField] private string associatedAnswer;
        [SerializeField] private string variantId;
        [SerializeField] private UnityEngine.Object authoredReference;
        [SerializeField] private string spawnGroupId;
        [TextArea] [SerializeField] private string resolvedPayload;

        public string ClueId => clueId;
        public ApplicationClueCategory Category => category;
        public string TargetId => targetId;
        public string AssociatedAnswer => associatedAnswer;
        public string VariantId => variantId;
        public UnityEngine.Object AuthoredReference => authoredReference;
        public string SpawnGroupId => spawnGroupId;
        public string ResolvedPayload => resolvedPayload;

        public bool Matches(string requiredTargetId, string requiredAnswer)
        {
            return string.Equals(targetId, requiredTargetId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(associatedAnswer, requiredAnswer, StringComparison.Ordinal);
        }
    }
}
