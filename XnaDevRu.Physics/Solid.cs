using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// Solids are the physical objects in a simulation.  Solids can be
    /// static or dynamic: basically, dynamic Solids move, and static Solids
    /// don't move.  (Static Solids can still be positioned manually.)  All
    /// Solids start out enabled, but they don't do much until Shapes are added.
    /// </summary>
    public abstract class Solid : IDisposable
    {
        #region fields

        public object Tag; //Can be field

        /// An internal list of this Solid's pending Forces.
        protected List<Force> forceList;

        /// Stores data describing the Solid.
        protected SolidData data;

        /// The axis-aligned bounding box of all shapes in local
        /// coordinates.  This array stores data in the following order:
        /// min x, max x, min y, max y, min z, max z.  This gets updated
        /// whenever a shape is added or removed.
        protected BoundingBox localAABB;

        /// Pointer to this Solid's collision event handler.
        protected CollisionEventProcessor collisionEventHandler;

        /// Pointer to user data.  This is totally user-managed (i.e. OPAL
        /// will never delete it).
        protected object userData;

        /// Data used for
        private bool isMoving;
        private MovementEventProcessor movementEventHandler;

		protected bool disposed = false;
		public bool Disposed { get { return disposed; } }
        #endregion

        public Solid()
        {
            // "mData" is initialized in its own constructor.
            forceList = new List<Force>();
            ResetAABB();

            this.Moving = true;
        }

		~Solid()
		{
			if (!disposed)
			{
				disposed = true;
				Dispose(false);
			}
		}

        /// <summary>
        /// Initializes the Solid with the given data structure.  Calling
        /// this more than once will automatically destroy all the old
        /// Shapes before adding new ones.
        /// </summary>
        /// <param name="data"></param>
        public abstract void Init(SolidData data);

        /// <summary>
        /// Returns all data describing the Solid.
        /// </summary>
        public SolidData Data
        {
            get 
            {
                // Update parameters that don't get updated automatically.
                data.Sleeping = this.Sleeping;

                return data;
            }
        }

        /// <summary>
        /// Gets/Sets the Solid's name.
        /// </summary>
        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }


        /// Returns true if the Solid is enabled.
        /// Sets whether the Solid can collide with other Solids and be
        /// physically simulated.  Forces applied to this Solid while
        /// disabled will be ignored.
        public virtual bool Enabled
        {
            get { return data.Enabled; }
            set { data.Enabled = value; }
        }


        /// Returns true if this is a static Solid.
        /// Sets whether the Solid should be static or dynamic.
        public virtual bool Static
        {
            get { return data.IsStatic; }
            set { data.IsStatic = value; }
        }

        /// Removes the Solid from its current Space and adds it to the new
        /// Space.
        public abstract SpaceBase Space { set; }

        /// <summary>
        /// Sets whether the Solid is sleeping (i.e. set this to false to
        /// wake up a sleeping Solid).
        /// Returns true if the Solid is sleeping.  If the Solid is static,
        /// this will always return true.
        /// </summary>
        public abstract bool Sleeping { get; set; }

        /// <summary>
        /// Gets/Sets the Solid's sleepiness level.
        /// </summary>
        public virtual float Sleepiness
        {
            get { return data.Sleepiness; }
            set
            {
                if (value >= 0.0 && value <= 1.0)
                    data.Sleepiness = value;
            }
        }

        /// <summary>
        /// Gets/Sets the amount of linear damping on this Solid.
        /// </summary>
        public virtual float LinearDamping
        {
            get { return data.LinearDamping; }
            set 
            {
                if (value >= 0.0)
                    data.LinearDamping = value;
            }
        }


        /// <summary>
        /// Gets/Sets the amount of angular damping on this Solid.
        /// </summary>
        public virtual float AngularDamping
        {
            get { return data.AngularDamping; }
            set 
            {
                if (value >= 0.0)
                    data.AngularDamping = value;
            }
        }

        /// <summary>
        /// Set this Solid's user data pointer to some external data.  This
        /// can be used to let the Solid point to some user object (e.g. an
        /// object with a visual mesh).  The user data is totally user-managed
        /// </summary>
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        /// <summary>
        /// Gets/Sets this Solid's transform matrix.
        /// </summary>
        public virtual Matrix Transform
        {
            get { return data.Transform; }
            set 
            {
                data.Transform = value;
                InternalUpdateEngineTransform();
            }
        }

        /// <summary>
        /// Sets the position of this Solid in global coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void SetPosition(float x, float y, float z)
        {
            isMoving = true;
            data.Transform.Translation = new Vector3(x, y, z);
            InternalUpdateEngineTransform();
        }

        /// <summary>
        /// Gets/Sets the position of this Solid in global coordinates.
        /// </summary>
        public virtual Vector3 Position
        {
            get { return data.Transform.Translation; }
            set 
            {
                isMoving = true;
                data.Transform.Translation = value;
                InternalUpdateEngineTransform();
            }
        }

        /// <summary>
        /// Returns the euler angles of the Solid's orientation.
        /// </summary>
        public virtual Vector3 EulerXYZ
        {
            get { return MathUtil.GetEulerAngles(data.Transform); }
        }

        /// <summary>
        /// Gets/Sets a quaternion representing the Solid's orientation.
        /// </summary>
        public virtual Quaternion Orientation
        {
            get { return Quaternion.CreateFromRotationMatrix(data.Transform); }
            set 
            {
                SetQuaternion(value.W, value.X, value.Y, value.Z);
            }
        }


        /// <summary>
        /// Sets a quaternion representing the Solid's orientation.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void SetQuaternion(float w, float x, float y, float z)
        {
            isMoving = true;

            MathUtil.SetQuaternion(ref data.Transform, w, x, y, z);

            InternalUpdateEngineTransform();
        }

        /// <summary>
        /// Returns the axis-aligned bounding box for all of the Solid's
        /// shapes relative to the Solid.
        /// </summary>
        public virtual BoundingBox LocalAABB
        {
            get { return localAABB; }
        }

        /// <summary>
        /// Returns the axis-aligned bounding box for all of the Solid's
        /// shapes in global coordinates.
        /// </summary>
        public virtual BoundingBox GlobalAABB
        {
            get 
            {
                // Transform the AABB extents to global coordinates.
                Vector3 min = Vector3.Transform( localAABB.Min, data.Transform);
                Vector3 max = Vector3.Transform( localAABB.Max, data.Transform);

                BoundingBox bbox = new BoundingBox(min, max);

                return bbox;

            }
        }

        /// <summary>
        /// Removes all shapes from this Solid.  Resets the Solid's
        /// axis-aligned bounding box.
        /// </summary>
        public abstract void ClearShapes();

        /// <summary>
        /// Adds a Shape to this Solid.  Updates the Solid's axis-aligned
        /// bounding box.
        /// </summary>
        /// <param name="data"></param>
        public abstract void AddShape(ShapeData data);

        /// <summary>
        /// Applies a force/torque to this Solid.  If the Solid is disabled,
        /// the Solid is static, or the magnitude of the force/torque is
        /// zero, this will do nothing.
        /// </summary>
        /// <param name="f"></param>
        public virtual void AddForce(Force f)
        {
            if (!data.Enabled)
                return;

            if (data.IsStatic)
                return;

            if (!MathUtil.AreEqual(f.Direction.LengthSquared(), 0))
            {
                forceList.Add(f);
            }
        }

        /// <summary>
        /// Removes all forces and torques currently affecting this Solid.
        /// </summary>
        public abstract void ZeroForces();

        /// <summary>
        /// Sets the Solid's linear velocity in local coordinates.
        /// </summary>
        public abstract Vector3 LocalLinearVel { get; set; }

        /// <summary>
        /// Given an offset point relative to the Solid's local origin,
        /// returns the linear velocity of the point in local coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract Vector3 GetLocalLinearVelAtLocalPos(Vector3 p);

        /// <summary>
        /// Gets/Sets the Solid's angular velocity in local coordinates.
        /// </summary>
        public abstract Vector3 LocalAngularVel { get; set;}


        /// <summary>
        /// Gets/Sets the Solid's linear velocity in global coordinates.
        /// </summary>
        public abstract Vector3 GlobalLinearVel { get; set;}

        /// <summary>
        /// Given an offset point relative to the Solid's local origin,
        /// returns the linear velocity of the point in global coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract Vector3 GetGlobalLinearVelAtLocalPos(Vector3 p);

        /// <summary>
        /// Gets/Sets the Solid's angular velocity in global coordinates.
        /// </summary>
        public abstract Vector3 GlobalAngularVel { get; set;}

        /// <summary>
        /// Set a new mass, center, and intertia for the solid.
        /// </summary>
        /// <param name="newmass"></param>
        /// <param name="offset"></param>
        public abstract void SetMass(Mass newmass, Matrix offset);

        /// <summary>
        /// Translate the Solid's mass using the given offset vector
        /// specified in global coordinates.
        /// </summary>
        /// <param name="offset"></param>
        public abstract void TranslateMass(Vector3 offset);

        /// <summary>
        /// Returns the Solid's collision event handler.  If this returns
        /// NULL, the Solid is not using one.
        /// </summary>
        public virtual CollisionEventProcessor CollisionEventHandler
        {
            get { return collisionEventHandler; }
            set { collisionEventHandler = value; }
        }

        //// Quickly spinning solids should be set as fast rotating solids to
        //// improve simulation accuracy.
        //virtual void  setFastRotation(bool fast);

        //virtual bool  getFastRotation()const;

        //virtual void  setFastRotationAxis(Vec3r axis);

        /// <summary>
        /// Gets or sets a value indicating whether [influenced by gravity].
        /// </summary>
        /// <value><c>true</c> if [influenced by gravity]; otherwise, <c>false</c>.</value>
        public abstract bool InfluencedByGravity { get; set; }

        /// <summary>
        /// Returns the Solid's mass.  This will return 0 if the Solid is static.
        /// </summary>
        public abstract float Mass { get; }

        /// <summary>
        /// Returns the Solid's inertia tensor as a 4x4 matrix.  This will
        /// be the identity matrix if the Solid is static.
        /// </summary>
        public abstract Matrix InertiaTensor { get; }

        /// <summary>
        /// Returns true if the solid has moved since last call of Solid::isMoving()
        /// Manual set of a moving state
        /// </summary>
        public virtual bool Moving
        {
            get 
            {
                if (isMoving)
                {
                    isMoving = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { isMoving = value; }
        }


        /// <summary>
        /// Gets/Sets the Solid's movement event handler.  If this returns
        /// NULL, the Solid is not using one.
        /// </summary>
        public virtual MovementEventProcessor MovementEventHandler
        {
            get { return movementEventHandler; }
            set { movementEventHandler = value; }
        }

        /// <summary>
        /// Update the OPAL transform using the physics engine transform.
        /// </summary>
		public abstract void InternalUpdateOPALTransform();

        /// <summary>
        /// Update the physics engine transform using the OPAL transform.
        /// </summary>
		public abstract void InternalUpdateEngineTransform();

        /// <summary>
        /// Loops over the Solid's list of Forces and applies them.
        /// </summary>
        /// <param name="stepSize"></param>
        public virtual void InternalApplyForces(float stepSize)
        {
            if (data.IsStatic)
                return;
            
            if (forceList.Count == 0)
                return;

            // If there are Forces to apply and the Solid is asleep, wake it up.
            if (this.Sleeping)
            {
                Sleeping = false;
            }

            float invStepSize = 1 / stepSize;

            for (int i = 0; i < forceList.Count; )
            {
                Force f = forceList[i];

                if (f.SingleStep)
                {
                    f.Duration = stepSize;
                }
                else if (forceList[i].Duration < stepSize)
                {
                    // Scale the size of the force/torque.
                    f.Direction *= (forceList[i].Duration * invStepSize);
                }

                // Apply the actual force/torque.
                ApplyForce(f);

                // The following is ok for all cases (even when duration is
                // < mStepSize).
                f.Duration -= stepSize;

                if (f.Duration <= 0)
                {
                    // Delete this force.
                    //mForceList[i] = mForceList.back();
                    //mForceList.pop_back();

                    forceList.RemoveAt(i);
                }
                else
                {
                    forceList[i] = f;
                    ++i;
                }
            }
        }

        /// <summary>
        /// Updates the SolidData sleeping value from the physics engine.
        /// </summary>
		public void InternalUpdateSleeping()
        {
            data.Sleeping = Sleeping;
        }


        /// <summary>
        /// Physics engine-specific function for applying Forces to
        /// Solids.
        /// </summary>
        /// <param name="f"></param>
        protected abstract void ApplyForce(Force f);

        /// <summary>
        /// Adds the given axis-aligned bounding box to the Solid's AABB.
        /// </summary>
        /// <param name="aabb"></param>
        protected void AddToLocalAABB(BoundingBox aabb)
        {
            // Loop over the 3 dimensions of the AABB's extents.
            //for (int i = 0; i < 3; ++i)
            //{
            //    if (aabb[i * 2] < mLocalAABB[i * 2])
            //    {
            //        mLocalAABB[i * 2] = aabb[i * 2];
            //    }

            //    if (aabb[i * 2 + 1] > mLocalAABB[i * 2 + 1])
            //    {
            //        mLocalAABB[i * 2 + 1] = aabb[i * 2 + 1];
            //    }
            //}

            localAABB = BoundingBox.CreateMerged(localAABB, aabb); // XNA rulezzz
        }

        /// <summary>
        /// Resets the Solid's axis-aligned bounding box.
        /// </summary>
        protected void ResetAABB()
        {
			localAABB = new BoundingBox();
			localAABB.Max = Vector3.Zero;
			localAABB.Min = Vector3.Zero;
        }

        public override string ToString()
        {
            return this.GetType().Name + " " + this.Name + " enabled=" + this.Enabled + " static=" + this.Static;
        }


		#region IDisposable Members
		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected abstract void Dispose(bool disposing);
		#endregion
	}
}
