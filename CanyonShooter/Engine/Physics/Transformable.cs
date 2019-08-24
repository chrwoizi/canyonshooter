using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CanyonShooter.GameClasses;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using CanyonShooter.GameClasses.World;
using System.Diagnostics;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.GameClasses.World.Canyon;

namespace CanyonShooter.Engine.Physics
{
    public class Transformable : ITransformable
    {
        #region Private Fields

        private ICanyonShooterGame game;

        private ITransformable self;
        private ICollisionActor collisionActor = null;

        private ITransformable parent;
        private List<ITransformable> childs = new List<ITransformable>();

        protected Matrix globalTransformation = Matrix.Identity;
        protected object globalTransformationLocker = new object();
        protected Vector3 globalPosition = Vector3.Zero;
        protected Quaternion globalRotation = new Quaternion(0, 0, 0, 1);
        protected Vector3 globalScale = Vector3.One;

        private Matrix localTransformation = Matrix.Identity;

        private Vector3 localPosition = Vector3.Zero;
        private Matrix localPositionAsMatrix = Matrix.Identity;

        private Quaternion localRotation = new Quaternion(0, 0, 0, 1);
        private Matrix localRotationAsMatrix = Matrix.Identity;

        private Vector3 localScale = Vector3.One;
        private Matrix localScaleAsMatrix = Matrix.Identity;

        private IPhysicsObject physicsObject;
        private Vector3 xpaPositionBackup;
        private Quaternion xpaRotationBackup;
        private List<ShapeData> shapes = new List<ShapeData>();

        public ReadOnlyCollection<ShapeData> Shapes
        {
            get
            {
                return new ReadOnlyCollection<ShapeData>(shapes);
            }
        }

        private bool nextPositionChanged = false;
        private bool nextRotationChanged = false;

        private static Mass DefaultMass = null;
        private static Matrix DefaultMassOffset = Matrix.Identity;

        private IQueue<QueueableCollisionEvent> collisionEvents;

        #endregion

        static Transformable()
        {
            DefaultMass = new Mass();
            DefaultMass.MassValue = 10.0f;
            DefaultMass.Center = new Vector3(0, 0, 0);
        }

        public Transformable(ICanyonShooterGame game)
        {
            Init(game, this, null);
        }

        public Transformable(ICanyonShooterGame game, ICollisionActor collisionActor)
        {
            Init(game, this, collisionActor);
        }

        public Transformable(ICanyonShooterGame game, ITransformable owner, ICollisionActor collisionActor)
        {
            Init(game, owner, collisionActor);
        }

        ~Transformable()
        {
            ConnectedToXpa = false;
        }

        private void Init(ICanyonShooterGame game, ITransformable owner, ICollisionActor collisionActor)
        {
            this.game = game;
            this.self = owner;
            this.collisionActor = collisionActor;

            if(game.Physics.MultiThreading)
            {
                collisionEvents = new LockingQueue<QueueableCollisionEvent>();
            }
            else
            {
                collisionEvents = new NonLockingQueue<QueueableCollisionEvent>();   
            }
        }

        /// <summary>
        /// calculates the relative values and calls recalcAbsolutes afterwards.
        /// </summary>
        private void RecalculateLocalValues()
        {
            localTransformation = localScaleAsMatrix * localRotationAsMatrix * localPositionAsMatrix;
            RecalculateGlobalValues();
        }

        /// <summary>
        /// Notifies derived classes when this has been changed
        /// </summary>
        protected virtual void OnTransform()
        {

        }

        public void BeforePhysicsSimulationStep()
        {
            physicsObject.UpdateToXpa();

            /*if (xpaSolid.UserData == CanyonShooter.GameClasses.World.Debris.DebrisEmitter.ttt)
            {
                Debug.Print("tox " + xpaSolid.GlobalLinearVel);
            }*/
        }

        public void AfterPhysicsSimulationStep()
        {
            physicsObject.UpdateFromXpa();

            /*if (xpaSolid.UserData == CanyonShooter.GameClasses.World.Debris.DebrisEmitter.ttt)
            {
                Debug.Print("frx " + xpaSolid.GlobalLinearVel);
            }*/
        }

        #region ITransformable Member

        /// <summary>
        /// calculates the absolute values. must be public because this calls the method on it's childs. do not call from other classes.
        /// </summary>
        public virtual void RecalculateGlobalValues()
        {
            if (parent != null)
            {
                globalTransformation = localTransformation * parent.GlobalTransformation;
                globalPosition = globalTransformation.Translation;
                globalRotation = Quaternion.Concatenate(localRotation, parent.GlobalRotation);
                globalRotation.Normalize();
                globalScale = parent.GlobalScale * localScale;

                Debug.Assert(!float.IsNaN(globalPosition.X));
            }
            else
            {
                globalTransformation = localTransformation;
                globalPosition = localPosition;
                globalRotation = localRotation;
                globalScale = localScale;

                Debug.Assert(!float.IsNaN(globalPosition.X));
            }

            foreach (ITransformable c in childs)
            {
                c.RecalculateGlobalValues();
            }

            // notify derived classes
            OnTransform();
        }

