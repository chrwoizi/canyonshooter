using System;
using System.Collections.Generic;
using System.Reflection;

namespace CanyonShooter.GameClasses.Console.Extern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleProperty : IGameConsoleExternCommand
    {
        private GameConsoleObject owner;
        private string propertyName;



        /// <summary>
        /// Initializes a new instance of the <see cref="GameConsoleProperty"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="propertyName">Name of the property.</param>
        public GameConsoleProperty(GameConsoleObject owner, string propertyName)
        {
            this.owner = owner;
            this.propertyName = propertyName;
            this.Validate();
        }



        #region IGameConsoleItem
        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        public string Info
        {
            get
            {
                return GetPropertyInformation();
            }
        }



        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public string Execute(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                return this.Get();
            }
            if (parameters.Length > 0)
            {
                string parameter = string.Join(" ", parameters);
                return this.Set(parameter);
            }

            return "";
        }



        public string[] GetSuggestions(string command)
        {
            return null;

        }
        #endregion



        private string GetPropertyInformation()
        {
            Type type = this.owner.SenderType;

            // PropertyInfo holen
            PropertyInfo prop = type.GetProperty(propertyName);

            // Get-Set-Methode holen
            MethodInfo method = prop.GetGetMethod();

            string result = "";
            if (method != null)
            {
                result += "\"" + this.owner.Name + "." + this.propertyName + "\" : <" +
                    GameConsoleTools.GetParameterTypeString(method.ReturnType) + ">" +
                    GameConsoleTools.GetGameConsoleInfoAttribute(prop);
            }

            method = prop.GetSetMethod();
            if (method != null)
            {
                if (!string.IsNullOrEmpty(result))
                    result += "\n";
                result += "\"" + this.owner.Name + "." + this.propertyName + " ";
                // Parameter-Liste der Methode bestimmen
                ParameterInfo[] methodParams = method.GetParameters();

                result += GameConsoleTools.GetParameterTypesString(methodParams);
                /*
                for (int i = 0; i < methodParams.Length; i++)
                {
                    result += "<" + methodParams[i].ParameterType.ToString() + ">";
                    if (i < methodParams.Length - 1)
                        result += " ";
                }
                 */
                result += "\" <" + method.ReturnType.ToString() + ">" +
                    GameConsoleTools.GetGameConsoleInfoAttribute(Attribute.GetCustomAttributes(prop));
            }

            return result;
        }



        private string Get()
        {
            // Typ der Klasse bestimmen, der die Property gehört
            Type type = this.owner.SenderType;

            // PropertyInfo holen
            PropertyInfo prop = type.GetProperty(propertyName);

            // Get-Set-Methode holen
            MethodInfo method = prop.GetGetMethod();

            List<object> parameterList = new List<object>();

            object result = type.InvokeMember(
                propertyName,
                BindingFlags.GetProperty,
                null,
                this.owner.Sender,
                parameterList.ToArray());

            if (result != null)
                return result.ToString();
            else
                return "<null>";
        }



        private string Set(string value)
        {
            // Typ der Klasse bestimmen, der die Property gehört
            Type type = this.owner.SenderType;

            // PropertyInfo holen
            PropertyInfo prop = type.GetProperty(propertyName);

            // Get-Set-Methode holen
            MethodInfo method = prop.GetSetMethod();

            if (method == null)
                return "ERROR: Property is read-only.";

            // Parameter-Liste der Methode bestimmen
            ParameterInfo[] methodParams = method.GetParameters();
            if (methodParams.Length < 1)
                return "ERROR: Invalid property parameters.";

            List<object> parameterList = new List<object>();

            object parameter = GameConsoleTools.ParseString(
                methodParams[0].ParameterType,
                value,
                this.owner.Owner.TypeConverterCollection);

            if (parameter == null)
                return "ERROR: Could not parse set-value.";
            parameterList.Add(parameter);

            type.InvokeMember(
                propertyName,
                BindingFlags.SetProperty,
                null,
                this.owner.Sender,
                parameterList.ToArray());

            return this.Get();
        }



        private bool Validate()
        {
            Type type = this.owner.SenderType;

            // PropertyInfo holen
            PropertyInfo prop = type.GetProperty(propertyName);

            if (prop == null)
                throw new Exception("Property \"" + this.propertyName + "\" of object \"" +
                    this.owner.Name + "\" not found.");

            // Get-Set-Methode holen

            if (prop.GetSetMethod() == null && prop.GetGetMethod() == null)
                throw new Exception("Property \"" + this.propertyName + "\" of object \"" +
                    this.owner.Name + "\" not found.");

            return true;
        }
    }
}
