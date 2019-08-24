//
//
//  @ Project : CanyonShooter
//  @ File Name : IMenu.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework;

namespace CanyonShooter.Menus
{
    /// <summary></summary>
    public interface IMenu : IGameComponent, IDrawable
    {
        /// <summary>
        /// the name of the file from which this has been loaded
        /// </summary>
        string Name { get; }
       
        Vector2 Mouse { get; }

        bool Action { get; set; }

        IMainList Shifts { get; set;}

        string CurrentMenu { get; set;}

        Button CurrentButton { get;}
        ButtonList CurrentList { get;}

        Rectangle MouseInRec { get;}

        //PromptCollection PromptsCollection { get; }

        Vector2 MousePosition {  get; }

        void load();

        void addPrompt(Prompt prompt);

        void removePrompt();

    }
}
