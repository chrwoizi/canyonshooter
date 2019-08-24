using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Physics;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Tests
{
    class TestAudio3DEmitter : Transformable 
    {
        private ISoundSystem system;
        private ISound fireSound;

        /// <summary>
        /// Creates a simple transformable sound emitter. 
        /// </summary>
        /// <param name="soundSystem"></param>
        /// <param name="soundName"></param>
        public TestAudio3DEmitter(ISoundSystem soundSystem, string soundName)
            : base(null)
        {
            system = soundSystem;
            fireSound = system.CreateSound(soundName);
            fireSound.Parent = this;
        }

        /// <summary>
        /// Emit the loaded sound.
        /// </summary>
        /// <param name="looped"></param>
        public void Emit(bool looped) 
        {
            fireSound.Loop = looped;
            fireSound.Play();
        }

        /// <summary>
        /// Transforms the emitter.
        /// </summary>
        protected override void OnTransform()
        { 
            // Transform this emitter. 
        } 
    }
}