        public void AddChild(ITransformable c)
        {
            if (!childs.Contains(c))
            {
                childs.Add(c);
                c.Parent = self;
            }
        }

        public void RemoveChild(ITransformable c)
        {
            if (childs.Remove(c))
            {
                c.Parent = null;
            }
        }

        public Matrix GlobalTransformation
        {
            get
            {
                return globalTransformation;
            }
        }

        public Matrix LocalTransformation
        {
            get
            {
                return localTransformation;
            }
        }

        public Vector3 GlobalPosition
        {
            get
            {
                return globalPosition;
            }
        }

        public Vector3 LocalPosition
        {
            get
            {
                if (ConnectedToXpa)
                {
                    return localPosition;
                }
                else
                {
                    return localPosition;
                }
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));
                if (ConnectedToXpa)
                {
                    nextPositionChanged = true;

                    localPosition = value;
                    localPositionAsMatrix = Matrix.CreateTranslation(localPosition);
                    RecalculateLocalValues();
                }
                else
                {
                    localPosition = value;
                    localPositionAsMatrix = Matrix.CreateTranslation(localPosition);
                    RecalculateLocalValues();
                }
            }
        }

        public Quaternion GlobalRotation
        {
            get
            {
                return globalRotation;
            }
        }

        public Quaternion LocalRotation
        {
            get
            {
                if (ConnectedToXpa)
                {
                    return localRotation;
                }
                else
                {
                    return localRotation;
                }
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));

                if (ConnectedToXpa)
                {
                    nextRotationChanged = true;

                    localRotation = value;
                    localRotation.Normalize();
                    localRotationAsMatrix = Matrix.CreateFromQuaternion(localRotation);
                    RecalculateLocalValues();
                }
                else
                {
                    localRotation = Quaternion.Normalize(value);
                    localRotationAsMatrix = Matrix.CreateFromQuaternion(localRotation);
                    RecalculateLocalValues();
                }
            }
        }

        public Vector3 GlobalScale
        {
            get
            {
                return globalScale;
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return localScale;
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));

                localScale = value;
                localScaleAsMatrix = Matrix.CreateScale(localScale);
                RecalculateLocalValues();
            }
        }

        public void Move(Vector3 v)
        {
            LocalPosition += v;
        }

        public void Rotate(Quaternion q)
        {
            q.Normalize();
            LocalRotation = Quaternion.Concatenate(q, LocalRotation);
        }

        public void Scale(Vector3 v)
        {
            LocalScale = Vector3.Multiply(v, LocalScale);
        }

        public virtual ITransformable Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (value != parent)
                {
                    if (parent != null)
                    {
                        parent.RemoveChild(self);
                        parent = null;
                    }
                    parent = value;
                    if (parent != null)
                    {
                        if (ConnectedToXpa) throw new Exception("ConnectedToXpa und Parent geht nicht zusammen");
                        parent.AddChild(self);
                    }
                    RecalculateLocalValues();
                }
            }
        }

        public ReadOnlyCollection<ITransformable> Childs
        {
            get { return new ReadOnlyCollection<ITransformable>(childs); }
        }

        public bool ConnectedToXpa
        {
            get
            {
                return physicsObject != null;
            }
            set
            {
                if (value == ConnectedToXpa) return;

                if (value)
                {
                    if (Parent != null) throw new Exception("ConnectedToXpa und Parent geht nicht zusammen");

                    physicsObject = game.Physics.CreatePhysicsObject();

                    physicsObject.SetMass(DefaultMass, DefaultMassOffset);
                    physicsObject.CollisionEventProcessor = collisionActor != null
                                                           ? new CollisionActorEventHandler(game.Physics,
                                                                                            collisionActor)
                                                           : null;
                    physicsObject.UserData = self;

                    game.Physics.RegisterTransformable(this);
                }
                else
                {
                    physicsObject.Dispose();

                    game.Physics.UnregisterTransformable(this);
                }
            }
        }

        public Vector3 Velocity
        {
            get
            {
                if (parent != null)
                {
                    Debug.Assert(!float.IsNaN(parent.Velocity.X));
                    return parent.Velocity;
                }
                else
                {
                    if (physicsObject == null) return Vector3.Zero;
                    Debug.Assert(!float.IsNaN(physicsObject.GlobalLinearVel.X));
                    return physicsObject.GlobalLinearVel;
                }
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));

                if (physicsObject == null) throw new Exception("Velocity can only be used with ConnectedToXpa == true");

                physicsObject.GlobalLinearVel = value;
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                if (parent != null)
                {
                    Debug.Assert(!float.IsNaN(parent.AngularVelocity.X));
                    return parent.AngularVelocity;
                }
                else
                {
                    if (physicsObject == null)
                        throw new Exception("AngularVelocity can only be used with ConnectedToXpa == true");

                    Debug.Assert(!float.IsNaN(physicsObject.GlobalAngularVel.X));
                    return physicsObject.GlobalAngularVel;
                }
            }
            set
            {
                Debug.Assert(!float.IsNaN(value.X));

                if (physicsObject == null) throw new Exception("AngularVelocity can only be used with ConnectedToXpa == true");

                physicsObject.GlobalAngularVel = value;
            }
        }

        public ITransformable Self
        {
            get
            {
                return self;
            }
        }

        public ICollisionActor CollisionActor
        {
            get
            {
                return collisionActor;
            }
            set
            {
                collisionActor = value;
                if (physicsObject != null) if (collisionActor != null) physicsObject.CollisionEventProcessor = new CollisionActorEventHandler(game.Physics, collisionActor);
            }
        }

        public bool InfluencedByGravity
        {
            get
            {
                return physicsObject.InfluencedByGravity;
            }
            set
            {
                physicsObject.InfluencedByGravity = value;
            }
        }

        public bool Static
        {
            get
            {
                return physicsObject.Static;
            }
            set
            {
                physicsObject.Static = value;
            }
        }

        public void AddShape(ShapeData shape, ContactGroup group)
        {
            if (!shapes.Contains(shape))
            {
                shapes.Add(shape);
                ShapeData s = shape.Clone();
                s.ContactGroup = (int)group;
                if (!(s is MeshShapeData))
                {
                    s.Scale(GlobalScale.X);
                }
                physicsObject.AddShape(s);
            }
        }

        public ShapeData GetShape(int id)
        {
            return physicsObject.GetShape(id);
        }

        public void ClearShapes()
        {
            shapes.Clear();
            physicsObject.ClearShapes();
        }

        public float Mass
        {
            get
            {
                return physicsObject.Mass;
            }
            set
            {
                Mass m = new Mass();
                m.MassValue = value;
                physicsObject.SetMass(m, Matrix.Identity);
            }
        }

        public void AddCollisionEvent(QueueableCollisionEvent e)
        {
            collisionEvents.Enqueue(e);
        }

        public void AddForce(Force f)
        {
            physicsObject.AddForce(f);
        }

        public void Update(GameTime gameTime)
        {
            if (ConnectedToXpa)
            {
                ///////////// FROM SOLID

                bool update = false;

                if (parent != null)
                {
                    throw new Exception("ConnectedToXpa und Parent geht nicht zusammen");
                }

                if (!physicsObject.Orientation.Equals(xpaRotationBackup))
                {
                    Quaternion xpaRotation = Quaternion.Normalize(Quaternion.Inverse(physicsObject.Orientation));
                    xpaRotationBackup = physicsObject.Orientation;

                    if(!nextRotationChanged)
                    {
                        Debug.Assert(!float.IsNaN(xpaRotation.X));
                        localRotation = xpaRotation;   
                    }
                    localRotationAsMatrix = Matrix.CreateFromQuaternion(localRotation);

                    update = true;
                }

                if (!physicsObject.Position.Equals(xpaPositionBackup))
                {
                    Vector3 xpaPosition = physicsObject.Position;
                    xpaPositionBackup = xpaPosition;

                    if (!nextPositionChanged)
                    {
                        Debug.Assert(!float.IsNaN(xpaPosition.X));
                        localPosition = xpaPosition;
                    }
                    localPositionAsMatrix = Matrix.CreateTranslation(localPosition);

                    update = true;
                }

                if (update)
                {
                    RecalculateLocalValues();
                }


                ///////////// TO SOLID 

                if (nextPositionChanged)
                {
                    nextPositionChanged = false;
                    physicsObject.Position = localPosition;
                    Debug.Assert(!float.IsNaN(physicsObject.Position.X));
                }

                if (nextRotationChanged)
                {
                    nextRotationChanged = false;
                    physicsObject.Orientation = Quaternion.Inverse(localRotation);
                    Debug.Assert(!float.IsNaN(physicsObject.Orientation.X));
                }


                ////// COLLISION EVENTS

                //string mname = "Transformable at " + globalPosition + ": " + collisionEvents.Count + " Collision Events";
                //Helper.Helper.BeginTimeMeasurementDebugOutput(mname);
                while (collisionEvents.Count > 0)
                {
                    QueueableCollisionEvent d = collisionEvents.Dequeue();
                    d.Invoke();
                }
                //Helper.Helper.EndTimeMeasurementDebugOutput(mname);
            }
        }


        #endregion

        #region IDisposable Member

        public virtual void Dispose()
        {
            ConnectedToXpa = false;

            collisionActor = null;

            self = null;

            collisionEvents.Clear();

            if(physicsObject!=null) physicsObject.Dispose();

            game.Physics.UnregisterTransformable(this);
        }

        #endregion

    }
}
