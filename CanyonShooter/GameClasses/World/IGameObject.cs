//
//
//  @ Project : CanyonShooter
//  @ File Name : IGameObject.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.GameClasses.World
{
    /// <summary></summary>
    public interface IGameObject : ITransformable, IGameComponent, IDrawable, ICollisionActor, IShaderConstantsSetter
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// model. can be set if you want to. can be null.
        /// </summary>
        IModel Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CanyonSegment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ContactGroup ContactGroup { get; set; }

        /// <summary>
        /// Sets the model. This can be used before LoadContent.
        /// </summary>
        /// <param name="name">The model's name</param>
        void SetModel(string name);

        /// <summary>
        /// Automatically calls ApplyModelsCollisionShapes when the Model or ConnectedToXpa changes
        /// </summary>
        bool AutoApplyModelsCollisionShapes { get; set; }

        /// <summary>
        /// Automatically calls ApplyModelsMass when the Model or ConnectedToXpa changes
        /// </summary>
        bool AutoApplyModelsMass { get; set; }

        /// <summary>
        /// Uses the collision shapes of the model for physics
        /// </summary>
        void ApplyModelsCollisionShapes();

        /// <summary>
        /// Uses the mass of the model for physics
        /// </summary>
        void ApplyModelsMass();

        /// <summary>
        /// Deletes the object from the world and disposes it
        /// </summary>
        void Destroy();

        /// <summary>
        /// Splits the game object into wreckage parts
        /// </summary>
        void Explode();

        void AddShape(ShapeData shape);
    }
}
