namespace BirthdayJobJam.Core
{
    public static class Game
    {
        public static GameContext Ctx { get; private set; }
        public static bool IsReady => Ctx != null;

        internal static void SetContext(GameContext context)
        {
            Ctx = context;
        }

        internal static void ClearContext(GameContext context)
        {
            if (Ctx == context)
                Ctx = null;
        }
    }
}
