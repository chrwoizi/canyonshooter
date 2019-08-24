
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
    public class RocketSmokeEmitter : Effect
    {
        public RocketSmokeEmitter(ICanyonShooterGame game, EffectType type, ParticleSettings settings)
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
            if (play && (destroyTime > DateTime.Now) || endlessPlay)
            {
                this.AddParticle(this.GlobalPosition,
                                 Vector3.Transform(new Vector3(0, 0, 15), GlobalRotation));
                SetCamera(game.Renderer.Camera.ViewMatrix, game.Renderer.Camera.ProjectionMatrix);
            }
            else
            {
                if(destroy)
                    this.Dispose();
            }
            
        }

        private bool play = false;
        private bool destroy = false;

        private bool endlessPlay = false;
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
        /// Plays this instance endlessly
        /// </summary>
        public override void Play()
        {
            endlessPlay = true;
            play = true;

        }
    }
}
