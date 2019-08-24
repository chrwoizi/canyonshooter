using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.Engine.Graphics.Cameras;

namespace CanyonShooter.Engine.Graphics.PostProcessing
{
    #region File Description
    // adapted from microsoft xna bloom sample
    //-----------------------------------------------------------------------------
    // BloomComponent.cs
    //
    // Microsoft XNA Community Game Platform
    // Copyright (C) Microsoft Corporation. All rights reserved.
    //-----------------------------------------------------------------------------
    #endregion

    public class RadialBlurPostProcess : DrawableGameComponent, IPostProcess
    {
        #region File Description
        //-----------------------------------------------------------------------------
        // BloomSettings.cs
        //
        // Microsoft XNA Community Game Platform
        // Copyright (C) Microsoft Corporation. All rights reserved.
        //-----------------------------------------------------------------------------
        #endregion

        public enum IntermediateBuffer
        {
            Blurred,
            FinalResult,
        }

        private ICanyonShooterGame game;
        private ResolveTexture2D resolveTarget;
        private RenderTarget2D renderTarget1;
        private RenderTarget2D renderTarget2;
        private SpriteBatch spriteBatch;
        private Effect blurCombineEffect;
        private Effect radialBlurEffect;


        public RadialBlurPostProcess(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            DrawOrder = (int)DrawOrderType.PostProcess;
        }

        public IntermediateBuffer ShowBuffer
        {
            get { return showBuffer; }
            set { showBuffer = value; }
        }

        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            blurCombineEffect = Game.Content.Load<Effect>("Content\\Effects\\PostProcessing\\RadialBlurCombine");
            radialBlurEffect = Game.Content.Load<Effect>("Content\\Effects\\PostProcessing\\RadialBlur");

            PresentationParameters pp = game.Graphics.Device.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;

            // Create a texture for reading back the backbuffer contents.
            resolveTarget = new ResolveTexture2D(GraphicsDevice, width, height, 1, format);

            // Create two rendertargets for the bloom processing. These are half the
            // size of the backbuffer, in order to minimize fillrate costs. Reducing
            // the resolution in this way doesn't hurt quality, because we are going
            // to be blurring the bloom images in any case.
            width /= 2;
            height /= 2;

            renderTarget1 = new RenderTarget2D(GraphicsDevice, width, height, 1,
                format, GraphicsDevice.PresentationParameters.MultiSampleType, GraphicsDevice.PresentationParameters.MultiSampleQuality);
            renderTarget2 = new RenderTarget2D(GraphicsDevice, width, height, 1,
                format, GraphicsDevice.PresentationParameters.MultiSampleType, GraphicsDevice.PresentationParameters.MultiSampleQuality);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UnloadContent()
        {
            resolveTarget.Dispose();
            renderTarget1.Dispose();
            renderTarget2.Dispose();
        }

        /// <summary>
        /// This is where it all happens. Grabs a scene that has already been rendered,
        /// and uses postprocess magic to add a glowing bloom effect over the top of it.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            game.Renderer.SetDefaultRenderStates();

            // Resolve the scene into a texture, so we can
            // use it as input data for the bloom processing.
            GraphicsDevice.ResolveBackBuffer(resolveTarget);

            radialBlurEffect.Parameters["InvScreenWidth"].SetValue(1.0f / resolveTarget.Width);

            DrawFullscreenQuad(resolveTarget, renderTarget1,
                               radialBlurEffect,
                               IntermediateBuffer.Blurred);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            GraphicsDevice.SetRenderTarget(0, null);

            EffectParameterCollection parameters = blurCombineEffect.Parameters;

            GraphicsDevice.Textures[1] = resolveTarget;

            Viewport viewport = GraphicsDevice.Viewport;

            blurCombineEffect.Parameters["Power"].SetValue(game.World.Players[0].RelativeSpeed);

            DrawFullscreenQuad(renderTarget1.GetTexture(),
                               viewport.Width, viewport.Height,
                               blurCombineEffect,
                               IntermediateBuffer.FinalResult);
        }


        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            GraphicsDevice.SetRenderTarget(0, renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect, currentBuffer);

            GraphicsDevice.SetRenderTarget(0, null);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            game.Renderer.SetDefaultRenderStates();

            // Begin the custom effect, if it is currently enabled. If the user
            // has selected one of the show intermediate buffer options, we still
            // draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            if (showBuffer >= currentBuffer)
            {
                effect.Begin();
                effect.CurrentTechnique.Passes[0].Begin();
            }

            // Draw the quad.
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();

            // End the custom effect.
            if (showBuffer >= currentBuffer)
            {
                effect.CurrentTechnique.Passes[0].End();
                effect.End();
            }
        }
    }
}
