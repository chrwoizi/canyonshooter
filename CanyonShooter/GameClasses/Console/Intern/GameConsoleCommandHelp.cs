using System.Collections;
using System.Collections.Generic;

namespace CanyonShooter.GameClasses.Console.Intern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleCommandHelp : IGameConsoleInternCommand
    {
        private Hashtable register;



        public GameConsoleCommandHelp(Hashtable register)
        {
            this.register = register;
        }



        public string Info
        {
            get
            {
                return "Gives help.";
            }
        }



        public string Execute(string[] parameters)
        {
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();

            foreach (string key in register.Keys)
            {
                IGameConsoleInternCommand command = register[key] as IGameConsoleInternCommand;
                if (command != null)
                {
                    dict.Add(key, command.Info);
                }
            }

            int size = "Command".Length;
            foreach (string key in dict.Keys)
            {
                if (size < key.Length)
                    size = key.Length;
            }

            size += 3;

            string result =
                "\n" +
                "Command".PadRight(size) + "Function\n" +
                "-----------------------------------------------\n";
            foreach (string key in dict.Keys)
            {
                result += key.PadRight(size) + dict[key] + "\n";
            }


            result += "\n" +
                "Type \"<Command> -?\" for further information.";
            return result;
        }
    }
}
