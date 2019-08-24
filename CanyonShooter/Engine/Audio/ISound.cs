using CanyonShooter.Engine.Physics;

namespace CanyonShooter.Engine.Audio
{
    /// <summary>
    /// This interface describes the sound object.
    /// @author: M.Rodriguez
    /// @date: 02.12.2007
    /// </summary>
    public interface ISound : ITransformable
    {
        /// <summary>
        /// Each sound has a volume property.
        /// </summary>
        float Volume
        {
            get;
            set;
        }

        /// <summary>
        /// The unique sound name property.
        /// </summary>
        string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Property to set loopable sound.
        /// </summary>
        bool Loop
        {
            get;
            set;
        }

        /// <summary>
        /// The sound type of this instance.
        /// </summary>
        SoundType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Play this sound now.
        /// </summary>
        void Play();

        /// <summary>
        /// Directly stop the playback.
        /// </summary>
        void Stop();

        /// <summary>
        /// Pause the current sound playback.
        /// </summary>
        void Pause();

        /// <summary>
        /// Get the current sound playback status.
        /// </summary>
        /// <returns>True if the instance plays a sound.</returns>
        bool Playing();
    }
}
