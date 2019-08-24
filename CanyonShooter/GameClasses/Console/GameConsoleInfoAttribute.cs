using System;

namespace CanyonShooter.GameClasses.Console
{
    public class GameConsoleInfoAttribute : Attribute
    {
        private string info;



        /// <summary>
        /// Initializes a new instance of the <see cref="GameConsoleInfoAttribute"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        public GameConsoleInfoAttribute(string info)
        {
            this.info = info;
        }



        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        public string Info
        {
            get
            {
                return this.info;
            }
        }
    }
}
