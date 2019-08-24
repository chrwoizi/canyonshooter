using System;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    /// <summary>
    /// Handles the ICollisionActor interface members.
    /// When a collision between an ICollisionActor and an other object occurs, 
    /// the OnCollision-Event of the ICollisionActor is fired.
    /// </summary>
    public class CollisionActorEventHandler : CollisionEventProcessor
    {
        private ICollisionActor actor;
        private IPhysics physics;

        public CollisionActorEventHandler(IPhysics physics, ICollisionActor actor)
        {
            if (actor == null) throw new Exception();
            this.actor = actor;
            this.physics = physics;
        }

        /// <summary>
        /// Called once for each pending CollisionEvent.  This is always
        /// called at the end of every time step so CollisionEvents
        /// get handled right away.
        /// </summary>
        /// <param name="e"></param>
        public override void HandleCollisionEvent(CollisionEvent e)
        {
            if (actor != null) (actor as ITransformable).AddCollisionEvent(new QueueableCollisionEvent(physics, actor, e));
        }

        public void Dispose()
        {
            pendingCollisionEvents.Clear();
            actor = null;
        }
    }
}
