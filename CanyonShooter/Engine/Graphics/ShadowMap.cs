using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class ShadowMap : RenderTarget2DAdapter
    {
        public static readonly int DefaultSize = 1024;
        private Matrix shadowMatrix;

        public ShadowMap(ICanyonShooterGame game, int width, int height)
            : base(game, width, height, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents, DepthFormat.Depth24)
        {
            if(!game.Graphics.ShadowMappingSupported)
            {
                throw new Exception("Shadow mapping is not supported on this device. It should be disabled for performance gain.");
            }
        }

        public override void OnDrawBegin()
        {
            base.OnDrawBegin();

            game.Renderer.RenderDepth = true;
        }

        public override void OnDrawEnd()
        {
            game.Renderer.RenderDepth = false;

            // TODO move matrix calculation to observer of camera.onTransform
            Matrix tex;
            tex.M11 = .5f;
            tex.M12 = 0;
            tex.M13 = 0;
            tex.M14 = 0;
            tex.M21 = 0;
            tex.M22 = -.5f;
            tex.M23 = 0;
            tex.M24 = 0;
            tex.M31 = 0;
            tex.M32 = 0;
            tex.M33 = 1;
            tex.M34 = 0;
            tex.M41 = .5f;
            tex.M42 = .5f;
            tex.M43 = 0;
            tex.M44 = 1;

            shadowMatrix = Scene.Camera.ViewMatrix * Scene.Camera.ProjectionMatrix * tex;

            base.OnDrawEnd();
        }

        public Matrix ShadowMatrix
        {
            get { return shadowMatrix; }
        }
    }
}
