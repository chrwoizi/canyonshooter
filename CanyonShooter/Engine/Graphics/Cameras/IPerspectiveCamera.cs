namespace CanyonShooter.Engine.Graphics.Cameras
{
    public interface IPerspectiveCamera : ICamera
    {
        /// <summary>
        /// field of view (horizontal frustum angle)
        /// </summary>
        float Fov { get; set; }
    }
}
