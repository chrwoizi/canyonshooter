using System;
using CanyonShooter.Engine.Graphics.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics.Lights
{
    /// <summary>
    /// Color and direction of a sunlight
    /// </summary>
    class Sunlight : GameComponent, ISunlight
    {
        private ICanyonShooterGame game;
        private Color color = new Color(80, 70, 70);
        private Vector3 direction = new Vector3(0, -1, 0);
        private bool shadows = false;
        private ShadowMap shadowMapLow = null;
        private ShadowMap shadowMapHigh = null;
        private static readonly float shadowMapCameraHeight = 1000;

        /// <summary>
        /// Static objects will cast a flickering shadow if these two numbers are not equal.
        /// </summary>
        private Vector2 shadowMapCameraSizeLow = new Vector2(3000, 3000);
        private Vector2 shadowMapCameraSizeHigh = new Vector2(500, 500);


        public Sunlight(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
        }

        public Sunlight(ICanyonShooterGame game, Color color, Vector3 direction)
            : base(game as Game)
        {
            this.game = game;
            this.color = color;
            this.direction = direction;
        }

        #region ISunlight Members

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public bool Shadows
        {
            get
            {
                return shadows;
            }
            set
            {
                if (!game.Graphics.ShadowMappingSupported)
                {
                    throw new Exception("Shadow mapping is not supported by this device. This call should not be made.");
                }

                if (shadows == value) return;

                shadows = value;

                if (shadowMapLow != null)
                {
                    shadowMapLow.Dispose();
                    shadowMapHigh.Dispose();
                }

                if (shadows)
                {
                    shadowMapLow =
                        new ShadowMap(game, 256, 256);
                    shadowMapLow.Scene.Camera =
                        new OrthographicCamera(game, (int)shadowMapCameraSizeLow.X, (int)shadowMapCameraSizeLow.Y);
                    shadowMapLow.Scene.Camera.LocalPosition = new Vector3(0, shadowMapCameraHeight, 0);
                    shadowMapLow.Scene.Camera.LocalRotation =
                        Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float)Math.PI / 2);

                    shadowMapHigh =
                        new ShadowMap(game, 1024, 1024);
                    shadowMapHigh.Scene.Camera =
                        new OrthographicCamera(game, (int)shadowMapCameraSizeHigh.X, (int)shadowMapCameraSizeHigh.Y);
                    shadowMapHigh.Scene.Camera.LocalPosition = new Vector3(0, shadowMapCameraHeight, 0);
                    shadowMapHigh.Scene.Camera.LocalRotation =
                        Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float)Math.PI / 2);

                    UpdateOrder = (int)UpdateOrderType.ShadowMap;
                    Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
            }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (shadows)
            {
                if (game.World.Players[0] != null)
                {
                    {
                        Vector3 v = game.World.Players[0].GlobalPosition +
                                    (int) shadowMapCameraSizeLow.Y*0.45f*
                                    game.World.Players[0].Camera.GlobalTransformation.Forward;

                        float x = v.X;
                        float z = v.Z;

                        bool rotate = shadowMapCameraSizeLow.X != shadowMapCameraSizeLow.Y;

                        if (!rotate)
                        {
                            // texel size in world coordinates
                            float texelWidth = shadowMapCameraSizeLow.X/shadowMapLow.Width;
                            float texelHeight = shadowMapCameraSizeLow.Y/shadowMapLow.Height;

                            // truncate x and z to texel size to reduce flicker
                            x = (float) Math.Floor(x/texelWidth)*texelWidth;
                            z = (float) Math.Floor(z/texelHeight)*texelHeight;
                        }

                        shadowMapLow.Scene.Camera.LocalPosition = new Vector3(x, shadowMapCameraHeight, z);

                        if (rotate)
                        {
                            shadowMapLow.Scene.Camera.LocalRotation =
                                Quaternion.Concatenate(
                                    Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float) Math.PI/2),
                                    Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
                                                                   (float)
                                                                   Math.Acos(
                                                                       game.World.Players[0].Camera.GlobalTransformation
                                                                           .
                                                                           Forward.Z)
                                        )
                                    );
                        }
                    }
                    {
                        Vector3 v = game.World.Players[0].GlobalPosition +
                                    (int) shadowMapCameraSizeHigh.Y*0.3f*
                                    game.World.Players[0].Camera.GlobalTransformation.Forward;

                        float x = v.X;
                        float z = v.Z;

                        bool rotate = shadowMapCameraSizeHigh.X != shadowMapCameraSizeHigh.Y;

                        if (!rotate)
                        {
                            // texel size in world coordinates
                            float texelWidth = shadowMapCameraSizeHigh.X/shadowMapHigh.Width;
                            float texelHeight = shadowMapCameraSizeHigh.Y/shadowMapHigh.Height;

                            // truncate x and z to texel size to reduce flicker
                            x = (float) Math.Floor(x/texelWidth)*texelWidth;
                            z = (float) Math.Floor(z/texelHeight)*texelHeight;
                        }

                        shadowMapHigh.Scene.Camera.LocalPosition = new Vector3(x, shadowMapCameraHeight, z);

                        if (rotate)
                        {
                            shadowMapHigh.Scene.Camera.LocalRotation =
                                Quaternion.Concatenate(
                                    Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float) Math.PI/2),
                                    Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
                                                                   (float)
                                                                   Math.Acos(
                                                                       game.World.Players[0].Camera.GlobalTransformation
                                                                           .
                                                                           Forward.Z)
                                        )
                                    );
                        }
                    }
                }

                shadowMapHigh.Draw(gameTime);
                shadowMapLow.Draw(gameTime);
            }
        }


        #region ISunlight Members


        public ShadowMap ShadowMapLow
        {
            get
            {
                if (!game.Graphics.ShadowMappingSupported)
                {
                    throw new Exception("Shadow mapping is not supported by this device. This call should not be made.");
                }
                return shadowMapLow;
            }
        }

        public ShadowMap ShadowMapHigh
        {
            get
            {
                if (!game.Graphics.ShadowMappingSupported)
                {
                    throw new Exception("Shadow mapping is not supported by this device. This call should not be made.");
                }
                return shadowMapHigh;
            }
        }

        #endregion
    }
}
