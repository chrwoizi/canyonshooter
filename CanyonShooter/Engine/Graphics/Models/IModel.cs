//
//
//  @ Project : CanyonShooter
//  @ File Name : IModel.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using System.Collections.Generic;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using DescriptionLibs.Model;
using CanyonShooter.Engine.Graphics.Effects;

namespace CanyonShooter.Engine.Graphics.Models
{
    /// <summary></summary>
    public interface IModel : ITransformable
    {
        /// <summary>
        /// the name of the loaded model file
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a List of Meshes
        /// </summary>
        List<IMesh> Meshes { get; }

        /// <summary>
        /// materials for submeshes.
        /// materials[i] is mapped to xnaModel.Meshes[i]
        /// a default material will be used for the submesh if the according material is null.
        /// </summary>
        List<IMaterial> Materials { get; }

        /// <summary>
        /// provides slots for attachable weapons
        /// </summary>
        List<WeaponSlot> WeaponSlots { get; }

        /// <summary>
        /// 
        /// </summary>
        List<ShapeData> CollisionShapes { get; }

        /// <summary>
        /// 
        /// </summary>
        List<WreckageModel> WreckageModels { get; }

        List<IEffect> ParticleEffects { get;}

        /// <summary>
        /// 
        /// </summary>
        float MassInDescription { get; }

        /// <summary>
        /// shows the afterburner. afterburners can be set in the description xml
        /// </summary>
        bool ShowAfterBurner { get; set; }

        float AfterBurnerLength { get; set; }

        /// <summary>
        /// returns the mesh named name. will throw an exception if it doesn't exist.
        /// </summary>
        /// <param name="name">name of the mesh</param>
        IMesh GetMesh(string name);

        /// <summary>
        /// Draws the model
        /// </summary>
        void Draw(IShaderConstantsSetter shaderConstantsSetter);

        /// <summary>
        /// Updates the mesh animations
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
    }
}
