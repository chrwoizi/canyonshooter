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
    public class LaserWallEmitter : Effect
    {
        public LaserWallEmitter(ICanyonShooterGame game, EffectType type, ParticleSettings settings)
            : base(game, type, settings)
        {
            this.game = game;
        }

        private ICanyonShooterGame game;
        protected override void InitializeSettings(ParticleSettings settings)
        {
            DrawOrder = (int)DrawOrderType.Particles;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (play && destroyTime > DateTime.Now)
            {
                this.AddParticle(this.GlobalPosition,
                                 Vector3.Transform(new Vector3(0, 0, 1), GlobalRotation));
                SetCamera(game.Renderer.Camera.ViewMatrix, game.Renderer.Camera.ProjectionMatrix);
            }
            else
            {
                if(destroy)
                    Dispose();
            }
        }

        private bool play = false;
        private bool destroy = false;
        private DateTime destroyTime = DateTime.Now;
        /// <summary>
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
            Play(new TimeSpan(0,0,0,5),true);
        }


    }

    //public class LaserWallDescription : EffectDescription
    //{
    //    public LaserWallDescription()
    //        : base()
    //    {
    //        TextureName = "Content/Textures/Particles/LaserWall";

    //        MaxParticles = 200;
    //        Duration = TimeSpan.FromSeconds(1f);
    //        MinHorizontalVelocity = 0;
    //        MaxHorizontalVelocity = 0;
    //        MinVerticalVelocity = 0;
    //        MaxVerticalVelocity = 0;
                        
    //        Gravity = new Vector3(0, 0, 0);
    //        EndVelocity = 5f;
    //        MinRotateSpeed = -5;
    //        MaxRotateSpeed = 7;
    //        MinStartSize = 10;
    //        MaxStartSize = 10;
    //        MinEndSize = 10;
    //        MaxEndSize = 10;
    //    }
    //}
}
