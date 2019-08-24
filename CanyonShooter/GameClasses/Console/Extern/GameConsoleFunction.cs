using System;
using System.Collections.Generic;
using System.Reflection;

namespace CanyonShooter.GameClasses.Console.Extern
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    class GameConsoleFunction : IGameConsoleExternCommand
    {
        private GameConsoleObject owner;
        private string functionName;
        private List<MethodInfo> methods = new List<MethodInfo>();



        public GameConsoleFunction(GameConsoleObject owner, string functionName)
        {
            this.owner = owner;
            this.functionName = functionName;
        }



        public bool AddMethod(Type[] parameterTypes)
        {
            Type type = this.owner.SenderType;

            MethodInfo[] methods = type.GetMethods();

            // look for method with specified method name and parameter types
            foreach (MethodInfo method in methods)
            {
                if (method.Name == this.functionName)
                {
                    ParameterInfo[] paramInfos = method.GetParameters();
                    if (AreParameterTypesEqual(paramInfos, parameterTypes))
                    {
                        // add specified method
                        if (this.owner.Sender != null || method.IsStatic)
                            this.methods.Add(method);
                        return true;
                    }
                }
            }
            return false;
        }



        public bool AddMethods()
        {
            bool result = false;
            Type type = this.owner.SenderType;

            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.Name == this.functionName)
                {
                    if (this.owner.Sender != null || method.IsStatic)
                        this.methods.Add(method);
                    result = true;
                }
            }
            return result;
        }



        private static bool AreParameterTypesEqual(ParameterInfo[] p1, Type[] p2)
        {
            // check parameter count
            if (p1.Length != p2.Length)
                return false;

            // check all parameter types
            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i].ParameterType != p2[i])
                {
                    return false;
                }
            }
            return true;
        }



        #region IGameConsoleItem
        public string Info
        {
            get
            {
                return GetFunctionInformation();
            }
        }



        public string Execute(string[] parameters)
        {
            // Typ der Klasse bestimmen, der die Property gehört
            Type type = this.owner.SenderType;

            foreach (MethodInfo m in methods)
            {
                string result = "";
                if (TryMethod(m, parameters, out result))
                {
                    return result;
                }
            }

            return "ERROR: Command could not be executed. \n" + GetFunctionInformation();
        }



        public string[] GetSuggestions(string command)
        {
            return null;
        }
        #endregion



        private bool TryMethod(MethodInfo method, string[] parameters, out string result)
        {
            result = "";
            if (method == null)
                return false;

            // Parameter-Liste der Methode bestimmen
            ParameterInfo[] methodParams = method.GetParameters();
            if (methodParams.Length != parameters.Length)
                return false;

            List<object> parameterList = new List<object>();

            for (int i = 0; i < methodParams.Length; i++)
            {
                object param = GameConsoleTools.ParseString(
                    methodParams[i].ParameterType,
                    parameters[i],
                    this.owner.Owner.TypeConverterCollection);

                if (param == null)
                {
                    return false;
                }
                parameterList.Add(param);
            }

            Type senderType = this.owner.SenderType;

            object objectResult = senderType.InvokeMember(this.functionName,
                BindingFlags.InvokeMethod,
                null,
                this.owner.Sender,
                parameterList.ToArray());

            if (objectResult != null)
                result = objectResult.ToString();
            return true;
        }



        private string GetFunctionInformation()
        {
            string result = "";

            Type type = this.owner.SenderType;

            foreach (MethodInfo m in this.methods)
            {
                result += GetMethodInformation(m) + "\n";
            }
            return result.Trim();
        }



        private string GetMethodInformation(MethodInfo method)
        {
            string result = "\"" + this.owner.Name + "." + method.Name + " ";
            // Parameter-Liste der Methode bestimmen

            string infoAttribute = GameConsoleTools.GetGameConsoleInfoAttribute(
                Attribute.GetCustomAttributes(method));

            result += GameConsoleTools.GetParameterTypesString(method.GetParameters());
            return result + "\" : <" + GameConsoleTools.GetParameterTypeString(method.ReturnType) +
                "> (" + infoAttribute + ")";
        }
    }
}
