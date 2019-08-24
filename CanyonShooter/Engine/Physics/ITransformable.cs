using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    /// <summary>
    /// Transform information: position, rotation, size. computes transformation as a matrix.
    /// Can be in a hierarchy of ITransformables. Relative values can be set. Absolute values
    /// are the combination of the parent and this.
    /// </summary>
    public interface ITransformable : IDisposable
    {
        /// <summary>
        /// Recalculates the global values following the hierarchy. 
        /// Used internally. 
        /// Serves no use to other classes.
        /// </summary>
        void RecalculateGlobalValues();

        /// <summary>
        /// Adds c to the childs and sets c's parent to this. You can also use the Parent property.
        /// </summary>
        /// <param name="c">The new child</param>
        void AddChild(ITransformable c);

        /// <summary>
        /// 
        /// </summary>
        void BeforePhysicsSimulationStep();

        /// <summary>
        /// 
        /// </summary>
        void AfterPhysicsSimulationStep();

        /// <summary>
        /// Removes c from the childs and sets c's parent to null. You can also use the Parent property.
        /// </summary>
        /// <param name="c">An existing child</param>
        void RemoveChild(ITransformable c);

        /// <summary>
        /// global transformation (parent.global and local) (scaled, rotated, translated)
        /// </summary>
        Matrix GlobalTransformation { get; }

        /// <summary>
        /// local transformation (scaled, rotated, translated)
        /// </summary>
        Matrix LocalTransformation { get; }

        /// <summary>
        /// global position (parent.global and local)
        /// </summary>
        Vector3 GlobalPosition { get; }

        /// <summary>
        /// local position
        /// </summary>
        Vector3 LocalPosition { get; set; }

        /// <summary>
        /// global rotation (parent.global and local)
        /// </summary>
        Quaternion GlobalRotation { get; }

        /// <summary>
        /// local rotation
        /// </summary>
        Quaternion LocalRotation { get; set; }

        /// <summary>
        /// global size (parent.global and local)
        /// </summary>
        Vector3 GlobalScale { get; }

        /// <summary>
        /// local size
        /// </summary>
        Vector3 LocalScale { get; set; }

        /// <summary>
        /// Moves by the specified vector. (in local coordinates)
        /// </summary>
        /// <param name="v">The vector.</param>
        void Move(Vector3 v);

        /// <summary>
        /// Rotates by the specified quaternion. (in local coordinates)
        /// </summary>
        /// <param name="q">The quaternion.</param>
        void Rotate(Quaternion q);

        /// <summary>
        /// Scales by the specified vector. (in local coordinates)
        /// </summary>
        /// <param name="v">The vector.</param>
        void Scale(Vector3 v);

        /// <summary>
        /// parent transformable
        /// </summary>
        ITransformable Parent { get; set; }

        /// <summary>
        /// child transformables (their parent is this)
        /// </summary>
        ReadOnlyCollection<ITransformable> Childs { get; }

        /// <summary>
        /// if this should be handled by the physics engine
        /// </summary>
        bool ConnectedToXpa { get; set; }

        /// <summary>
        /// The velocity of this in units per second.
        /// </summary>
        Vector3 Velocity { get; set; }

        /// <summary>
        /// The AngularVelocity of this.
        /// </summary>
        Vector3 AngularVelocity { get; set; }

        /// <summary>
        /// If you are using the delegation pattern you must(!) set Self to your object (instance of GameObject or Effect).
        /// Do this in the constructor of the Transformable. This property is read only.
        /// Why this is necessary:
        /// Not all implementations of ITransformable are real subclasses of Transformable (because: no multiple inheritance and delegation pattern)
        /// and a Transformable uses "this" for some calculations. This leads to errors in hierarchical ITransformable structures (see Parent property)
        /// because "this" is not the actual object but the delegation object. Imagine you set Parent to an instance of GameObject. this does not know
        /// the actual game object but only the transformable member of the game object. 
        /// </summary>
        ITransformable Self { get; }

        bool InfluencedByGravity { get; set; }

        bool Static { get; set; }

        void AddShape(ShapeData shape, ContactGroup group);
        ShapeData GetShape(int id);
        void ClearShapes();

        /// <summary>
        /// mass. default is 1
        /// </summary>
        float Mass { get; set; }

        ICollisionActor CollisionActor { get; set; }

        /// <summary>
        /// thread safe. all added events will be dispatched in the next update
        /// </summary>
        void AddCollisionEvent(QueueableCollisionEvent e);

        void AddForce(Force f);

        void Update(GameTime gameTime);
    }
}
