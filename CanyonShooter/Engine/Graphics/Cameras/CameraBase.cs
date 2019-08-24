using System;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using CanyonShooter.GameClasses.Console;

namespace CanyonShooter.Engine.Graphics.Cameras
{
    abstract class CameraBase : Transformable, ICamera
    {
        public readonly Vector3 FRONT = new Vector3(0, 0, -1);
        public readonly Vector3 RIGHT = new Vector3(1, 0, 0);
        public readonly Vector3 UP = new Vector3(0, 1, 0);


        #region Protected Fields

        protected ICanyonShooterGame game;

        protected Matrix viewMatrix = Matrix.Identity;
        protected Matrix projectionMatrix = Matrix.Identity;

        protected float nearClip = 1.0f;
        protected float farClip = 10000.0f;

        protected int updateOrder = (int)UpdateOrderType.Camera;
        protected bool enabled = false;

        #endregion


        public CameraBase(ICanyonShooterGame game)
            : base(game, null)
        {
            this.game = game;
        }

        protected override void OnTransform()
        {
            MakeViewMatrix();
        }

        private void MakeViewMatrix()
        {
            Vector3 absoluteFront = Vector3.TransformNormal(FRONT, GlobalTransformation);
            Vector3 absoluteUp = Vector3.TransformNormal(UP, GlobalTransformation);

            viewMatrix = Matrix.CreateLookAt(GlobalPosition, GlobalPosition + absoluteFront, absoluteUp);
        }

        protected abstract void MakeProjectionMatrix();


        #region IUpdatable Member

        public void Initialize()
        {
            MakeViewMatrix();
            MakeProjectionMatrix();
        }

        public virtual void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                if (EnabledChanged != null) EnabledChanged.Invoke(null, null);
            }
        }

        public int UpdateOrder
        {
            get
            {
                return updateOrder;
            }
            set
            {
                updateOrder = value;
                if(UpdateOrderChanged != null) UpdateOrderChanged.Invoke(null, null);
            }
        }

        public event EventHandler EnabledChanged;
        public event EventHandler UpdateOrderChanged;

        #endregion

        public void ZoomIn(float zoomLength)
        {
            if (LocalPosition.X != float.NaN)
            {
                    LocalPosition += this.Direction * zoomLength;
                    GraphicalConsole.GetSingleton(game).WriteLine("Camera ZoomOut - Position:" + LocalPosition.ToString(), 0);
            }
        }

        public void ZoomOut(float zoomLength)
        {
            if (LocalPosition.X != float.NaN)
            {
                LocalPosition -= this.Direction * zoomLength;
                GraphicalConsole.GetSingleton(game).WriteLine("Camera ZoomOut - Position:" + LocalPosition.ToString(), 0);
            }
        }

        #region ICamera Member

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public Vector3 Direction
        {
            get { return Vector3.TransformNormal(FRONT, GlobalTransformation); }
        }

        public float NearClip
        {
            get
            {
                return nearClip;
            }
            set
            {
                nearClip = value;
                MakeProjectionMatrix();
            }
        }

        public float FarClip
        {
            get
            {
                return farClip;
            }
            set
            {
                farClip = value;
                MakeProjectionMatrix();
            }
        }

        public void OnDeviceChanged()
        {
            MakeProjectionMatrix();
        }

        #endregion
    }
}
