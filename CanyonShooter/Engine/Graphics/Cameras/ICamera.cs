using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics.Cameras
{
    public interface ICamera : IGameComponent, IUpdateable, ITransformable
    {
        /// <summary>
        /// view matrix
        /// </summary>
        Matrix ViewMatrix { get; }

        /// <summary>
        /// projection matrix
        /// </summary>
        Matrix ProjectionMatrix { get; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <value>The direction.</value>
        Vector3 Direction { get; }

        /// <summary>
        /// near clip plane
        /// </summary>
        float NearClip { get; set; }

        /// <summary>
        /// far clip plane
        /// </summary>
        float FarClip { get; set; }

        /// <summary>
        /// call this method when the device has changed.
        /// </summary>
        void OnDeviceChanged();
    }
}
