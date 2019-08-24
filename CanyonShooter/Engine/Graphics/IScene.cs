using CanyonShooter.Engine.Graphics.Cameras;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics
{
    public interface IScene
    {
        bool isEmpty { get; }
        ICamera Camera { get; set; }
        void AddDrawable(IDrawable d);
        void RemoveDrawable(IDrawable d);
        void Draw(GameTime gameTime);
    }
}
