using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Tests
{
    class TestAudioPlayback
    {
        // Here go the sound affairs
        private ISoundSystem sounds;
        private ISound sndFire;
        private ISound sndStop;
        private ISound sndStart;
        private ISound sndAmmo;
        private ISound sndReload;

        /// <summary>
        /// Main constructor of Test Frame.
        /// </summary>
        public TestAudioPlayback() { }

        /// <summary>
        /// Setup the test environment.
        /// </summary>
        /// <param name="game"></param>
        public void SetUp(ICanyonShooterGame game) 
        {
            // Create sound system
            sounds = new SoundSystem(game);

            // Load all sounds
            sndFire = sounds.CreateSound("MFireLoop");
            sndStop = sounds.CreateSound("MFireStop");
            sndStart = sounds.CreateSound("MFireStart");
            sndAmmo = sounds.CreateSound("ReleaseMines");
            sndReload = sounds.CreateSound("Ammo");

            // Set global volumes
            sounds.EffectVolume = 0.8f;
        }

        /// <summary>
        /// Test1: Minigun playback simulation from FSM.
        /// </summary>
        public void TestMinigunPlayback() 
        { 
            sndStart.Play();
            while (sndStart.Playing()) { }
            sndFire.Play();
            while (sndFire.Playing()) { }
            sndStop.Play();
            while (sndStop.Playing()) { }

            sndStart.Play();
            while (sndStart.Playing()) { }
            sndFire.Play();
            while (sndFire.Playing()) { }

            // No more ammo
            sndAmmo.Play();
            while (sndAmmo.Playing()) { }
            // Stop minigun
            sndStop.Play();
            while (sndStop.Playing()) { }

            // Reload ammo
            sndReload.Play();
            while (sndReload.Playing()) { }

            // More fun ....
            sndStart.Play();
            while (sndStart.Playing()) { }
            sndFire.Play();
            while (sndFire.Playing()) { }
            sndStop.Play();
            while (sndStop.Playing()) { }
        }

        /// <summary>
        /// Destroy the test environment.
        /// </summary>
        public void TearDown() 
        {
            sounds.Shutdown();
        }
    }
}
