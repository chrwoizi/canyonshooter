using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Audio
{
    /// <summary>
    /// This interface describes the Sound Manager.
    /// @author: M.Rodriguez
    /// @date: 02.12.2007
    /// </summary>
    public interface ISoundSystem : IGameComponent
    {
        /// <summary>
        /// Property of the main music volume.
        /// </summary>
        float MusicVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Property of the effect music volume.
        /// </summary>
        float EffectVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the current musicbox track name.
        /// </summary>
        /// <returns></returns>
        string GetMusicName();

        /// <summary>
        /// The sound listener property.
        /// </summary>
        SoundListener SoundListener
        {
            get;
            set;
        }
        
        /// <summary>
        /// Turn on the music box and play randomized
        /// background music.
        /// </summary>
        /// <param name="status">The new music box status</param>
        void MusicBox(MusicBoxStatus status);

        /// <summary>
        /// Unload all music content.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Stop the playback of all sounds imidiatly.
        /// </summary>
        void AllStop();

        /// <summary>
        /// Update the sound engine playbacks.
        /// </summary>
        void Update();

        /// <summary>
        /// Get the sound system playback status.
        /// </summary>
        /// <returns>True if a sound is playing.</returns>
        bool Playing();

        /// <summary>
        /// Create new sounds with this Sound Factory Method. The
        /// new sound will appear in the soundqueue.
        /// Choose one of the follwing effects:
        /// Lasergun, LasergunMiddle, LasergunSlow, LasergunHigh,
        /// Thunder, RocketLaunch, ReleaseMines, Heli, Desruptor
        /// Clock, Explosion, Ammo, Beam, Phaser, UFO, UFOLoop, 
        /// UFOStop, UFOStart, HeliAlternative, MFireLoop, MFireStart,
        /// MFireStop, Malt, MSelect, GodMode, Alert, GameOver,
        /// ExtraLife, Button
        /// <param name="soundName">The unique identification name of a sound.</param>
        /// <returns>An instance of the sound object.</returns>
        ISound CreateSound(string soundName);

        /// <summary>
        /// Removes the sound from the list. Called by ISound.Dispose
        /// </summary>
        /// <param name="sound"></param>
        void RemoveSoundReference(ISound sound);
    }
}
