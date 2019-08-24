// Zuständigkeit: Richard, Christian, Florian
#region Using Statements

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using XnaDevRu.Physics.Ode;
using System.Threading;
using System.Diagnostics;

#endregion

namespace CanyonShooter.Engine.Physics
{
    public enum ContactGroup
    {
        Default = 0,
        Debris = 10,
        Enemies = 11,
        Statics = 12,
        Items = 13,
        Canyon = 14,
        Player = 16,
        Trigger = 17,
        PlayerProjectiles = 18,
        EnemyProjectiles = 19,
        Weapons = 20,
        Sky = 21,
        None = 22,
        Waypoint = 5,
    }

    /// <summary>
    /// Physik-Engine. Bindet eine open-source Physik-Engine ein.
    /// </summary>
    public class Physics : GameComponent, IPhysics
    {
        class AddShapeEvent
        {
            public Solid solid;

            /// <summary>
            /// if null -> clear all shapes
            /// </summary>
            public ShapeData shape;

            public AddShapeEvent(Solid solid, ShapeData shape)
            {
                this.solid = solid;
                this.shape = shape;
            }
        }


        #region Private Fields

        private Thread thread;
        private bool shutdown = false;
        private bool mayGameContinue = true;
        private object mayGameContinueSignal = new object();
        private bool mayPhysicsContinue = true;
        private object mayPhysicsContinueSignal = new object();

        private Simulator simulator = null;

        private Collection<ITransformable> transformables = new Collection<ITransformable>();

        private ICanyonShooterGame game;

        private IQueue<IPhysicsObject> createdObjects;
        private IQueue<IPhysicsObject> destroyedObjects;

        private bool multiThreading = false;
        
        #endregion


