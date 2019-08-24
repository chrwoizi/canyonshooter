//
//
//  @ Project : CanyonShooter
//  @ File Name : IRenderer.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Graphics.Models;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Graphics
{
    public enum PostProcessType
    {
        None,
        Bloom,
        RadialBlur,
        BloomAndBlur
    }

    /// <summary></summary>
    public interface IRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        ICamera Camera { get; set; }

        /// <summary>
        /// draws a Model onto the screen with a given transformation.
        /// </summary>
        /// <param name="model">Which model to draw.</param>
        void Draw(IModel model, IShaderConstantsSetter shaderConstantsSetter);

        /// <summary>
        /// the post processing filter type
        /// </summary>
        PostProcessType PostProcessType { get; set; }

        /// <summary>
        /// overrides the current technique with a depth technique
        /// </summary>
        bool RenderDepth { get; set; }

        bool DrawCollisionShapes { get; set; }

        void SetDefaultRenderStates();

        void DrawCollisionShape(ShapeData shape, Matrix modelMatrix);
    }
}
