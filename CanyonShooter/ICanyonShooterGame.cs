//
//
//  @ Project : CanyonShooter
//  @ File Name : ICanyonShooterGame.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.Engine;
using CanyonShooter.Engine.Audio;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Input;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework.Content;

namespace CanyonShooter
{
    /// <summary></summary>
    public interface ICanyonShooterGame
    {
        /// <summary>
        /// the config
        /// </summary>
        Config Config { get; }

        /// <summary>
        /// access to the content-pipeline
        /// </summary>
        ContentManager Content { get; }
        bool HasSpaceMouse { get; set; }
        /// <summary>
        /// access to the graphics-device
        /// </summary>
        IGraphics Graphics { get; }

        /// <summary>
        /// the world
        /// </summary>
        IWorld World { get; set; }
        
        /// <summary>
        /// the physics engine
        /// </summary>
        IPhysics Physics { get; }
        
        /// <summary>
        /// renderer
        /// </summary>
        IRenderer Renderer { get; }

        /// <summary>
        /// Gets the EffectsFactory.
        /// </summary>
        IEffectFactory Effects { get;}

        /// <summary>
        /// the hud
        /// </summary>
        //IHud Hud { get; }

        /// <summary>
        /// the menu
        /// </summary>
        //IMenu Menu { get; }
        
        /// <summary>
        /// the input devices
        /// </summary>
        IInput Input { get; }
     
        /// <summary>
        /// the Profil
        /// </summary>
        //Profil Profil { get; }

        /// <summary>
        /// the GameStates
        /// </summary>
        GameStates GameStates { get; }

        /// <summary>
        /// highscore list
        /// </summary>
        IHighscores Highscores { get; }

        ISoundSystem Sounds { get;}

        bool DebugMode { get; }

        void LoadLevel(string levelname);
    }
}
