using System;

namespace CanyonShooter.GameClasses.Console
{
    interface IGameConsole
    {
        /// <summary>
        /// Registers a function of an object and all its overloads.
        /// </summary>
        /// <param name="functionOwner">The object, that owns the specified function.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <returns></returns>
        bool RegisterObjectFunction(object functionOwner, string objectName, string functionName);



        /// <summary>
        /// Register a function of an object, specified by the parameter-types.
        /// </summary>
        /// <param name="functionOwner">The object, that owns the specified function.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <param name="parameterTypes">The parameter types of the specified function in correct order.</param>
        /// <returns></returns>
        bool RegisterObjectFunction(object functionOwner, string objectName, string functionName, Type[] parameterTypes);



        /// <summary>
        /// Registers a property of an object.
        /// </summary>
        /// <param name="propertyOwner">The object, that owns the specified property.</param>
        /// <param name="objectName">Name of the object, that will be used in console.</param>
        /// <param name="propertyName">Name of the property as declared in its class.</param>
        /// <returns></returns>
        bool RegisterObjectProperty(object propertyOwner, string objectName, string propertyName);



        /// <summary>
        /// Registers a static function of a class and all its overloads.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified function.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <returns></returns>
        bool RegisterStaticFunction(Type classType, string className, string functionName);



        /// <summary>
        /// Registers a static function of a class and all its overloads.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified function.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="functionName">Name of the function as declared in its class.</param>
        /// <param name="parameterTypes">The parameter types of the specified function in correct order.</param>
        /// <returns></returns>
        bool RegisterStaticFunction(Type classType, string className, string functionName, Type[] parameterTypes);



        /// <summary>
        /// Registers the static property.
        /// </summary>
        /// <param name="classType">Type of the class, that owns the specified property.</param>
        /// <param name="className">Name of the class, that will be used in console.</param>
        /// <param name="propertyName">Name of the property as declared in its class.</param>
        /// <returns></returns>
        bool RegisterStaticProperty(Type classType, string className, string propertyName);



        /// <summary>
        /// Registers the type parser to parse console parameters.
        /// </summary>
        /// <param name="typeParser">The type parser.</param>
        /// <returns></returns>
        bool RegisterTypeParser(GameConsoleTypeParser typeParser);
    }
}