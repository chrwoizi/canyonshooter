//
//
//  @ Project : CanyonShooter
//  @ File Name : IWorld.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using System;
using System.Collections.ObjectModel;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses;
using CanyonShooter.GameClasses.World.Canyon;
using CanyonShooter.GameClasses.World.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.World
{
    /// <summary></summary>
    public interface IWorld : IGameComponent, IDisposable
    {
        /// <summary>
        /// the level name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// the players
        /// </summary>
        ReadOnlyCollection<IPlayer> Players { get; }

        Finish Finish { get; set; }

        /// <summary>
        /// the canyon
        /// </summary>
        ICanyon Canyon { get; }

        /// <summary>
        /// the level
        /// </summary>
        Level Level { get; }

        /// <summary>
        /// the sky
        /// </summary>
        ISky Sky { get; }

        /// <summary>
        /// the ambient light (light from everywhere due to scattering and reflection)
        /// </summary>
        Color AmbientLight { get; set; }

        /// <summary>
        /// the items in the world
        /// </summary>
        ReadOnlyCollection<IItem> Items { get; }

        /// <summary>
        /// the static objects
        /// </summary>
        ReadOnlyCollection<IStatic> Statics { get; }

        /// <summary>
        /// effects
        /// </summary>
        ReadOnlyCollection<IEffect> Effects { get; }

        /// <summary>
        /// the enemies
        /// </summary>
        ReadOnlyCollection<IEnemy> Enemies { get; }

        /// <summary>
        /// the point lights sorted by relevance to current camera
        /// </summary>
        ReadOnlyCollection<IPointLight> PointLights { get; }

        /// <summary>
        /// Adds light to this
        /// </summary>
        /// <param name="light"></param>
        void AddPointLight(IPointLight light);

        /// <summary>
        /// Removes light from this
        /// </summary>
        /// <param name="light"></param>
        void RemovePointLight(IPointLight light);

        /// <summary>
        /// Adds obj to this
        /// </summary>
        /// <param name="obj">can be any type of object (item, static, effect, enemies, projectile, etc.)</param>
        void AddObject(IGameObject obj);

        /// <summary>
        /// Removes obj from this. Only GameObject may call this method.
        /// </summary>
        /// <param name="obj">can be any type of object (item, static, effect, enemies, projectile, etc.)</param>
        void RemoveObject(IGameObject obj);

        /// <summary>
        /// Adds an effect.
        /// </summary>
        /// <param name="fx">The fx.</param>
        void AddEffect(IEffect fx);

        /// <summary>
        /// Removes an effect.
        /// </summary>
        /// <param name="fx">The fx.</param>
        void RemoveEffect(IEffect fx);
    }
}