        public Physics(ICanyonShooterGame game, bool multiThreading)
            : base(game as Game)
        {
            this.game = game;

            Simulator.CreateMethod = OdeSimulator.CreateOdeSimulator;
            simulator = Simulator.Create(); // Simulator erzeugen;
            simulator.StepSize = 0.012f; // 0.0167f ~ 60 Hz physics update rate.
            simulator.MaxContacts = 4;

            UpdateOrder = int.MaxValue - 10;

            SetupContactGroups();

            this.multiThreading = multiThreading;

            if (multiThreading)
            {
                createdObjects = new LockingQueue<IPhysicsObject>();
                destroyedObjects = new LockingQueue<IPhysicsObject>();

                thread = new Thread(RunMultiThreaded);
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            else
            {
                createdObjects = new NonLockingQueue<IPhysicsObject>();
                destroyedObjects = new NonLockingQueue<IPhysicsObject>();   
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!multiThreading)
            {
                Step(gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public bool MultiThreading
        {
            get
            {
                return multiThreading;
            }
        }

        private void RunMultiThreaded()
        {
            DateTime previous = DateTime.Now;
            while(!shutdown)
            {
                //Helper.Helper.BeginTimeMeasurement("physics step");
                DateTime now = DateTime.Now;
                TimeSpan difference = TimeSpan.FromMilliseconds(Math.Max((now - previous).TotalMilliseconds, 1));

                if (difference.TotalSeconds > simulator.StepSize)
                {
                    Step(difference.TotalSeconds);
                    //Step((double)simulator.StepSize);

                    previous = now;

                    //Debug.Print("physics");
                }
                //int ms = Helper.Helper.EndTimeMeasurement("physics step");
                
                /*if(ms > difference.TotalMilliseconds)
                {
                    Debug.Print("physics step: dt = " + (int)(difference.TotalMilliseconds) + " , duration: " + ms + " , diff: " + (ms - difference.TotalMilliseconds));
                }*/

                // synchronize with main-thread's main loop
                lock (mayPhysicsContinueSignal)
                {
                    if (!mayPhysicsContinue) Monitor.Wait(mayPhysicsContinueSignal);
                    mayPhysicsContinue = false;
                }
                /*lock (mayGameContinueSignal)
                {
                    mayGameContinue = true;
                    Monitor.PulseAll(mayGameContinueSignal);
                }*/
            }
        }

        private void Step(double difference)
        {
            if (simulator != null)
            {
                Helper.Helper.BeginTimeMeasurement("physics_113");
                if(multiThreading) Monitor.Enter(simulator);
                try
                {
                    Helper.Helper.EndTimeMeasurement("physics_113");
                    while (createdObjects.Count > 0)
                    {
                        Solid xpaSolid = simulator.CreateSolid();

                        // solids can disable automatically when they aren't moved any more.
                        // disabled solids will not move until they are re-enabled. we don't want that.
                        xpaSolid.Sleepiness = 0;

                        Debug.Assert(!float.IsNaN(xpaSolid.Position.X));
                        Debug.Assert(!float.IsNaN(xpaSolid.GlobalLinearVel.X));

                        createdObjects.Dequeue().Solid = xpaSolid;
                    }

                    Helper.Helper.BeginTimeMeasurement("physics_141");
                    if (multiThreading) Monitor.Enter(transformables);
                    try
                    {
                        Helper.Helper.EndTimeMeasurement("physics_141");
                        foreach (Transformable t in transformables)
                        {
                            t.BeforePhysicsSimulationStep();
                        }
                    }
                    finally
                    {
                        if(multiThreading) Monitor.Exit(transformables);
                    }

                    //Helper.Helper.BeginTimeMeasurement("physics step internal");
                    simulator.Simulate((float)difference);
                    //int ms = Helper.Helper.EndTimeMeasurement("physics step internal");

                    /*if (ms > difference * 1000)
                    {
                        Debug.Print("physics step internal: dt = " + (int)(difference*1000) + " , duration: " + ms);
                    }*/

                    Helper.Helper.BeginTimeMeasurement("physics_153");
                    if (multiThreading) Monitor.Enter(transformables);
                    try
                    {
                        Helper.Helper.EndTimeMeasurement("physics_153");
                        foreach (Transformable t in transformables)
                        {
                            t.AfterPhysicsSimulationStep();
                        }
                    }
                    finally
                    {
                        if (multiThreading) Monitor.Exit(transformables);
                    }

                    while (destroyedObjects.Count > 0)
                    {
                        IPhysicsObject obj = destroyedObjects.Dequeue();
                        if (obj != null)
                        {
                            //Remove Reference of Transformable in the CollisionActorEventHandler
                            // We use the Dispose() for this.
                            if (obj.CollisionEventProcessor is CollisionActorEventHandler)
                            if (obj.Solid != null)
                            {
                                ((CollisionActorEventHandler)obj.Solid.CollisionEventHandler).Dispose();

                                //Remove the Tag-Reference which references to the Transformable/GameObject in Collision Events.
                                obj.Solid.UserData = null;
                                
                                // Remove CollisionEventHandler from the simulator
                                simulator.RemoveGlobalCollisionEventHandler(obj.Solid.CollisionEventHandler);

                                // Removes the solid from the xpa internal list but doesn't destroy it!
                                simulator.DestroySolid(obj.Solid);

                                // The GeomDatas keep the solid in memory by referencing it
                                foreach (GeomData g in (obj.Solid as OdeSolid).geomDataList)
                                {
                                    /*// remove all references to GeomData
                                    g.SpaceID.Remove(g.GeomID);
                                    g.GeomID.Destroy();
                                    //GCHandle.FromIntPtr(Tao.Ode.Ode.dGeomGetData(g.GeomID)).Free();
                                    //Tao.Ode.Ode.dGeomSetData(g.GeomID, new IntPtr(0));
                                    g.GeomID = null;
                                    g.Shape = null;*/

                                    // remove reference to solid
                                    g.Solid = null;
                                }

                                obj.Solid.ClearShapes();
                            }
                        }
                    }
                }
                finally
                {
                    if (multiThreading) Monitor.Exit(simulator);
                }
            }
        }

        /// <summary>
        /// not thread safe because it is called in the constructor (main thread)
        /// </summary>
        private void DisableContactGroupCollisionWithEverything(ContactGroup g)
        {
            simulator.SetupContactGroup((int) g, false);
        }

        /// <summary>
        /// not thread safe because it is called in the constructor (main thread)
        /// </summary>
        private void DisableContactGroupCollision(ContactGroup g1, ContactGroup g2)
        {
            simulator.SetupContactGroups((int)g1, (int)g2, false);
        }

        /// <summary>
        /// not thread safe because it is called in the constructor (main thread)
        /// </summary>
        private void EnableContactGroupCollision(ContactGroup g1, ContactGroup g2)
        {
            simulator.SetupContactGroups((int)g1, (int)g2, true);
        }

        /// <summary>
        /// not thread safe because it is called in the constructor (main thread)
        /// </summary>
        private void DisableContactGroupCollisionWithSelf(ContactGroup g)
        {
            DisableContactGroupCollision(g, g);
        }

        /// <summary>
        /// not thread safe because it is called in the constructor (main thread)
        /// </summary>
        private void SetupContactGroups()
        {
            DisableContactGroupCollisionWithEverything(ContactGroup.None);
            DisableContactGroupCollisionWithEverything(ContactGroup.Trigger);
            DisableContactGroupCollisionWithEverything(ContactGroup.Items);
            DisableContactGroupCollisionWithEverything(ContactGroup.Sky);
            DisableContactGroupCollisionWithEverything(ContactGroup.Waypoint);


            DisableContactGroupCollisionWithSelf(ContactGroup.Enemies);
            DisableContactGroupCollisionWithSelf(ContactGroup.Player);
            DisableContactGroupCollisionWithSelf(ContactGroup.Statics);
            DisableContactGroupCollisionWithSelf(ContactGroup.Debris);
            DisableContactGroupCollisionWithSelf(ContactGroup.Weapons);
            DisableContactGroupCollisionWithSelf(ContactGroup.PlayerProjectiles);
            DisableContactGroupCollisionWithSelf(ContactGroup.EnemyProjectiles);

            DisableContactGroupCollision(ContactGroup.Player, ContactGroup.PlayerProjectiles);
            DisableContactGroupCollision(ContactGroup.Enemies, ContactGroup.EnemyProjectiles);

            // TODO remove
            DisableContactGroupCollision(ContactGroup.Enemies, ContactGroup.Canyon);

            DisableContactGroupCollision(ContactGroup.Weapons, ContactGroup.PlayerProjectiles);
            DisableContactGroupCollision(ContactGroup.Weapons, ContactGroup.EnemyProjectiles);
            DisableContactGroupCollision(ContactGroup.Weapons, ContactGroup.Player);
            DisableContactGroupCollision(ContactGroup.Weapons, ContactGroup.Enemies);

            EnableContactGroupCollision(ContactGroup.Sky, ContactGroup.Player);

            DisableContactGroupCollision(ContactGroup.Canyon, ContactGroup.Canyon);
        }


        #region IPhysics Member

        public bool MayGameContinue
        {
            set
            {
                mayGameContinue = value;
            }
        }
        public bool MayPhysicsContinue
        {
            get
            {
                return mayPhysicsContinue;
            }
            set
            {
                mayPhysicsContinue = value;
            }
        }

        public IPhysicsObject CreatePhysicsObject()
        {
            IPhysicsObject result;

            if (multiThreading)
            {
                result = new LockingPhysicsObject(this);
            }
            else
            {
                result = new NonLockingPhysicsObject(this);
            }

            createdObjects.Enqueue(result);
            return result;
        }

        public void DestroyPhysicsObject(IPhysicsObject obj)
        {
            if(obj != null)
            {
                destroyedObjects.Enqueue(obj);
            }
        }

        public void RegisterTransformable(ITransformable t)
        {
            if (multiThreading)
            {
                Helper.Helper.BeginTimeMeasurementDebugOutput("physics_283");
                lock (transformables)
                {
                    Helper.Helper.EndTimeMeasurementDebugOutput("physics_283");
                    if (!transformables.Contains(t))
                    {
                        transformables.Add(t);
                    }
                }
            }
            else
            {
                if (!transformables.Contains(t))
                {
                    transformables.Add(t);
                }
            }
        }

        public void UnregisterTransformable(ITransformable t)
        {
            if (multiThreading)
            {
                Helper.Helper.BeginTimeMeasurementDebugOutput("physics_296");
                lock (transformables)
                {
                    Helper.Helper.EndTimeMeasurementDebugOutput("physics_296");
                    transformables.Remove(t);
                }
            }
            else
            {
                transformables.Remove(t);
            }
        }

        #endregion

        #region IDisposable Member

        void IDisposable.Dispose()
        {
            if (multiThreading)
            {
                shutdown = true;
                while (thread.IsAlive)
                {
                    lock (MayPhysicsContinueSignal)
                    {
                        MayPhysicsContinue = true;
                        Monitor.PulseAll(MayPhysicsContinueSignal);
                    }
                    Thread.Sleep(100);
                }

                Helper.Helper.BeginTimeMeasurementDebugOutput("physics_312");
                lock (simulator)
                {
                    Helper.Helper.EndTimeMeasurementDebugOutput("physics_312");
                    if (simulator != null)
                        simulator.Destroy();
                }
            }
            else
            {
                if (simulator != null)
                    simulator.Destroy();
            }
        }

        #endregion

        #region IPhysics Members


        public bool CanContactGroupsCollide(ContactGroup g1, ContactGroup g2)
        {
            return simulator.GroupsMakeContacts((int) g1, (int) g2);
        }

        public bool CanSolidsCollide(Solid s1, Solid s2)
        {
            bool result = false;

            ShapeData sd1;
            ShapeData sd2;

            if ((s1.UserData as ITransformable) == null)
            {
                return false;
            }
            else
            {
                sd1 = (s1.UserData as ITransformable).GetShape(0);
            }

            if ((s2.UserData as ITransformable) == null)
            {
                return false;
            }
            else
            {
                sd2 = (s2.UserData as ITransformable).GetShape(0);
            }

            int cg1 = sd1.ContactGroup;
            int cg2 = sd2.ContactGroup;
            result = simulator.GroupsMakeContacts(cg1, cg2);


            return result;
        }

        public VolumeQueryResult VolumeQuery(Matrix transform, string name, ShapeData[] shapes)
        {
            VolumeQueryResult result;

            Helper.Helper.BeginTimeMeasurementDebugOutput("physics_368");
            if(multiThreading) Monitor.Enter(simulator);
            try
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("physics_368");
                VolumeSensor sensor;

                sensor = simulator.CreateVolumeSensor();

                VolumeSensorData data = new VolumeSensorData();
                data.Transform = transform;
                data.Enabled = true;
                data.Name = name;
                sensor.Init(data);

                Solid volume = simulator.CreateSolid();

                foreach (ShapeData shape in shapes)
                {
                    volume.AddShape(shape);
                }

                // TODO solve bug in SpaceBase.setParentSpace
                //SpaceBase space = game.Physics.CreateSpace();
                //volume.Space = space;

                result = sensor.QueryVolume(volume);
            }
            finally
            {
                if (multiThreading) Monitor.Exit(simulator);
            }

            return result;
        }

        public void DispatchCollisionEvent(QueueableCollisionEvent d)
        {
            d.Actor.OnCollision(d.Event);
        }

        #endregion

        #region IPhysics Members


        public object MayGameContinueSignal
        {
            get { return mayGameContinueSignal; }
        }

        public object MayPhysicsContinueSignal
        {
            get { return mayPhysicsContinueSignal; }
        }

        #endregion
    }
}


