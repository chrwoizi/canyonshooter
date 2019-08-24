using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public class BloomAndRadialBlurPostProcess : DrawableGameComponent, IPostProcess
    {
        private ICanyonShooterGame game;
        private RadialBlurPostProcess blur;
        private BloomPostProcess bloom;

        public BloomAndRadialBlurPostProcess(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            DrawOrder = (int)DrawOrderType.PostProcess;
            blur = new RadialBlurPostProcess(game);
            bloom = new BloomPostProcess(game);
        }

        internal void addToComponents(GameComponentCollection gameComponents)
        {
            gameComponents.Add(bloom);
            gameComponents.Add(blur);
        }
    }
}
