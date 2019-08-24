namespace CanyonShooter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CanyonShooterGame game = new CanyonShooterGame(args))
            {
                game.Run();
            }
        }
    }
}

