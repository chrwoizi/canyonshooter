using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CanyonShooter.GameClasses;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    public class LockingPhysicsObject : IPhysicsObject
    {
        #region Temporary Data
        private Vector3 globalLinearVel = Vector3.Zero;
        private bool globalLinearVelChanged = false;
        private object globalLinearVelLocker = new object();
        private Vector3 globalAngularVel = Vector3.Zero;
        private bool globalAngularVelChanged = false;
        private object globalAngularVelLocker = new object();
        private Vector3 position = Vector3.Zero;
        private bool positionChanged = false;
        private object positionLocker = new object();
        private Quaternion orientation = Quaternion.Inverse(Quaternion.Identity);
        private bool orientationChanged = false;
        private object orientationLocker = new object();
        private bool isInfluencedByGravity = true;
        private bool isInfluencedByGravityChanged = false;
        private object isInfluencedByGravityLocker = new object();
        private bool isStatic = false;
        private bool isStaticChanged = false;
        private object isStaticLocker = new object();
        private CollisionEventProcessor collisionEventProcessor = null;
        private bool collisionEventProcessorChanged = false;
        private object collisionEventProcessorLocker = new object();
        private Mass mass;
        private bool massChanged = false;
        private Matrix massOffset;
        private bool massOffsetChanged = false;
        private object massLocker = new object();
        private object userData;
        private bool userDataChanged = false;
        private object userDataLocker = new object();
        private List<ShapeData> shapes = new List<ShapeData>();
        private bool shapesChanged = false;
        private LockingQueue<Force> forces = new LockingQueue<Force>();

        private bool disposed = false;
        #endregion

        private Solid solid;
        private IPhysics physics;

        public LockingPhysicsObject(IPhysics physics)
        {
            this.physics = physics;
        }

        public void UpdateToXpa()
        {
            if (disposed) return;
            if (solid == null) return;

            lock (positionLocker)
                if (positionChanged)
                {
                    solid.Position = position;
                    positionChanged = false;
                    Debug.Assert(!float.IsNaN(solid.Position.X));
                }

            lock (orientationLocker)
                if (orientationChanged)
                {
                    solid.Orientation = orientation;
                    orientationChanged = false;
                    Debug.Assert(!float.IsNaN(solid.Orientation.X));
                }

            lock (isStaticLocker)
                if (isStaticChanged)
                {
                    solid.Static = isStatic;
                    isStaticChanged = false;
                }

            lock (isInfluencedByGravityLocker)
                if (isInfluencedByGravityChanged)
                {
                    solid.InfluencedByGravity = isInfluencedByGravity;
                    isInfluencedByGravityChanged = false;
                }

            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_85");
            lock (shapes)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_85");
                if (shapesChanged)
                {
                    solid.ClearShapes();

                    foreach (ShapeData data in shapes)
                    {
                        solid.AddShape(data);
                    }

                    shapesChanged = false;
                }
            }

            while (forces.Count > 0)
            {
                solid.AddForce(forces.Dequeue());
            }

            lock (userDataLocker)
                if (userDataChanged)
                {
                    solid.UserData = userData;
                    userDataChanged = false;
                }

            lock (collisionEventProcessorLocker)
                if (collisionEventProcessorChanged)
                {
                    solid.CollisionEventHandler = collisionEventProcessor;
                    collisionEventProcessorChanged = false;
                }

            lock (massLocker)
                if (massChanged || massOffsetChanged)
                {
                    Debug.Assert(mass != null);
                    solid.SetMass(mass, massOffset);
                    massChanged = false;
                    massOffsetChanged = false;
                }

            lock (globalLinearVelLocker)
                if (globalLinearVelChanged)
                {
                    solid.GlobalLinearVel = globalLinearVel;
                    globalLinearVelChanged = false;
                    Debug.Assert(!float.IsNaN(solid.GlobalLinearVel.X));
                }

            lock (globalAngularVelLocker)
                if (globalAngularVelChanged)
                {
                    solid.GlobalAngularVel = globalAngularVel;
                    globalAngularVelChanged = false;
                    Debug.Assert(!float.IsNaN(solid.GlobalAngularVel.X));
                }
        }

        public void UpdateFromXpa()
        {
            if (disposed) return;
            if (solid == null) return;

            lock (positionLocker)
                if (!positionChanged)
                {
                    if (!float.IsNaN(solid.Position.X))
                    {
                        position = solid.Position;
                    }
                }

            lock (orientationLocker)
                if (!orientationChanged)
                {
                    if (!float.IsNaN(solid.Orientation.X))
                    {
                        orientation = solid.Orientation;
                    }
                }

            lock (globalLinearVelLocker)
                if (!globalLinearVelChanged)
                {
                    if (!float.IsNaN(solid.GlobalLinearVel.X))
                    {
                        globalLinearVel = solid.GlobalLinearVel;
                    }
                }

            lock (globalAngularVelLocker)
                if (!globalAngularVelChanged)
                {
                    if (!float.IsNaN(solid.GlobalAngularVel.X))
                    {
                        globalAngularVel = solid.GlobalAngularVel;
                    }
                }
        }

        public void AddForce(Force f)
        {
            forces.Enqueue(f);
        }

        public void AddShape(ShapeData data)
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_175");
            lock (shapes)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_175");

                shapesChanged = true;

                shapes.Add(data);
            }
        }

        public ShapeData GetShape(int id)
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_162");
            lock (shapes)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_162");
                if (shapes[id] != null) return shapes[id];
                else return solid.Data.GetShapeData(id);
            }
        }

        public void ClearShapes()
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_174");
            lock (shapes)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_174");

                shapesChanged = true;

                shapes.Clear();
            }
        }

        /// <summary>
        /// not thread safe !!! For use in Transformable.UpdateToXpa and Transformable.OnXpaChanged only
        /// </summary>
        public Vector3 Position
        {
            get
            {
                lock (positionLocker)
                {
                    return position;
                }
            }
            set
            {
                lock (positionLocker)
                {
                    positionChanged = true;
                    position = value;
                }
            }
        }

        /// <summary>
        /// not thread safe !!! For use in Transformable.UpdateToXpa and Transformable.OnXpaChanged only
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                lock (orientationLocker)
                {
                    return orientation;
                }
            }
            set
            {
                lock (orientationLocker)
                {
                    orientationChanged = true;
                    orientation = value;
                }
            }
        }

        /// <summary>
        /// not thread safe !!! For use in Transformable.UpdateToXpa and Transformable.OnXpaChanged only
        /// </summary>
        public Vector3 GlobalLinearVel
        {
            get
            {
                lock (globalLinearVelLocker)
                {
                    Debug.Assert(!float.IsNaN(globalLinearVel.X));
                    return globalLinearVel;
                }
            }
            set
            {
                lock (globalLinearVelLocker)
                {
                    globalLinearVelChanged = true;
                    globalLinearVel = value;
                }
            }
        }

        /// <summary>
        /// not thread safe !!! For use in Transformable.UpdateToXpa and Transformable.OnXpaChanged only
        /// </summary>
        public Vector3 GlobalAngularVel
        {
            get
            {
                lock (globalAngularVelLocker)
                {
                    return globalAngularVel;
                }
            }
            set
            {
                lock (globalAngularVelLocker)
                {
                    globalAngularVelChanged = true;
                    globalAngularVel = value;
                }
            }
        }

        public void SetMass(Mass newmass, Matrix offset)
        {
            lock (massLocker)
            {
                massChanged = true;
                massOffsetChanged = true;
                mass = newmass;
                massOffset = offset;
            }
        }

        public bool InfluencedByGravity
        {
            get
            {
                return isInfluencedByGravity;
            }
            set
            {
                lock (globalAngularVelLocker)
                {
                    isInfluencedByGravityChanged = true;
                    isInfluencedByGravity = value;
                }
            }
        }

        public float Mass
        {
            get
            {
                return mass.MassValue;
            }
        }

        public void Dispose()
        {
            disposed = true;
            physics.DestroyPhysicsObject(this);
        }

        public CollisionEventProcessor CollisionEventProcessor
        {
            get
            {
                return collisionEventProcessor;
            }
            set
            {
                lock (collisionEventProcessorLocker)
                {
                    collisionEventProcessorChanged = true;
                    collisionEventProcessor = value;
                }
            }
        }

        public bool Static
        {
            get
            {
                return isStatic;
            }
            set
            {
                lock (isStaticLocker)
                {
                    isStaticChanged = true;
                    isStatic = value;
                }
            }
        }

        public object UserData
        {
            get
            {
                return userData;
            }
            set
            {
                lock (userDataLocker)
                {
                    userDataChanged = true;
                    userData = value;
                }
            }
        }

        public Solid Solid
        {
            get
            {
                return solid;
            }
            set
            {
                if (disposed) return;

                solid = value;
            }
        }
    }
}
