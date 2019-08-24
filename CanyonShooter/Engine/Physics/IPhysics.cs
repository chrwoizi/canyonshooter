//
//
//  @ Project : CanyonShooter
//  @ File Name : IPhysics.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using System;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    /// <summary></summary>
    public interface IPhysics : IDisposable, IGameComponent, IUpdateable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IPhysicsObject CreatePhysicsObject();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void DestroyPhysicsObject(IPhysicsObject obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void RegisterTransformable(ITransformable t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void UnregisterTransformable(ITransformable t);

        /// <summary>
        /// returns true if the contact groups can collide.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        bool CanContactGroupsCollide(ContactGroup g1, ContactGroup g2);

        /// <summary>
        /// returns true if the solids are in contact groups which can collide.
        /// useful for testing wether the solids collide or just intersect (i.e. trigger volumes)
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        bool CanSolidsCollide(Solid s1, Solid s2);

        /// <summary>
        /// thread safe
        /// </summary>
        /// <param name="shapes"></param>
        /// <returns></returns>
        VolumeQueryResult VolumeQuery(Matrix transform, string name, ShapeData[] shapes);

        /// <summary>
        /// thread safe collision event dispatch
        /// </summary>
        /// <param name="d"></param>
        void DispatchCollisionEvent(QueueableCollisionEvent d);

        bool MayGameContinue { set; }
        object MayGameContinueSignal { get; }
        bool MayPhysicsContinue { get; set; }
        object MayPhysicsContinueSignal { get; }

        /// <summary>
        /// returns true if the physics engine runs in a separate thread
        /// </summary>
        bool MultiThreading { get; }
    }
}
