using System;
using System.Collections;
using System.Collections.Generic;
using CanyonShooter.GameClasses.Console.Extern;
using CanyonShooter.GameClasses.Console.Intern;

namespace CanyonShooter.GameClasses.Console
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsole : IGameConsole
    {
        private Hashtable register = new Hashtable();
        private GameConsoleTypeParserCollection typeParserCollections = new GameConsoleTypeParserCollection();
        private bool caseSensitive;



        /// <summary>
        /// Initializes a new instance of the <see cref="GameConsole"/> class.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        public GameConsole(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
            RegisterCommand("HELP", new GameConsoleCommandHelp(this.register));
            RegisterCommand("LIST", new GameConsoleCommandList(this.register));
            //RegisterCommand("QUIT", new GameConsoleCommandText("Quits this console", ""));
            //RegisterCommand("EXIT", new GameConsoleCommandText("Quits this console", ""));
        }



        #region IGameConsole Members
        /// <summary>
        /// Registers a function of an object and all its overloads.
        /// </summary>
        /// <param name="functionOwner">The object, that owns the specified function.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <returns></returns>
        public bool RegisterObjectFunction(object functionOwner, string objectName, string functionName)
        {
            if (functionOwner == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(functionOwner, functionOwner.GetType(), objectName);
            consoleObject.RegisterFunction(functionName);
            return true;
        }



        /// <summary>
        /// Register a function of an object, specified by the parameter-types.
        /// </summary>
        /// <param name="functionOwner">The object, that owns the specified function.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <param name="parameterTypes">The parameter types of the specified function in correct order.</param>
        /// <returns></returns>
        public bool RegisterObjectFunction(object functionOwner, string objectName, string functionName, Type[] parameterTypes)
        {
            if (functionOwner == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(functionOwner, functionOwner.GetType(), objectName);
            consoleObject.RegisterFunction(functionName, parameterTypes);
            return true;
        }



        /// <summary>
        /// Registers a property of an object.
        /// </summary>
        /// <param name="propertyOwner">The object, that owns the specified property.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="propertyName">Name of the property as declared in its class.</param>
        /// <returns></returns>
        public bool RegisterObjectProperty(object propertyOwner, string objectName, string propertyName)
        {
            if (propertyOwner == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(propertyOwner, propertyOwner.GetType(), objectName);
            consoleObject.RegisterProperty(propertyName);
            return true;
        }



        /// <summary>
        /// Registers a static function of a class and all its overloads.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified function.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <returns></returns>
        public bool RegisterStaticFunction(Type classType, string className, string functionName)
        {
            if (classType == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(null, classType, className);
            consoleObject.RegisterFunction(functionName);
            return true;
        }



        /// <summary>
        /// Registers a static function of a class and all its overloads.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified function.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <param name="parameterTypes">The parameter types of the specified function in correct order.</param>
        /// <returns></returns>
        public bool RegisterStaticFunction(Type classType, string className, string functionName, Type[] parameterTypes)
        {
            if (classType == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(null, classType, className);
            consoleObject.RegisterFunction(functionName, parameterTypes);
            return true;
        }



        /// <summary>
        /// Registers the static property.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified property.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="propertyName">Name of the property as declared in its class.</param>
        /// <returns></returns>
        public bool RegisterStaticProperty(Type classType, string className, string propertyName)
        {
            if (classType == null)
                return false;
            GameConsoleObject consoleObject = this.GetConsoleObject(null, classType, className);
            consoleObject.RegisterProperty(propertyName);
            return true;
        }



        /// <summary>
        /// Registers the type converter to parse console parameters.
        /// </summary>
        /// <param name="typeConverter">The type converter.</param>
        /// <returns></returns>
        public bool RegisterTypeParser(GameConsoleTypeParser typeParser)
        {
            if (typeParser == null)
                return false;
            typeParserCollections.Add(typeParser);
            return true;
        }
        #endregion



        public GameConsoleTypeParserCollection TypeConverterCollection
        {
            get
            {
                return this.typeParserCollections;
            }
        }


        /// <summary>
        /// Gets the ConsoleObject for the objectName from Hashtable.
        /// If not exists, it is created and added to Hashtable.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        private GameConsoleObject GetConsoleObject(object sender, Type senderType, string objectName)
        {
            GameConsoleObject consoleObject = null;

            // create new hashtable entry, if not yet included
            if (register.ContainsKey(objectName))
            {
                consoleObject = register[objectName] as GameConsoleObject;
            }
            else
            {
                consoleObject = new GameConsoleObject(this, sender, senderType, objectName);
                register.Add(objectName, consoleObject);
            }

            return consoleObject;
        }



        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private bool RegisterCommand(string commandName, IGameConsoleInternCommand command)
        {
            register.Add(commandName.ToUpper(), command);
            return true;
        }



        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="commandStart">The command start.</param>
        /// <returns></returns>
        public string[] GetSuggestions(string commandStart)
        {
            List<string> list = new List<string>();
            foreach (string command in this.register.Keys)
            {
                if (command.ToUpper().StartsWith(commandStart.ToUpper()))
                    list.Add(command);
                else
                {
                    if (commandStart.ToUpper().StartsWith(command.ToUpper()))
                    {
                        // remove the command name and the dot
                        string functionStart = commandStart.ToUpper().Replace(command.ToUpper(), "").Substring(1);
                        GameConsoleObject consoleObject = this.register[command] as GameConsoleObject;
                        if (consoleObject != null)
                        {
                            list.AddRange(consoleObject.GetSuggestions(functionStart));
                        }
                    }
                }
            }
            list.Sort();
            return list.ToArray();
        }



        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string Execute(string command)
        {
            List<string> parameters = new List<string>();
            string[] quotes = command.Split('"');
            if (quotes.Length % 2 == 0)
            {
                return "ERROR: odd number of quotes are not allowed.";
            }

            for (int i = 0; i < quotes.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (!string.IsNullOrEmpty(quotes[i]))
                        parameters.AddRange(quotes[i].Trim().Split(' '));
                }
                else
                {
                    if (!string.IsNullOrEmpty(quotes[i]))
                        parameters.Add(quotes[i]);
                }
            }

            if (parameters.Count == 0)
                return "";

            string commandObject = parameters[0];
            parameters.Remove(parameters[0]);

            string objectName = "";
            string functionName = "";
            string[] commandObjectParts = commandObject.Split('.');
            if (commandObjectParts.Length > 0)
            {
                objectName = commandObjectParts[0];
            }
            if (commandObjectParts.Length > 1)
            {
                functionName = commandObjectParts[1];
            }

            if (!this.caseSensitive)
            {
                objectName = objectName.ToUpper();
                functionName = functionName.ToUpper();
            }


            // console objects
            if (register.ContainsKey(objectName))
            {
                GameConsoleObject consoleObject = register[objectName] as GameConsoleObject;
                if (consoleObject != null)
                {
                    return consoleObject.Execute(functionName, parameters.ToArray());
                }
            }

            // intern commands
            if (register.ContainsKey(objectName.ToUpper()))
            {
                IGameConsoleInternCommand consoleCommand = register[objectName.ToUpper()] as IGameConsoleInternCommand;
                if (consoleCommand != null)
                {
                    return consoleCommand.Execute(parameters.ToArray());
                }
            }
            return "\nERROR: Command not found. Type \"help\" for information.";
        }
    }
}