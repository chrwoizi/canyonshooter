using System.Collections;
using CanyonShooter.GameClasses.Console.Extern;

namespace CanyonShooter.GameClasses.Console.Intern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleCommandList : IGameConsoleInternCommand
    {
        private Hashtable register;



        public GameConsoleCommandList(Hashtable register)
        {
            this.register = register;
        }



        public string Info
        {
            get
            {
                return "Lists all registered objects.";
            }
        }



        public string Execute(string[] parameters)
        {
            string result = "\n" +
                "object-name : <object-type>\n" +
                "---------------------------------\n";
            foreach (string key in register.Keys)
            {
                GameConsoleObject consoleObject = register[key] as GameConsoleObject;
                if (consoleObject != null)
                {
                    result += consoleObject.Info;
                }
            }
            result += "\n";
            result += "For information on an object type \"<object-name> -?\".";
            return result;
        }
    }
}
