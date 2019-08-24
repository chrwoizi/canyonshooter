using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics.Lights
{
    class MovingPointLight : PointLight
    {
        private Transformable t;
        private Vector3 rotationAxis;
        private float rotationSpeed;
        private ICanyonShooterGame game;

        public MovingPointLight(ICanyonShooterGame game, Color color, Vector3 position, Vector3 offset, Vector3 rotationAxis, float rotationSpeed)
            : base(game, color)
        {
            this.game = game;
            t = new Transformable(game);

            LocalPosition = offset;

            Parent = t;
            t.LocalPosition = position;

            this.rotationAxis = rotationAxis;
            this.rotationSpeed = rotationSpeed;

            squaredAttenuation = 0.00001f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (game.World == null) return;
            t.Rotate(Quaternion.CreateFromAxisAngle(rotationAxis, rotationSpeed));
            if((GlobalPosition - game.World.Players[0].GlobalPosition).Length() > 2000)
            {
                game.World.RemovePointLight(this);
                Dispose();
            }
        }
    }
}
