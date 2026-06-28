namespace FieldServiceManagement.Helpers
{
    public static class GreetingHelper
    {
        private static readonly Random Random = new();

        private static readonly string[] MorningGreetings =
        {
            "Good morning",
            //"Welcome back",
            //"Great to see you again"
        };

        private static readonly string[] AfternoonGreetings =
        {
            "Good afternoon",
            //"Welcome back",
            //"Great to see you again"
        };

        private static readonly string[] EveningGreetings =
        {
            "Good evening",
            //"Welcome back",
            //"Great to see you again"
        };

        public static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;

            string[] greetings = hour switch
            {
                >= 5 and < 12 => MorningGreetings,
                >= 12 and < 18 => AfternoonGreetings,
                _ => EveningGreetings
            };

            return greetings[Random.Next(greetings.Length)];
        }
    }
}