namespace CanyonShooter.GameClasses.Console.Extern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    interface IGameConsoleExternCommand
    {
        string Execute(string[] parameters);

        string Info
        {
            get;
        }
    }
}
