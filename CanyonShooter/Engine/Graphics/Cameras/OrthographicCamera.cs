using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics.Cameras
{
    class OrthographicCamera : CameraBase, IOrthographicCamera
    {
        #region Private Fields

        private int width = 100;
        private int height = 100;

        #endregion


        public OrthographicCamera(ICanyonShooterGame game, int width, int height)
            : base(game)
        {
            this.width = width;
            this.height = height;
            MakeProjectionMatrix();
        }

        protected override void MakeProjectionMatrix()
        {
            projectionMatrix = Matrix.CreateOrthographic(width, height, nearClip, farClip);
        }

        #region IOrthogonalCamera Members

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                MakeProjectionMatrix();
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                MakeProjectionMatrix();
            }
        }

        #endregion
    }
}
