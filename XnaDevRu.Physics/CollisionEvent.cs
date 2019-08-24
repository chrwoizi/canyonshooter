using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure containing info about a specific collision event
    /// involving two Solids.
    /// </summary>
    public struct CollisionEvent
    {
        /// <summary>
        /// The colliding Solid whose CollisionEventHandler gets called.
        /// </summary>
        private Solid thisSolid;

        /// <summary>
        /// The Solid that collided with the one owning the
        /// CollisionEventHandler.
        /// </summary>
        private Solid otherSolid;

        /// <summary>
        /// The point of collision.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The normal vector at the point of collision.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The depth of interpenetration.  This may not be very helpful
        /// if the two Solids are allowed to pass through each other (i.e.
        /// their contact groups do not generate contacts).
        /// </summary>
        private float depth;

        /// <summary>
        /// The colliding Solid whose CollisionEventHandler gets called.
        /// </summary>
        public Solid ThisSolid {
            get {
                return thisSolid;
            }
            set {
                thisSolid = value;
            }
        }

        /// <summary>
        /// The Solid that collided with the one owning the
        /// CollisionEventHandler.
        /// </summary>
        public Solid OtherSolid {
            get {
                return otherSolid;
            }
            set {
                otherSolid = value;
            }
        }

        ///// <summary>
        ///// The point of collision.
        ///// </summary>
        //public Vector3 Position {
        //    get {
        //        return pos;
        //    }
        //    set {
        //        pos = value;
        //    }
        //}

        ///// <summary>
        ///// The normal vector at the point of collision.
        ///// </summary>
        //public Vector3 Normal {
        //    get {
        //        return normal;
        //    }
        //    set {
        //        normal = value;
        //    }
        //}

        /// <summary>
        /// The depth of interpenetration.  This may not be very helpful
        /// if the two Solids are allowed to pass through each other (i.e.
        /// their contact groups do not generate contacts).
        /// </summary>
        public float Depth {
            get {
                return depth;
            }
            set {
                depth = value;
            }
        }

        public override string ToString()
        {
            return "Collision, solid1= " + ThisSolid + " solid2= " + OtherSolid;
        }

    }

    /// <summary>
    /// A listener that gets notified when two Solids touch.  These events
    /// get handled at the end of the time step, which is necessary because
    /// some physics engines cannot update objects in the middle of a time
    /// step (e.g. in the middle of collision detection).
    /// </summary>
    public abstract class CollisionEventProcessor : EventProcessor
    {
        /// This list holds pending CollisionEvents.  It allows events to
        /// be queued during collision detection and handled at a safe time.
        protected List<CollisionEvent> pendingCollisionEvents;

        public CollisionEventProcessor()
        {
            pendingCollisionEvents = new List<CollisionEvent>();
        }

        ~CollisionEventProcessor()
        {
            pendingCollisionEvents.Clear();
        }
        /// <summary>
        /// Called once for each pending CollisionEvent.  This is always
        /// called at the end of every time step so CollisionEvents
        /// get handled right away.
        /// </summary>
        /// <param name="e"></param>
        public abstract void HandleCollisionEvent(CollisionEvent e);

        /// <summary>
        /// Adds a CollisionEvent to this handler's internal list.  These
        /// will get handled at the end of the current time step.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void InternalPushCollisionEvent(CollisionEvent e)
        {
            pendingCollisionEvents.Add(e);
        }

        /// <summary>
        /// Loops through the pending CollisionEvents, calling the event
        /// handling function (which is always user-defined) for each.
        /// </summary>
        protected internal virtual void InternalHandlePendingCollisionEvents()
        {
            foreach (CollisionEvent e in pendingCollisionEvents)
                HandleCollisionEvent(e);

            pendingCollisionEvents.Clear();
        }
    }

    /// <summary>
    /// A listener that gets notified when a simulation step has finished.
    /// </summary>
    public abstract class PostStepEventProcessor : EventProcessor
    {
        public PostStepEventProcessor() { }

        /// <summary>
        /// Called once for each pending CollisionEvent.  This is always
        /// called at the end of a time step, so CollisionEvents always
        /// get handled right away.
        /// </summary>
        public abstract void HandlePostStepEvent();
    }

}
