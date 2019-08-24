using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics.Cameras
{
    class FreePerspectiveCamera : PerspectiveCamera
    {
        #region Private Data Members

        private Vector3 speed = new Vector3(500,500,500);

        #endregion


        public FreePerspectiveCamera(ICanyonShooterGame game) : base(game)
        {
            this.ConnectedToXpa = true;
            this.InfluencedByGravity = false;
            this.Enabled = true;
        }

        #region ICamera Member

        public Vector3 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // free camera movement. calculate velocities
            // forward,backward,left,right
            Vector3 velocityXZ = new Vector3(0, 0, 0);
            if (game.GameStates.InputFocus == this)
            {
                if (game.Input.IsKeyDown("Camera.Forward"))
                {
                    velocityXZ += new Vector3(0, 0, -Speed.Z);
                }
                if (game.Input.IsKeyDown("Camera.Backward"))
                {
                    velocityXZ += new Vector3(0, 0, Speed.Z);
                }
                if (game.Input.IsKeyDown("Camera.Left"))
                {
                    velocityXZ += new Vector3(-Speed.X, 0, 0);
                }
                if (game.Input.IsKeyDown("Camera.Right"))
                {
                    velocityXZ += new Vector3(Speed.X, 0, 0);
                }
                velocityXZ = Vector3.Transform(velocityXZ, LocalRotation);

                // up,down
                Vector3 velocityY = Vector3.Zero;
                if (game.Input.IsKeyDown("Camera.Up"))
                {
                    velocityY += new Vector3(0, Speed.Y, 0);
                }
                if (game.Input.IsKeyDown("Camera.Down"))
                {
                    velocityY += new Vector3(0, -Speed.Y, 0);
                }

                // combine velocities
                Vector3 velocity = velocityY + velocityXZ;

                // set velocity
                this.Velocity = velocity;

                // rotation
                Vector3 up = Vector3.Transform(UP, Quaternion.Conjugate(LocalRotation));
                Quaternion horizontal = Quaternion.CreateFromAxisAngle(up, -0.004f * game.Input.MouseMovement.X);
                Quaternion vertical = Quaternion.CreateFromAxisAngle(RIGHT, -0.004f * game.Input.MouseMovement.Y);
                Rotate(Quaternion.Concatenate(vertical, horizontal));
            }
        }
        
        #endregion
    }
}
