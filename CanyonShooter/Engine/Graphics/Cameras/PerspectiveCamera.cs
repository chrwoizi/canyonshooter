using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics.Cameras
{
    class PerspectiveCamera : CameraBase, IPerspectiveCamera
    {
        #region Private Fields

        private float fov = 90.0f;

        #endregion


        public PerspectiveCamera(ICanyonShooterGame game)
            : base(game)
        {
        }

        protected override void MakeProjectionMatrix()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(fov), 
                (float)game.Graphics.Device.Viewport.Width/(float)game.Graphics.Device.Viewport.Height,
                nearClip, farClip);
        }

        #region IPerspectiveCamera Member

        public float Fov
        {
            get
            {
                return fov;
            }
            set
            {
                fov = value;
                MakeProjectionMatrix();
            }
        }

        #endregion
    }
}
