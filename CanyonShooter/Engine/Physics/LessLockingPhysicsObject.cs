using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CanyonShooter.GameClasses;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    public class LessLockingPhysicsObject : IPhysicsObject
    {
        #region Temporary Data
        private Vector3 globalLinearVel = Vector3.Zero;
        private bool globalLinearVelChanged = false;
        private Vector3 globalAngularVel = Vector3.Zero;
        private bool globalAngularVelChanged = false;
        private Vector3 position = Vector3.Zero;
        private bool positionChanged = false;
        private Quaternion orientation = Quaternion.Inverse(Quaternion.Identity);
        private bool orientationChanged = false;
        private bool isInfluencedByGravity = true;
        private bool isInfluencedByGravityChanged = false;
        private bool isStatic = false;
        private bool isStaticChanged = false;
        private CollisionEventProcessor collisionEventProcessor = null;
        private bool collisionEventProcessorChanged = false;
        private Mass mass;
        private bool massChanged = false;
        private Matrix massOffset;
        private bool massOffsetChanged = false;
        private object userData;
        private bool userDataChanged = false;
        private List<ShapeData> shapes = new List<ShapeData>();
        private bool shapesChanged = false;
        private NonLockingQueue<Force> forces = new NonLockingQueue<Force>();

        private object locker = new object();

        private bool disposed = false;
        #endregion

        private Solid solid;
        private IPhysics physics;

        public LessLockingPhysicsObject(IPhysics physics)
        {
            this.physics = physics;
        }

        public void UpdateToXpa()
        {
            if (disposed) return;
            if (solid == null) return;

            lock (locker)
            {
                if (positionChanged)
                {
                    solid.Position = position;
                    positionChanged = false;
                    Debug.Assert(!float.IsNaN(solid.Position.X));
                }

                if (orientationChanged)
                {
                    solid.Orientation = orientation;
                    orientationChanged = false;
                    Debug.Assert(!float.IsNaN(solid.Orientation.X));
                }

                if (isStaticChanged)
                {
                    solid.Static = isStatic;
                    isStaticChanged = false;
                }

                if (isInfluencedByGravityChanged)
                {
                    solid.InfluencedByGravity = isInfluencedByGravity;
                    isInfluencedByGravityChanged = false;
                }

                if (shapesChanged)
                {
                    solid.ClearShapes();

                    foreach (ShapeData data in shapes)
                    {
                        solid.AddShape(data);
                    }

                    shapesChanged = false;
                }

                if (userDataChanged)
                {
                    solid.UserData = userData;
                    userDataChanged = false;
                }

                if (collisionEventProcessorChanged)
                {
                    solid.CollisionEventHandler = collisionEventProcessor;
                    collisionEventProcessorChanged = false;
                }

                if (massChanged || massOffsetChanged)
                {
                    Debug.Assert(mass != null);
                    solid.SetMass(mass, massOffset);
                    massChanged = false;
                    massOffsetChanged = false;
                }

                if (globalLinearVelChanged)
                {
                    solid.GlobalLinearVel = globalLinearVel;
                    globalLinearVelChanged = false;
                    Debug.Assert(!float.IsNaN(solid.GlobalLinearVel.X));
                }

                if (globalAngularVelChanged)
                {
                    solid.GlobalAngularVel = globalAngularVel;
                    globalAngularVelChanged = false;
                    Debug.Assert(!float.IsNaN(solid.GlobalAngularVel.X));
                }

                while (forces.Count > 0)
                {
                    solid.AddForce(forces.Dequeue());
                }
            }
        }

        public void UpdateFromXpa()
        {
            if (disposed) return;
            if (solid == null) return;

            lock (locker)
            {
                if (!positionChanged)
                {
                    if (!float.IsNaN(solid.Position.X))
                    {
                        position = solid.Position;
                    }
                }

                if (!orientationChanged)
                {
                    if (!float.IsNaN(solid.Orientation.X))
                    {
                        orientation = solid.Orientation;
                    }
                }

                if (!globalLinearVelChanged)
                {
                    if (!float.IsNaN(solid.GlobalLinearVel.X))
                    {
                        globalLinearVel = solid.GlobalLinearVel;
                    }
                }

                if (!globalAngularVelChanged)
                {
                    if (!float.IsNaN(solid.GlobalAngularVel.X))
                    {
                        globalAngularVel = solid.GlobalAngularVel;
                    }
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
            lock (locker)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_175");

                shapesChanged = true;

                shapes.Add(data);
            }
        }

        public ShapeData GetShape(int id)
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_162");
            lock (locker)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("solid_162");
                if (shapes[id] != null) return shapes[id];
                else return solid.Data.GetShapeData(id);
            }
        }

        public void ClearShapes()
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("solid_174");
            lock (locker)
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
                lock (locker)
                {
                    return position;
                }
            }
            set
            {
                lock (locker)
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
                lock (locker)
                {
                    return orientation;
                }
            }
            set
            {
                lock (locker)
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
                lock (locker)
                {
                    Debug.Assert(!float.IsNaN(globalLinearVel.X));
                    return globalLinearVel;
                }
            }
            set
            {
                lock (locker)
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
                lock (locker)
                {
                    return globalAngularVel;
                }
            }
            set
            {
                lock (locker)
                {
                    globalAngularVelChanged = true;
                    globalAngularVel = value;
                }
            }
        }

        public void SetMass(Mass newmass, Matrix offset)
        {
            lock (locker)
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
                lock (locker)
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
                lock (locker)
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
                lock (locker)
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
                lock (locker)
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
