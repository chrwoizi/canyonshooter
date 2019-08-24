#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using CanyonShooter.Engine;
using CanyonShooter.Engine.Graphics.Effects;
using DescriptionLibs.ParticleEffect;
using Microsoft.Xna.Framework;

#endregion

namespace CanyonShooter.ParticleEngine
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    public class SmokePlumeEmitter : Effect
    {
        public SmokePlumeEmitter(ICanyonShooterGame game, EffectType type, ParticleSettings settings)
            : base(game, type, settings)
        {
            this.game = game;
        }

        private ICanyonShooterGame game;
        protected override void InitializeSettings(ParticleSettings settings)
        {
            DrawOrder = (int)DrawOrderType.Particles;
            //settings.TextureName = "Content/Textures/Particles/smoke";
            //settings.MaxParticles = 600;
            //settings.Duration = TimeSpan.FromSeconds(10);
            //settings.MinHorizontalVelocity = 0;
            //settings.MaxHorizontalVelocity = 15;
            //settings.MinVerticalVelocity = 10;
            //settings.MaxVerticalVelocity = 20;
            //settings.Gravity = new Vector3(-20, -2, 0);
            //settings.EndVelocity = 0.75f;
            //settings.MinRotateSpeed = -1;
            //settings.MaxRotateSpeed = 1;
            //settings.MinStartSize = 5;
            //settings.MaxStartSize = 10;
            //settings.MinEndSize = 50;
            //settings.MaxEndSize = 200;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (play && destroyTime > DateTime.Now)
            {
                this.AddParticle(this.GlobalPosition, Vector3.Zero);
                SetCamera(game.Renderer.Camera.ViewMatrix, game.Renderer.Camera.ProjectionMatrix);
            }
            else
            {
                if (destroy)
                    Dispose();
            }
            
        }

        private bool play = false;
        private bool destroy = false;
        private DateTime destroyTime = DateTime.Now;
  
        /// Plays the effect for the specified duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="destroy">if set to <c>true</c> destroy it.</param>
        public override void Play(TimeSpan duration, bool destroy)
        {
            play = true;
            destroyTime = DateTime.Now.Add(duration);
            this.destroy = destroy;
        }

        /// <summary>
        /// Plays this instance and destroys it after 5 seconds
        /// </summary>
        public override void Play()
        {
            Play(new TimeSpan(0, 0, 0, 5), true);
        }

    }
}
