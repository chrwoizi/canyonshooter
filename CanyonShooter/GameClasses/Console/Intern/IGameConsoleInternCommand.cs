namespace CanyonShooter.GameClasses.Console.Intern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    interface IGameConsoleInternCommand
    {
        string Execute(string[] parameters);

        string Info
        {
            get;
        }
    }
}
