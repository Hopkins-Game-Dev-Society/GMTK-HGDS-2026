namespace BirthdayJobJam.Application
{
    public enum ApplicationWrongAnswerConsequence
    {
        None = 0,
        ResetChallenge = 10,
        ResetCurrentSection = 20,
        RequireRefresh = 30
    }
}
