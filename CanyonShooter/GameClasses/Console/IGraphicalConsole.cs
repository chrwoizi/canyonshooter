using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Console
{
    interface IGraphicalConsole : IGameComponent, IDrawable, IGameConsole
    {
        IGraphicalConsole GetSingleton();
        bool WriteLine(string value, int console);
    }
}
