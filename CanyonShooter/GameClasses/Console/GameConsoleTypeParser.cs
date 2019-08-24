using System;
using System.Collections.Generic;

namespace CanyonShooter.GameClasses.Console
{
    /// <summary>
    /// @owner: Markus Lorenz
    /// </summary>
    public abstract class GameConsoleTypeParser
    {
        /// <summary>
        /// Determines whether this instance can parse a string to the specified destination type.
        /// </summary>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can parse a string to the specified destination type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanParse(Type destinationType);



        /// <summary>
        /// Parses the specified value-string to destination type.
        /// </summary>
        /// <param name="value">The value-string.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public abstract object Parse(string value, Type destinationType);
    }



    public class GameConsoleTypeParserCollection : List<GameConsoleTypeParser>
    {
    }
}
