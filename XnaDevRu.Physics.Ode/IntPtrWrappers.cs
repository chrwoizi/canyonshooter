using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics.Ode
{
    public abstract class PtrWrapper : IDisposable
    {
        // CanyonShooter BEGIN

        // commented out
        //public static List<WeakReference> references = new List<WeakReference>();

        // CanyonShooter END

        protected IntPtr _ptr;

        public PtrWrapper(IntPtr ptr)
        {
            _ptr = ptr;

            // CanyonShooter BEGIN

            // commented out
            //references.Add(new WeakReference(this));

            // CanyonShooter END
        }

        public static bool IsEmpty(PtrWrapper wrp)
        {
            if (wrp == null)
                return true;

            return wrp._ptr == IntPtr.Zero;
        }

        public bool IsNull()
        {
            return _ptr == IntPtr.Zero;
        }
        public bool IsNotNull()
        {
            return _ptr != IntPtr.Zero;
        }

        public bool IsNull(bool throws)
        {
            bool empty = _ptr == IntPtr.Zero;

            if (empty)
                if (throws)
                    throw new InvalidOperationException(GetType().Name + " PtrWrapper is used but its unmanaged pointer is empty");

            return empty;
        }

        public IntPtr Ptr
        {
            get { return _ptr; }
        }

        ~PtrWrapper()
        {
            
        }

        #region IDisposable Members

        public void Dispose()
        {
            _ptr = IntPtr.Zero;
        }

        #endregion

        public override string ToString()
        {
            return base.ToString() + (IsNull() ? " (null)" : "");
        }
    }

    /// <summary>
    /// dynamics world
    /// </summary>
    public class dWorldID : PtrWrapper
    {
        public dWorldID(IntPtr ptr):base(ptr)
        {
        }

        /// <summary>
        /// Step the world.
        /// This uses an iterative method that takes time on the order of m*N and memory on the order of m, where
        /// m is the total number of constraint rows and N is the number of iterations.
        ///
        /// For large systems this is a lot faster than dWorldStep, but it is less accurate.
        /// </summary>
        /// <remarks>
        /// QuickStep is great for stacks of objects especially when the auto-disable feature is used as well.
        /// However, it has poor accuracy for near-singular systems. Near-singular systems can occur when using
        /// high-friction contacts, motors, or certain articulated structures.
        /// For example, a robot with multiple legs sitting on the ground may be near-singular.
        ///
        /// There are ways to help overcome QuickStep's inaccuracy problems:
        /// 	- 	Increase CFM.
        /// 	-	Reduce the number of contacts in your system (e.g. use the minimum number of contacts for
        ///     	the feet of a robot or creature).
        ///		-	Don't use excessive friction in the contacts.
        ///		-	Use contact slip if appropriate
        ///		-	Avoid kinematic loops (however, kinematic loops are inevitable in legged creatures).
        ///		-	Don't use excessive motor strength.
        ///		-	Use force-based motors instead of velocity-based motors.
        ///
        /// Increasing the number of QuickStep iterations may help a little bit, but it is not going to help much
        /// if your system is really near singular.
        /// </remarks>
        /// <param name="stepSize">the stepsize</param>
        public void QuickStep(float stepSize)
        {
            if (IsNull(true))
                return;

            Tao.Ode.Ode.dWorldQuickStep(this, stepSize);
        }

        public Vector3 Gravity
        {
            get
            {
                Tao.Ode.Ode.dVector3 tmp = new Tao.Ode.Ode.dVector3();

                Tao.Ode.Ode.dWorldGetGravity(this, ref tmp);

                return tmp;
            }
            set
            {
                Tao.Ode.Ode.dWorldSetGravity(this, value.X, value.Y, value.Z);
            }
        }

        public static implicit operator IntPtr(dWorldID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dWorldID(IntPtr ptr)
        {
            return new dWorldID(ptr);
        }

        /// <summary>
        /// Create a body in this world with default mass parameters at position (0,0,0).
        /// Return its ID (really a handle to the body).
        /// </summary>
        /// <returns></returns>
        public dBodyID CreateBody()
        {
            if (IsNull())
                return new dBodyID(IntPtr.Zero); // may be we should throw an exception here (mike)

            return new dBodyID(Tao.Ode.Ode.dBodyCreate(this));
        }

    }

    /// <summary>
    /// geometry (collision object) 
    /// </summary>
    public class dGeomID : PtrWrapper
    {
        bool _enabled;

        public dGeomID(IntPtr ptr):base(ptr)
        {
        }

        public void Destroy()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dGeomDestroy(this);

            Dispose();
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (IsNull())
                    return;

                _enabled = value;

                if (value)
                    Tao.Ode.Ode.dGeomEnable(this);
                else
                    Tao.Ode.Ode.dGeomDisable(this);

            }
        }

        public float[] AABB
        {
            get
            {
                float[] aabb = new float[6];

                Tao.Ode.Ode.dGeomGetAABB(this, aabb);

                return aabb;
            }
        }

        /// <summary>
        /// Gets/Set the body associated with a placeable geom.
        /// </summary>
        public dBodyID RigidBody
        {
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dGeomSetBody(this, value);
            }
            get
            {
                if (IsNull())
                    return null;

                return Tao.Ode.Ode.dGeomGetBody(this);
            }

        }

        /// <summary>
        /// Determine if a geom is a space.
        /// </summary>
        public bool IsSpace
        {
            get
            {
                if (IsNull(true))
                    return true;

                /// <returns>An int, non-zero if the given geom is a space, zero otherwise.</returns>
                return Tao.Ode.Ode.dGeomIsSpace(this) != 0;
            }
        }

        public Vector3 Position
        {
            set
            {
                Tao.Ode.Ode.dGeomSetPosition(this, value.X, value.Y, value.Z);
            }
        }

        public Matrix Rotation
        {
            set
            {
                Tao.Ode.Ode.dGeomSetRotation(this, value);
            }
        }

        public static implicit operator IntPtr(dGeomID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dGeomID(IntPtr ptr)
        {
            return new dGeomID(ptr);
        }

    }
    
    /// <summary>
    /// collision space 
    /// </summary>
    public class dSpaceID : PtrWrapper
    {
        public dSpaceID(IntPtr ptr):base(ptr)
        {
        }

        /// <summary>
        /// Create a new geometry transform object, and return its ID.
        /// </summary>
        /// <returns></returns>
        public dGeomID CreateGeomTransform()
        {
            return Tao.Ode.Ode.dCreateGeomTransform(this);
        }


        /// <summary>
        /// Create a box geom of the given x/y/z side lengths (lx,ly,lz), and return
        /// its ID.
        ///
        /// If this space is nonzero, insert it into that space. The point of reference
        /// for a box is its center.
        /// </summary>
        /// <returns>A dGeomID</returns>
        /// <param name="space">the space the box should be inserted to</param>
        /// <param name="lx">length of the x side</param>
        /// <param name="ly">length of the y side</param>
        /// <param name="lz">length of the z side</param>
        public dGeomID CreateBoxGeom(float lx, float ly, float lz)
        {
            return Tao.Ode.Ode.dCreateBox(this, lx, ly, lz);
        }

        /// <summary>
        /// Create a sphere geom of the given radius, and return its ID. If space is
        /// nonzero, insert it into that space. The point of reference for a sphere
        /// is its center.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public dGeomID CreateSphereGeom(float radius)
        {
            return Tao.Ode.Ode.dCreateSphere(this, radius);
        }

        /// <summary>
        /// Create a (flat-ended) cylinder geom of the given parameters, and return its ID.
        /// If space is nonzero, insert it into that space.
        ///
        /// The cylinder's length is given by length. The cylinder is aligned along the geom's
        /// local Z axis.
        /// The radius of the cylinder is given by radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public dGeomID CreateCylinderGeom(float radius, float lenght)
        {
            return Tao.Ode.Ode.dCreateCylinder(this, radius, lenght);
        }

        /// <summary>
        /// Create a plane geom of the given parameters, and return its ID.
        ///
        /// If space is nonzero, insert it into that space. The plane equation is
        ///		a*x+b*y+c*z = d
        ///
        /// The plane's normal vector is (a,b,c), and it must have length 1.
        ///
        /// Planes are non-placeable geoms. This means that, unlike placeable geoms,
        /// planes do not have an assigned position and rotation. This means that
        /// the parameters (a,b,c,d) are always in global coordinates. In other words
        /// it is assumed that the plane is always part of the static environment and
        /// not tied to any movable object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public dGeomID CreatePlaneGeom(float a, float b, float c, float d)
        {
            return Tao.Ode.Ode.dCreatePlane(this, a, b, c, d);
        }

        /// <summary>
        /// Trimesh class constructor.
        ///
        /// The Data member defines the vertex data the newly created triangle mesh will use.
        ///
        /// Callbacks are optional.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Callback"></param>
        /// <param name="ArrayCallback"></param>
        /// <param name="RayCallback"></param>
        /// <returns></returns>
        public dGeomID CreateTriMeshGeom(dTriMeshDataID Data, Tao.Ode.Ode.dTriCallback Callback, Tao.Ode.Ode.dTriArrayCallback ArrayCallback, Tao.Ode.Ode.dTriRayCallback RayCallback)
        {
            return Tao.Ode.Ode.dCreateTriMesh(this, Data, Callback, ArrayCallback, RayCallback);
        }

        /// <summary>
        /// Trimesh class constructor.
        ///
        /// The Data member defines the vertex data the newly created triangle mesh will use.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public dGeomID CreateTriMeshGeom(dTriMeshDataID Data)
        {
            return Tao.Ode.Ode.dCreateTriMesh(this, Data, null, null, null);
        }

		/// <summary>
		/// This determines which pairs of geoms in a space may potentially intersect,
		/// and calls the callback function with each candidate pair.
		///
		/// The callback function is of type dNearCallback, which is defined as:
		///		typedef void dNearCallback (void *data, dGeomID o1, dGeomID o2);
		///
		/// The data argument is passed from dSpaceCollide directly to the callback
		/// function. Its meaning is user defined.
		/// The o1 and o2 arguments are the geoms that may be near each other.
		/// The callback function can call dCollide on o1 and o2 to generate contact
		/// points between each pair. Then these contact points may be added to the
		/// simulation as contact joints. The user's callback function can of course
		/// chose not to call dCollide for any pair, e.g. if the user decides that
		/// those pairs should not interact.
		///
		/// Other spaces that are contained within the colliding space are not treated
		/// specially, i.e. they are not recursed into. The callback function may be
		/// passed these contained spaces as one or both geom arguments.
		///
		/// dSpaceCollide() is guaranteed to pass all intersecting geom pairs to the
		/// callback function, but it may also make mistakes and pass non-intersecting
		/// pairs. The number of mistaken calls depends on the internal algorithms
		/// used by the space. Thus you should not expect that dCollide will return
		/// contacts for every pair passed to the callback.
		/// </summary>
        public void Collide()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dSpaceCollide(this, IntPtr.Zero, OdeHelper.CollisionCallbackRunner);
        }

        /// <summary>
        /// Create a simple space.
        /// </summary>
        /// <returns></returns>
        public static dSpaceID Create()
        {
            return Tao.Ode.Ode.dSimpleSpaceCreate(IntPtr.Zero);
        }

        /// <summary>
        /// Create a multi-resolution hash table space.
        /// </summary>
        /// <returns></returns>
        public static dSpaceID HashSpaceCreate()
        {
            return Tao.Ode.Ode.dHashSpaceCreate(IntPtr.Zero);
        }

        /// <summary>
        /// Sets some parameters for a multi-resolution hash table space.
        ///
        /// The smallest and largest cell sizes used in the hash table will be
        /// 2^minlevel and 2^maxlevel respectively.
        ///
        /// minlevel must be less than or equal to maxlevel.
        /// </summary>
        public void SetLevels(int minlevel, int maxlevel)
        {
            Tao.Ode.Ode.dHashSpaceSetLevels(this, minlevel, maxlevel);
        }

        /// <summary>
        /// Remove a geom from a space.
        ///
        /// This does nothing if the geom is not actually in the space.
        ///
        /// This function is called automatically by dGeomDestroy if the geom is in a space.
        /// </summary>
        public void Remove(dGeomID geomId)
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dSpaceRemove(this, geomId);
        }

        /// <summary>
        /// Add a geom to a space.
        ///
        /// This does nothing if the geom is already in the space.
        ///
        /// This function can be called automatically if a space argument is given to
        /// a geom creation function.
        /// </summary>
        public void Add(dGeomID geomId)
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dSpaceAdd(this, geomId);
        }

        public static implicit operator IntPtr(dSpaceID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dSpaceID(IntPtr ptr)
        {
            return new dSpaceID(ptr);
        }

    }
    
    /// <summary>
    /// a dTriMeshData object which is used to store mesh data.
    /// </summary>
    public class dTriMeshDataID : PtrWrapper
    {

        public dTriMeshDataID(IntPtr ptr):base(ptr)
        {
        }

        public void Destroy()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dGeomTriMeshDataDestroy(this);

            Dispose();
        }

        /// <summary>
        /// Create a dTriMeshData object which is used to store mesh data.
        /// </summary>
        /// <returns></returns>
        public static dTriMeshDataID Create()
        {
            return Tao.Ode.Ode.dGeomTriMeshDataCreate();
        }

        public void BuildSingle( Vector3[] Vertices, int VertexStride, int VertexCount, int[] Indices, int IndexCount, int TriStride)
        {
            if(Vertices == null)
                return;

            if(Vertices.Length == 0)
                return;

            Tao.Ode.Ode.dVector3[] buf = new Tao.Ode.Ode.dVector3[VertexCount];

            for(int i =0; i<VertexCount; i++)
                buf[i] = Vertices[i];

            Tao.Ode.Ode.dGeomTriMeshDataBuildSingle(this, buf, VertexStride, VertexCount, Indices, IndexCount, TriStride);
        }


        public static implicit operator IntPtr(dTriMeshDataID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dTriMeshDataID(IntPtr ptr)
        {
            return new dTriMeshDataID(ptr);
        }

    }
    
    /// <summary>
    /// rigid body (dynamics object)
    /// </summary>
    public class dBodyID : PtrWrapper
    {
        bool _enabled;

        public dBodyID(IntPtr ptr):base(ptr)
        {
        }

        /// <summary>
        /// Set the auto-disable flag of a body.
        ///
        /// If the AutoDisable is true the body will be automatically disabled 
        /// when it has been idle for long enough.
        /// </summary>
        public bool AutoDisable
        {
            set
            {
                if (IsNull())
                    return;

                int disable = value == true ? 1 : 0;

                Tao.Ode.Ode.dBodySetAutoDisableFlag(this, disable);
            }
        }

        /// <summary>
        /// Set a body's linear velocity threshold for automatic disabling.
        ///
        /// The body's linear velocity magnitude must be less than this threshold for
        /// it to be considered idle.
        ///
        /// Set the threshold to dInfinity to prevent the linear velocity from being considered.
        /// </summary>
        public float AutoDisableLinearThreshold
        {
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetAutoDisableLinearThreshold(this, value);
            }
        }

        /// <summary>
        /// Set a body's angular velocity threshold for automatic disabling.
        ///
        /// The body's linear angular magnitude must be less than this threshold for
        /// it to be considered idle.
        ///
        /// Set the threshold to dInfinity to prevent the angular velocity from being considered.
        /// </summary>
        public float AutoDisableAngularThreshold
        {
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetAutoDisableAngularThreshold(this, value);
            }
        }

        /// <summary>
        /// Set the number of simulation steps that a body must be idle before
        /// it is automatically disabled.
        ///
        /// Set this to zero to disable consideration of the number of steps.
        /// </summary>
        public int AutoDisableSteps
        {
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetAutoDisableSteps(this, value);
            }
        }

        /// <summary>
        /// Set the amount of simulation time that a body must be idle before
        /// it is automatically disabled.
        ///
        /// Set this to zero to disable consideration of the amount of simulation time.
        /// </summary>
        public float AutoDisableTime
        {
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetAutoDisableTime(this, value);
            }
        }

		/// <summary>
		/// Add torque to a body using relative coordinates.
		///
		/// This function takes a force vector that is relative to the body's own frame of reference.
		/// </summary>
		/// <remarks>
		/// Forces are accumulated on to each body, and the accumulators are zeroed after each time step.
		///
		/// Force is applied at body's center of mass
		/// </remarks>
        public void AddRelativeTorque(Vector3 torque)
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dBodyAddRelTorque(this, torque.X, torque.Y, torque.Z);

        }

        /// <summary>
        /// Add force to a body using absolute coordinates.
        /// </summary>
        /// <remarks>
        /// Forces are accumulated on to each body, and the accumulators are zeroed after each time step.
        ///
        /// Force is applied at body's center of mass
        /// </remarks>
        public void AddForce(Vector3 force)
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dBodyAddForce(this, force.X, force.Y, force.Z);

        }

        /// <summary>
		/// Returns given vector as expressed in the world coordinate system (x,y,z), 
        /// rotate it to the body coordinate system (result).
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector3 BodyVectorFromWorld(Vector3 vec)
        {
            Tao.Ode.Ode.dVector3 temp = new Tao.Ode.Ode.dVector3();

            Tao.Ode.Ode.dBodyVectorFromWorld(this, vec.X, vec.Y, vec.Z, ref temp);

            return temp;

        }

        /// <summary>
        /// Given a vector expressed in the body coordinate system (x,y,z), rotate it to the world coordinate system (result).
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vector3 BodyVectorToWorld(Vector3 vec)
        {
            Tao.Ode.Ode.dVector3 temp = new Tao.Ode.Ode.dVector3();

            Tao.Ode.Ode.dBodyVectorToWorld(this, vec.X, vec.Y, vec.Z, ref temp);

            return temp;
        }

        public bool InfluencedByGravity
        {
            get
            {
                return Tao.Ode.Ode.dBodyGetGravityMode(this) != 0;
            }
            set
            {
                Tao.Ode.Ode.dBodySetGravityMode(this, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets/Sets the angular velocity of a body
        ///
        /// The vector is valid until any changes are made to the rigid body system structure.
        /// </summary>
        public Vector3 AngularVel
        {
            get
            {
                if (IsNull())
                    return Vector3.Zero;

                return Tao.Ode.Ode.dBodyGetAngularVel(this);
            }
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetAngularVel(this, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Gets/Sets the linear velocity of a body
        ///
        /// The vector is valid until any changes are made to the rigid body system structure.
        /// </summary>
        public Vector3 LinearVel
        {
            get
            {
                if (IsNull())
                    return Vector3.Zero;

                return Tao.Ode.Ode.dBodyGetLinearVel(this);
            }
            set
            {
                if (IsNull())
                    return;

                Tao.Ode.Ode.dBodySetLinearVel(this, value.X, value.Y, value.Z);
            }

        }

        /// <summary>
        /// Set the body force accumulation vector.
        /// This is mostly useful to zero the force and torque for deactivated bodies before they are reactivated,
        /// in the case where the force-adding functions were called on them while they were deactivated.
        /// </summary>
        public Vector3 Force
        {
            get
            {
                return Tao.Ode.Ode.dBodyGetForce(this);
            }
            set
            {
                Tao.Ode.Ode.dBodySetForce(this, value.X, value.Y, value.Z);

            }
        }

        /// <summary>
        /// Gets/Set the body torque accumulation vector.
        /// This is mostly useful to zero the force and torque for deactivated bodies before they are reactivated,
        /// in the case where the force-adding functions were called on them while they were deactivated.
        /// </summary>
        public Vector3 Torque
        {
            get
            {
                return Tao.Ode.Ode.dBodyGetTorque(this);
            }
            set
            {
                Tao.Ode.Ode.dBodySetTorque(this, value.X, value.Y, value.Z);

            }
        }

        public Matrix Rotation
        {
            get
            {
                return Tao.Ode.Ode.dBodyGetRotation(this);
            }
            set
            {
                Tao.Ode.Ode.dMatrix3 val = value;
                Tao.Ode.Ode.dBodySetRotation(this, val);
            }
        }

        public Vector3 Position
        {
            get
            {
                return Tao.Ode.Ode.dBodyGetPosition(this);
            }
            set
            {
                Tao.Ode.Ode.dBodySetPosition(this, value);
            }
        }

        /// <summary>
        /// Gets/Sets the mass of the body (see the mass functions)
        /// </summary>
        public Tao.Ode.Ode.dMass Mass
        {
            get
            {
                Tao.Ode.Ode.dMass m = new Tao.Ode.Ode.dMass();

                Tao.Ode.Ode.dBodyGetMass(this, ref m);

                return m;
            }
            set
            {
                Tao.Ode.Ode.dBodySetMass(this, ref value);
            }
        }

        /// <summary>
        /// Take a point on a body (px,py,pz) and return that point's velocity in body-relative coordinates.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 GetRelPointVel(Vector3 p)
        {
            Tao.Ode.Ode.dVector3 result = new Tao.Ode.Ode.dVector3();
            Tao.Ode.Ode.dBodyGetRelPointVel(this, p.X, p.Y, p.Z, ref result);
            return result;
        }


        public static implicit operator IntPtr(dBodyID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dBodyID(IntPtr ptr)
        {
            return new dBodyID(ptr);
        }

        /// <summary>
        /// Destroy this body.
        ///
        /// All joints that are attached to this body will be put into limbo (i.e. unattached and
        /// not affecting the simulation, but they will NOT be deleted)
        /// </summary>
        public void Destroy()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dBodyDestroy(this);

            Dispose();
        }

        public bool Enabled
        {
            get { return _enabled; }
            set 
            {
                if (IsNull())
                    return;

                _enabled = value;

                if (value)
                    Tao.Ode.Ode.dBodyEnable(this);
                else
                    Tao.Ode.Ode.dBodyDisable(this);
            }
        }

    }

    public class dJointID : PtrWrapper
    {

        public dJointID(IntPtr ptr)
            : base(ptr)
        {
        }

        /// <summary>
        /// Destroy a joint, disconnecting it from its attached bodies and removing it from the world.
        /// However, if the joint is a member of a group then this function has no effect - to destroy
        /// that joint the group must be emptied or destroyed.
        /// </summary>
        public void Destroy()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dJointDestroy(this);

            Dispose();
        }

        /// <summary>
        /// Attach this joint to some new bodies.
        ///
        /// If the joint is already attached, it will be detached from the old bodies first.
        /// To attach this joint to only one body, set body1 or body2 to zero - a zero body
        /// refers to the static environment.
        /// Setting both bodies to zero puts the joint into "limbo", i.e. it will have no
        /// effect on the simulation.
        /// Some joints, like hinge-2 need to be attached to two bodies to work.
        /// </summary>
        public void Attach(dBodyID b1Id, dBodyID b2Id)
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dJointAttach(this, b1Id, b2Id);

        }

        public static implicit operator IntPtr(dJointID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dJointID(IntPtr ptr)
        {
            return new dJointID(ptr);
        }

    }


    public class dJointGroupID : PtrWrapper
    {

        public dJointGroupID(IntPtr ptr)
            : base(ptr)
        {
        }

        /// <summary>
        /// Create a joint group.
        /// </summary>
        /// <returns></returns>
        public static dJointGroupID Create()
        {
            return Tao.Ode.Ode.dJointGroupCreate(0);
        }

        /// <summary>
        /// Destroy a joint group. All joints in the joint group will be destroyed.
        /// </summary>
        public void Destroy()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dJointGroupDestroy(this);

            Dispose();
        }

        /// <summary>
        /// Empty a joint group.
        /// All joints in the joint group will be destroyed, but the joint group itself will not be destroyed.
        /// </summary>
        public void Empty()
        {
            if (IsNull())
                return;

            Tao.Ode.Ode.dJointGroupEmpty(this);

            Dispose();
        }

        public static implicit operator IntPtr(dJointGroupID obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return obj._ptr;
        }

        public static implicit operator dJointGroupID(IntPtr ptr)
        {
            return new dJointGroupID(ptr);
        }

    }

}
