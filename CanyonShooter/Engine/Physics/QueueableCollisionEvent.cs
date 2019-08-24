using System;
using System.Collections.Generic;
using System.Text;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    public class QueueableCollisionEvent
    {
        private IPhysics physics;
        private ICollisionActor actor;
        private CollisionEvent e;

        public QueueableCollisionEvent(IPhysics physics, ICollisionActor actor, CollisionEvent e)
        {
            if(actor == null) throw new Exception();
            this.actor = actor;
            this.e = e;
            this.physics = physics;
        }

        public ICollisionActor Actor
        {
            get
            {
                return actor;
            }
        }

        public CollisionEvent Event
        {
            get
            {
                return e;
            }
        }

        public void Invoke()
        {
            physics.DispatchCollisionEvent(this);
        }
    }
}
