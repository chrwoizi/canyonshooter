namespace CanyonShooter.GameClasses.Console.Intern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleCommandText : IGameConsoleInternCommand
    {
        private string infoText;
        private string executeText;



        public GameConsoleCommandText(string infoText, string executeText)
        {
            this.infoText = infoText;
            this.executeText = executeText;
        }



        public string Info
        {
            get
            {
                return this.infoText;
            }
        }



        public string Execute(string[] parameters)
        {
            return this.executeText;
        }
    }
}
