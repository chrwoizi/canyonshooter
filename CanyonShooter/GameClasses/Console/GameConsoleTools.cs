using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CanyonShooter.GameClasses.Console
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    static class GameConsoleTools
    {
        private static Dictionary<string, string> parameterReplaces = new Dictionary<string, string>();



        static GameConsoleTools()
        {
            parameterReplaces.Add("System.String", "String");
            parameterReplaces.Add("System.Int32", "Int");
            parameterReplaces.Add("System.Bool", "Bool");
            parameterReplaces.Add("System.Double", "Double");
            parameterReplaces.Add("System.Float", "Float");
            parameterReplaces.Add("System.Void", "Void");
        }



        public static object ParseString(Type type, string value, GameConsoleTypeParserCollection typeParserCollection)
        {
            if (value == null)
                return null;

            value = value.Trim();

            if (type == typeof(string))
            {
                return value;
            }

            if (type == typeof(int))
            {
                int result;
                if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.GetCultureInfo("en-US"), out result))
                    return null;
                return result;
            }
            if (type == typeof(double))
            {
                double result;
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out result))
                    return null;
                return result;
            }
            if (type == typeof(float))
            {
                float result;
                if (!float.TryParse(value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out result))
                    return null;
                return result;
            }
            if (type == typeof(bool))
            {
                if (value == "0")
                    return false;
                if (value == "1")
                    return true;

                bool result;
                if (!bool.TryParse(value, out result))
                    return null;
                return result;
            }
            if (type.IsEnum)
            {
                object result = null;
                try
                {
                    result = Enum.Parse(type, value, true);
                }
                catch
                {
                    // ignore exception.
                }
                return result;
            }

            foreach (GameConsoleTypeParser parser in typeParserCollection)
            {
                if (parser.CanParse(type))
                {
                    return parser.Parse(value, type);
                }
            }
            return null;
        }



        public static string GetGameConsoleInfoAttribute(Attribute[] attributes)
        {
            foreach (Attribute attr in attributes)
            {
                // Check for the AnimalType attribute.
                GameConsoleInfoAttribute infoAttribute = attr as GameConsoleInfoAttribute;
                if (infoAttribute != null)
                {
                    return infoAttribute.Info;
                }
            }
            return "";
        }



        public static string GetGameConsoleInfoAttribute(MethodInfo method)
        {
            return GetGameConsoleInfoAttribute(Attribute.GetCustomAttributes(method));
        }




        public static string GetGameConsoleInfoAttribute(PropertyInfo propertyInfot)
        {
            return GetGameConsoleInfoAttribute(Attribute.GetCustomAttributes(propertyInfot));
        }



        public static string GetParameterTypeString(Type type)
        {
            string result = type.ToString();
            foreach (string key in parameterReplaces.Keys)
            {
                result = result.Replace(key, parameterReplaces[key]);
            }
            return result;
        }



        public static string GetParameterTypesString(ParameterInfo[] methodParams)
        {
            string result = "";

            for (int i = 0; i < methodParams.Length; i++)
            {
                result += "<" + GetParameterTypeString(methodParams[i].ParameterType) + ">";
                if (i < methodParams.Length - 1)
                    result += " ";
            }
            return result;
        }
    }
}
