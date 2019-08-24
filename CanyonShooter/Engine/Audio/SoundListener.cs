using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Audio
{
    /// <summary>
    /// This class implements the listener of all sounds.
    /// @author: M.Rodriguez
    /// @date: 02.12.2007
    /// </summary>
    public class SoundListener : Transformable
    {
        private Vector3 position;

        public SoundListener(ICanyonShooterGame game)
            : base(game)
        {
            
        }

        /// <summary>
        /// The main position of the sound listener.
        /// </summary>
        public Vector3 ListenerPosition
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Transform the position of the listener.
        /// </summary>
        protected override void OnTransform()
        {
            position = GlobalPosition;
        }
    }
}
