//
//
//  @ Project : CanyonShooter
//  @ File Name : IEffect.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using System;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using DescriptionLibs.ParticleEffect;

namespace CanyonShooter.Engine.Graphics.Effects
{
    /// <summary></summary>
    public enum EffectType
    {
        ROCKET_SMOKE,
        BLASTPIPE,
        SMOKE_PLUME,
        EXPLOSION,
        LASER_WALL,
    };

    /// <summary></summary>
    public interface IEffect : ITransformable, IGameComponent, IDrawable
    {
        /// <summary>
        /// type of effect.
        /// one of the EFFECT_TYPE constants
        /// </summary>
        EffectType Type { get; }

        /// <summary>
        /// starts the animation for the specified duration
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="destroy">if set to <c>true</c> the Effect is detroyed after the duration.</param>
        void Play(TimeSpan duration, bool destroy);

        void Play();

        /// <summary>
        /// Called when this collides with another GameObject
        /// </summary>
        /// <param name="other">the other object</param>
        void OnCollision(IGameObject other);

        ParticleSettings Settings { get;}
    }
}
