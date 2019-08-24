namespace CanyonShooter.Engine.Graphics.Cameras
{
    public interface IOrthographicCamera : ICamera
    {
        int Width { get; set; }
        int Height { get; set; }
    }
}
