using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Tests
{
    class TestAudio3D
    {
        // Here go the sound affairs
        private ISoundSystem sounds;
        private TestAudio3DEmitter emitter;
        private SoundListener listener;
        private static Random rnd = new Random();

        /// <summary>
        /// Main constructor of Test Frame.
        /// </summary>
        public TestAudio3D() { }

        /// <summary>
        /// Setup the test environment.
        /// </summary>
        /// <param name="game"></param>
        public void SetUp(ICanyonShooterGame game)
        {
            // Create sound system
            sounds = new SoundSystem(game);

            // Create the listener
            listener = new SoundListener(game);

            // Set global volumes
            sounds.EffectVolume = 0.8f;

            // The listener "player"
            sounds.SoundListener = listener;
        }

        /// <summary>
        /// Test1: Playback 3D Sound.
        /// </summary>
        public void TestPlayback3D()
        {
            emitter = new TestAudio3DEmitter(sounds, "MFireLoop");
            emitter.Emit(true);
            for (int i = 0; i < 1000; i++)
            {
                float dx = (float)-Math.Cos(rnd.Next(0, 6));
                float dz = (float)-Math.Sin(rnd.Next(0, 6));
                emitter.LocalPosition = new Vector3(dx, 0, dz);
                System.Threading.Thread.Sleep(1000);
                sounds.Update();
            }
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
