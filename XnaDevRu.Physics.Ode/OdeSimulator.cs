using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics.Ode
{
	public class OdeSimulator : Simulator
	{
		public OdeSimulator()
			: base()
		{
		}

        public static Simulator CreateOdeSimulator()
        {
            return new OdeSimulator();
        }

		public override void InitData(SimulatorData data)
		{
			base.InitData(data);

			// Create ODE world.
			//world = new World();
			world = Tao.Ode.Ode.dWorldCreate();

			// Set default gravity.
			Gravity = Defaults.Gravity;

			// Create the root ODE space.
			if (data.UseOctreeInsteadHash)
			{
				//space = new QuadTreeSpace(data.worldCenter, data.worldSize, data.octreeDepth);
				space = Tao.Ode.Ode.dQuadTreeSpaceCreate(IntPtr.Zero, OdeHelper.ToOdeVector3(data.WorldCenter), OdeHelper.ToOdeVector3(data.WorldSize), data.OctreeDepth); 
			}
			else
			{
				space = Tao.Ode.Ode.dHashSpaceCreate(IntPtr.Zero);
				Tao.Ode.Ode.dHashSpaceSetLevels(space, data.HashMinLevel, data.HashMaxLevel); // (KleMiX) ...
			}

			//space.Collider = OdeHelper.CollisionCallback;

			rootSpace = new OdeSpace(space);

			// Create the ODE contact joint group.
			//contactJointGroup = new ODE.Joints.JointGroup();
			contactJointGroup = Tao.Ode.Ode.dJointGroupCreate(0);

			// Set the ODE global CFM value that will be used by all Joints
			// (including contacts).  This affects normal Joint constraint
			// operation and Joint limits.  The user cannot adjust CFM, but
			// they can adjust ERP (a.k.a. bounciness/restitution) for materials
			// (i.e. contact settings) and Joint limits.
			//dWorldSetCFM( mWorldID, Defaults.Ode.globalCFM );
			//world.CFM = Defaults.Ode.globalCFM;
			Tao.Ode.Ode.dWorldSetCFM(world, Defaults.Ode.GlobalCFM);

			// Set the ODE global ERP value.  This will only be used for Joints
			// under normal conditions, not at their limits.  Also, it will not
			// affect contacts at all since they use material properties to
			// calculate ERP.
			//dWorldSetERP( mWorldID, ( real ) 0.5 * ( defaults::ode::maxERP +defaults::ode::minERP ) );
			//world.ERP = 0.5f * (Defaults.Ode.maxERP + Defaults.Ode.minERP);
			Tao.Ode.Ode.dWorldSetERP(world, 0.5f * (Defaults.Ode.MaxERP + Defaults.Ode.MinERP));
			Tao.Ode.Ode.dWorldSetContactSurfaceLayer(world, Defaults.Ode.SurfaceLayer);

			SolverAccuracy = Defaults.SolverAccuracy;
			collisionCount = 0;
			// "mRaycastResult" is initialized in its own constructor.
			sensorSolid = null;
			rayContactGroup = Defaults.Shape.ContactGroup;
		}

		public void InternalAddCollidedSolid(Solid solid)
		{
			// If the collided Solid is attached to the Sensor performing the
			// volume query, ignore this intersection.
			if (sensorSolid == solid)
			{
				return;
			}

			OdeSolid osolid = solid as OdeSolid;

			if (osolid.CollisionCount != collisionCount)
			{
				volumeQueryResult.InternalAddSolid(solid);
				osolid.CollisionCount = collisionCount;
			}
		}

		public void InternalAddRaycastResult(Solid solid, Vector3 intersection, Vector3 normal, float depth)
		{
			// If the collided Solid is attached to the Sensor performing the
			// raycast, ignore this intersection.
			if (sensorSolid == solid)
			{
				return;
			}

			// Add this RaycastResult to the vector of results.
			RaycastResult result;
			result.Solid = solid;
			result.Intersection = intersection;
			result.Normal = normal;
			result.Distance = depth;
			raycastResults.Add(result);
		}

		public int InternalGetRayContactGroup()
		{
			return rayContactGroup;
		}

		public dWorldID World { get { return world; } }
		public dSpaceID Space { get { return space; } }
		public IntPtr JointGroup { get { return contactJointGroup; } }

		public override SolverAccuracyLevel SolverAccuracy
		{
			get
			{
				return base.SolverAccuracy;
			}
			set
			{
				base.SolverAccuracy = value;

				switch (value)
				{
					case SolverAccuracyLevel.VeryLow:
						solverType = SolverType.QuickStep;
						//quickWorldIterations = 5;
						Tao.Ode.Ode.dWorldSetQuickStepNumIterations(world, 5);
						break;
					case SolverAccuracyLevel.Low:
						solverType = SolverType.QuickStep;
						//quickWorldIterations = 10;
						Tao.Ode.Ode.dWorldSetQuickStepNumIterations(world, 10);
						break;
					case SolverAccuracyLevel.Medium:
						solverType = SolverType.QuickStep;
						// 20 is the default in ODE
						//quickWorldIterations = 20;
						Tao.Ode.Ode.dWorldSetQuickStepNumIterations(world, 20);
						break;
					case SolverAccuracyLevel.High:
						solverType = SolverType.QuickStep;
						//quickWorldIterations = 40;
						Tao.Ode.Ode.dWorldSetQuickStepNumIterations(world, 40);
						break;
					case SolverAccuracyLevel.VeryHigh:
						solverType = SolverType.WorldStep;
						break;
					default:
						break;
				}

			}
		}

		#region Simulator Implementation
		public override Joint CreateJoint()
		{
			Joint newJoint = new OdeJoint(world);
			AddJoint(newJoint);
			return newJoint;
		}

		public override Solid CreateSolid()
		{
			Solid newSolid = new OdeSolid(world, space);
			AddSolid(newSolid);
			return newSolid;
		}

		public override SpaceBase CreateSpace()
		{
			OdeSpace newSpace = new OdeSpace();

			// Add this new Space as a child of the Simulator's root Space.
			//space.Add((ODE.Geoms.Geom)newSpace.InternalGetSpaceID());
			Tao.Ode.Ode.dSpaceAdd(space, newSpace.InternalGetSpaceID());

			AddSpace(newSpace);
			return newSpace;
		}

		public override void Destroy()
		{
			// The following must occur after Simulator::~Simulator() is called;
			// otherwise, Simulator::~Simulator() will try to destroy Solids after
			// ODE has closed.
			/*space.Destroy();
			world.Destroy();
			contactJointGroup.Destroy();*/
			Tao.Ode.Ode.dSpaceDestroy(space);
			Tao.Ode.Ode.dWorldDestroy(world);
			Tao.Ode.Ode.dJointGroupDestroy(contactJointGroup);
			Tao.Ode.Ode.dCloseODE();
		}

		public override Vector3 Gravity
		{
			get
			{
				return world.Gravity;
			}
			set
			{
				world.Gravity = value;
			}
		}

		public override List<RaycastResult> InternalFireRay(Ray r, float length, Solid attachedSolid, int rayContactGroup)
		{
			if (raycastResults != null)
				raycastResults.Clear();
			sensorSolid = attachedSolid;
			this.rayContactGroup = rayContactGroup;

			// Create an ODE ray geom.  Make sure its user data pointer is
			// NULL because this is used in the collision callback to
			// distinguish the ray from other geoms.
			//ODE.Geoms.Ray ray = new ODE.Geoms.Ray(r.Position, r.Direction, length);
			//ray.UserData = null;
			IntPtr ray = Tao.Ode.Ode.dCreateRay(space, length);
			Tao.Ode.Ode.dGeomRaySet(ray, r.Position.X, r.Position.Y, r.Position.Z, r.Direction.X, r.Direction.Y, r.Direction.Z);
			Tao.Ode.Ode.dGeomSetData(ray, IntPtr.Zero);

			// Check for collisions.  This will fill mRaycastResult with valid
			// data.  Its Solid pointer will remain NULL if nothing was hit.
			//ODE.Space.Collide2(ray, (ODE.Geoms.Geom)space, OdeHelper.RaycastCollisionCallback);
			Tao.Ode.Ode.dSpaceCollide2( ray, space, GCHandle.ToIntPtr(GCHandle.Alloc(this)), OdeHelper.RaycastCollisionCallback);

			// Finished with ODE ray, so destroy it.
			Tao.Ode.Ode.dGeomDestroy(ray);

			return raycastResults;
		}

		public override VolumeQueryResult InternalQueryVolume(Solid volume, Solid attachedSolid)
		{
			sensorSolid = attachedSolid;
			collisionCount++;
			volumeQueryResult.InternalClearSolids();
			List<GeomData> geomList = ((OdeSolid)volume).InternalGetGeomDataList();

			// Check for collisions with each of the volume Solid's geoms.
			// This will fill up mVolumeQueryResult with those Solids that
			// collide with these geoms.
            foreach (GeomData g in geomList)
			{
				//ODE.Space.Collide2(g, (ODE.Geoms.Geom)space, OdeHelper.VolumeCollisionCallback);
				Tao.Ode.Ode.dSpaceCollide2(g.SpaceID, space, GCHandle.ToIntPtr(GCHandle.Alloc(this)), OdeHelper.VolumeCollisionCallback);
			}

			// We don't want the volume Solid to be listed in the results.
			volumeQueryResult.InternalRemoveSolid(volume);

			return volumeQueryResult;
		}

		protected override void StepPhysics()
		{
			// Apply linear and angular damping; if using the "add opposing
			// forces" method, be sure to do this before calling ODE step
			// function.
            if(solidList != null)
                foreach (OdeSolid solid in solidList)
                {
                    if (!solid.Static)
                    {
                        if (solid.Sleeping)
                        {
                            // Reset velocities, force, & torques of objects that go
                            // to sleep; ideally, this should happen in the ODE
                            // source only once - when the object initially goes to
                            // sleep.
                            /*ODE.Body body = solid.InternalGetBodyID();
                            body.ApplyLinearVelocityDrag(0, 0, 0);
                            body.ApplyAngularVelocityDrag(0, 0, 0);
                            body.AddForce(0, 0, 0);
                            body.AddTorque(0, 0, 0);*/
                            IntPtr body = solid.InternalGetBodyID();
                            Tao.Ode.Ode.dBodySetLinearVel(body, 0, 0, 0);
                            Tao.Ode.Ode.dBodySetAngularVel(body, 0, 0, 0);
                            Tao.Ode.Ode.dBodySetForce(body, 0, 0, 0);
                            Tao.Ode.Ode.dBodySetTorque(body, 0, 0, 0);
                        }
                        else
                        {
                            // Dampen Solid motion.  3 possible methods:
                            // 1) apply a force/torque in the opposite direction of
                            // motion scaled by the body's velocity
                            // 2) same as 1, but scale the opposing force by
                            // the body's momentum (this automatically handles
                            // different mass values)
                            // 3) reduce the body's linear and angular velocity by
                            // scaling them by 1 - damping * stepsize

                            /*ODE.Body body = solid.InternalGetBody();
                            ODE.Mass mass = body.Mass;*/

                            dBodyID body = solid.InternalGetBodyID();
                            Tao.Ode.Ode.dMass mass = body.Mass;
                            //Tao.Ode.Ode.dBodyGetMass(body, ref mass); // (mike)

                            // Method 2
                            // Since the ODE mass.I inertia matrix is local, angular
                            // velocity and torque also need to be local.

                            // Linear damping
                            float linearDamping = solid.LinearDamping;
                            if (0 != linearDamping)
                            {
                                Vector3 lVelGlobal = Tao.Ode.Ode.dBodyGetLinearVel(body); // C# compiler automatically calls an implicit convertion here

                                // The damping force depends on the damping amount,
                                // mass, and velocity (i.e. damping amount and
                                // momentum).
                                float dampingFactor = -linearDamping * mass.mass;
                                Vector3 dampingForce = new Vector3(dampingFactor * lVelGlobal.X, dampingFactor * lVelGlobal.Y, dampingFactor * lVelGlobal.Z);

                                // Add a global force opposite to the global linear
                                // velocity.
                                body.AddForce(dampingForce);
                                //(mike)//Tao.Ode.Ode.dBodyAddForce(body, dampingForce.X, dampingForce.Y, dampingForce.Z);
                            }

                            // Angular damping
                            float angularDamping = solid.AngularDamping;
                            if (0 != angularDamping)
                            {
                                Vector3 aVelGlobal = Tao.Ode.Ode.dBodyGetAngularVel(body); // C# compiler automatically calls an implicit convertion here

                                //(mike) //Tao.Ode.Ode.dBodyVectorFromWorld(body, aVelGlobal.X, aVelGlobal.Y, aVelGlobal.Z, ref temp);

                                Vector3 aVelLocal = body.BodyVectorFromWorld(aVelGlobal);

                                // The damping force depends on the damping amount,
                                // mass, and velocity (i.e. damping amount and
                                // momentum).
                                float dampingFactor = -angularDamping;
                                Vector3 aMomentum;

                                // Make adjustments for inertia tensor.
                                //dMULTIPLYOP0_331( aMomentum, = , mass.I, aVelLocal ); (KleMiX) ???
                                aMomentum.X = mass.I.M00 * aVelLocal.X + mass.I.M01 * aVelLocal.Y + aVelLocal.X + mass.I.M02 * aVelLocal.Z;
                                aMomentum.Y = mass.I.M10 * aVelLocal.X + mass.I.M11 * aVelLocal.Y + aVelLocal.X + mass.I.M12 * aVelLocal.Z;
                                aMomentum.Z = mass.I.M20 * aVelLocal.X + mass.I.M21 * aVelLocal.Y + aVelLocal.X + mass.I.M22 * aVelLocal.Z;

                                Vector3 dampingTorque = new Vector3(dampingFactor * aMomentum.X, dampingFactor * aMomentum.Y, dampingFactor * aMomentum.Z);

                                // Add a local torque opposite to the local angular
                                // velocity.
                                //body.AddRelativeTorque(dampingTorque);
                                Tao.Ode.Ode.dBodyAddRelTorque(body, dampingTorque.X, dampingTorque.Y, dampingTorque.Z);
                            }
                        }
                    }
                }

			// Do collision detection; add contacts to the contact joint group.
			//space.Collide();
			Tao.Ode.Ode.dSpaceCollide(space, GCHandle.ToIntPtr(GCHandle.Alloc(this)),
				OdeHelper.CollisionCallbackRunner);

			// Take a simulation step.
			if (SolverType.QuickStep == solverType)
			{
				//world.QuickStep(stepSize, quickWorldIterations);
				Tao.Ode.Ode.dWorldQuickStep(world, stepSize);
			}
			else
			{
				//world.TimeStep(stepSize);
				Tao.Ode.Ode.dWorldStep(world, stepSize);
			}

			// Remove all joints from the contact group.
			//contactJointGroup.Clear();
			Tao.Ode.Ode.dJointGroupEmpty(contactJointGroup);

			// Fix angular velocities for freely-spinning bodies that have
			// gained angular velocity through explicit integrator inaccuracy.

            if(solidList != null)
                foreach (OdeSolid s in solidList)
                {
                    if (!s.Sleeping && !s.Static)
                    {
                        s.InternalDoAngularVelFix();
                    }
                }
		}
		#endregion

		public enum SolverType
		{
			/// Time complexity: O(m^3), space complexity O(m^2), where
			/// m = # of constraints.
			WorldStep,

			/// Time complexity: O(m*N), space complexity O(m), where
			/// m = # of constraints and N = # of iterations.
			QuickStep
		}

		//protected int quickWorldIterations;
		/// The ODE joint constraint group.
		protected IntPtr contactJointGroup;
		/// The ODE world ID used by this Simulator.
		protected dWorldID world;
		/// The root of the ODE collision detection hierarchy.
		protected dSpaceID space;

		/// The type of constraint solver to use.
		protected SolverType solverType;

		/// Used for volume collision checks.
		protected int collisionCount;

		/// Temporary list of Solids that collided in a volume collision
		/// check.
		protected VolumeQueryResult volumeQueryResult;

		/// Used for ray casting and volume queries.  If a RaycastSensor or
		/// VolumeSensor is attached to a Solid, this pointer will point to
		/// that Solid.  It is used to make sure the raycasts and collision
		/// query doesn't collide with the attached Solid.
		protected Solid sensorSolid;

		/// A temporary variable that lets rays to use contact groups.  This
		/// allows them to limit which Shapes they collide with.
		protected int rayContactGroup;

		/// A vector of RaycastResults returned from a ray cast.
		protected List<RaycastResult> raycastResults;
	}
}
