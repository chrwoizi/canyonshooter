using CanyonShooter.Engine.Graphics.Cameras;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics.Lights
{
    /// <summary>
    /// base of all lights
    /// </summary>
    public interface ILight : IGameComponent
    {
        /// <summary>
        /// Returns the relevance to camera. A lower value means a higher relevance.
        /// For PointLights it could be the distance to the camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns>any value in [0;infinity)</returns>
        float Relevance(ICamera camera);

        /// <summary>
        /// switches the light on
        /// </summary>
        void On();

        /// <summary>
        /// switches the light off
        /// </summary>
        void Off();
    }
}
