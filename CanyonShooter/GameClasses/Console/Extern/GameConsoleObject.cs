using System;
using System.Collections;
using System.Collections.Generic;

namespace CanyonShooter.GameClasses.Console.Extern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleObject
    {
        private Hashtable functions = new Hashtable();



        /// <summary>
        /// Initializes a new instance of the <see cref="GameConsoleObject"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="name">The name.</param>
        public GameConsoleObject(GameConsole owner, object sender, Type senderType, string name)
        {
            this.propOwner = owner;
            this.propSender = sender;
            this.propSenderType = senderType;
            this.propName = name;
        }



        #region Properties
        private string propName;
        private GameConsole propOwner;
        private object propSender;
        private Type propSenderType;



        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        public string Info
        {
            get
            {
                return this.Name + " : <" + this.SenderType + "> \n";
            }
        }


        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.propName;
            }
        }


        public GameConsole Owner
        {
            get
            {
                return this.propOwner;
            }
        }



        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public object Sender
        {
            get
            {
                return this.propSender;
            }
        }



        /// <summary>
        /// Gets the type of the sender.
        /// </summary>
        /// <value>The type of the sender.</value>
        public Type SenderType
        {
            get
            {
                return this.propSenderType;
            }
        }
        #endregion



        /// <summary>
        /// Gets the suggestions.
        /// </summary>
        /// <param name="commandStart">The command start.</param>
        /// <returns></returns>
        public string[] GetSuggestions(string functionStart)
        {
            List<string> list = new List<string>();
            foreach (string function in this.functions.Keys)
            {
                if (function.ToUpper().StartsWith(functionStart.ToUpper()))
                    list.Add(this.Name + "." + function);
            }
            list.Sort();
            return list.ToArray();
        }



        private GameConsoleFunction GetGameConsoleFunctions(string functionName)
        {
            GameConsoleFunction consoleFunction = null;

            // create new hashtable entry, if not yet included
            if (functions.ContainsKey(functionName))
            {
                consoleFunction = functions[functionName] as GameConsoleFunction;
            }
            else
            {
                consoleFunction = new GameConsoleFunction(this, functionName);
                functions.Add(functionName, consoleFunction);
            }

            return consoleFunction;
        }



        public bool RegisterProperty(string propertyName)
        {
            if (!functions.ContainsKey(propertyName))
                functions.Add(propertyName, new GameConsoleProperty(this, propertyName));
            return true;
        }



        public bool RegisterFunction(string functionName)
        {
            GameConsoleFunction functions = this.GetGameConsoleFunctions(functionName);
            if (!functions.AddMethods())
                throw new Exception("No function \"" + functions + "\" found.");
            return true;
        }



        public bool RegisterFunction(string functionName, Type[] parameterTypes)
        {
            GameConsoleFunction functions = this.GetGameConsoleFunctions(functionName);
            if (!functions.AddMethod(parameterTypes))
                throw new Exception("No function \"" + functions + "\" with specified parameter-types found.");
            return true;
        }



        public string Execute(string functionName, string[] parameters)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                if (parameters.Length == 1 && parameters[0] == "-?")
                {
                    return GetRegisteredFunctions();
                }
            }
            if (!functions.ContainsKey(functionName))
            {
                return "ERROR: function/property not registered";
            }

            IGameConsoleExternCommand item = functions[functionName] as IGameConsoleExternCommand;
            if (item != null)
                return item.Execute(parameters);
            else
                return "";
        }



        private string GetRegisteredFunctions()
        {
            string result = "\n";
            foreach (string key in functions.Keys)
            {
                IGameConsoleExternCommand command = functions[key] as IGameConsoleExternCommand;
                result += command.Info + "\n";
            }
            return result;
        }
    }
}
