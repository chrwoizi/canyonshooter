using System;
using CanyonShooter.Engine.Graphics.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class RenderTarget2DAdapter : IDisposable
    {
        private IScene scene = new Scene();
        private RenderTarget2D renderTarget;
        protected ICanyonShooterGame game;
        private bool renderedAtLeastOnce = false;
        private DepthStencilBuffer depthStencilBuffer = null;

        public RenderTarget2DAdapter(ICanyonShooterGame game, int width, int height, int numberLevels, SurfaceFormat format, RenderTargetUsage usage)
        {
            this.game = game;
            CreateRenderTarget(format, width, height, numberLevels, usage);
        }

        public RenderTarget2DAdapter(ICanyonShooterGame game, int width, int height, int numberLevels, SurfaceFormat format, RenderTargetUsage usage, DepthFormat depthFormat)
        {
            this.game = game;
            CreateRenderTarget(format, width, height, numberLevels, usage);

            depthStencilBuffer = new DepthStencilBuffer(game.Graphics.Device, width, height, depthFormat, MultiSampleType.None, 0);
        }

        private void CreateRenderTarget(SurfaceFormat format, int width, int height, int numberLevels, RenderTargetUsage usage)
        {
            if (!GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware,
                                                                  GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format, TextureUsage.None,
                                                                  QueryUsages.None, ResourceType.RenderTarget, format))
            {
                throw new Exception("Display format " + format + " is not supported by the graphics device.");
            }
            renderTarget = new RenderTarget2D(game.Graphics.Device, width, height, numberLevels, format, usage);
            renderTarget.Name = "RenderTarget2DAdapter";
        }

        public void Draw(GameTime gameTime)
        {
            ICamera cameraBackup = game.Renderer.Camera;
            game.Renderer.Camera = scene.Camera;

            game.Graphics.Device.SetRenderTarget(0, renderTarget);

            DepthStencilBuffer depthStencilBufferBackup = game.Graphics.Device.DepthStencilBuffer;
            if (depthStencilBuffer != null) game.Graphics.Device.DepthStencilBuffer = depthStencilBuffer;

            game.Graphics.Device.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            OnDrawBegin();

            scene.Draw(gameTime);

            OnDrawEnd();

            if (depthStencilBuffer != null) game.Graphics.Device.DepthStencilBuffer = depthStencilBufferBackup;

            game.Graphics.Device.SetRenderTarget(0, null);

            game.Renderer.Camera = cameraBackup;

            renderedAtLeastOnce = true;
        }

        public virtual void OnDrawBegin()
        {

        }

        public virtual void OnDrawEnd()
        {

        }

        public IScene Scene
        {
            get
            {
                return scene;
            }
        }

        public Texture2D Texture
        {
            get
            {
                if(renderedAtLeastOnce)
                    return renderTarget.GetTexture();
                else
                    return null;
            }
        }

        public int Width
        {
            get
            {
                return renderTarget.Width;
            }
        }

        public int Height
        {
            get
            {
                return renderTarget.Height;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            renderTarget.Dispose();
        }

        #endregion
    }
}
