using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
	/// <summary>
	/// A Simulator is an environment that contains simulated objects.
	/// It performs collision detection and physical simulation.  It is
	/// a factory that creates, maintains, and destroys Solids, Joints,
	/// Motors, and Sensors.
	/// </summary>
	public abstract class Simulator
	{
		//public event MovementEventHandler OnMove;
		//public event PostStepEventProcessor OnPostStep;

		public delegate Simulator ConstructorDelegate();

		public static ConstructorDelegate CreateMethod;

		/// <summary>
		/// Helper function used for ray casting.  Immediately fires a ray
		/// into the scene and returns intersections results.  Uses the
		/// ray's contact group parameter to limit collision checks.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="length"></param>
		/// <param name="attachedSolid"></param>
		/// <param name="rayContactGroup"></param>
		/// <returns></returns>
		public abstract List<RaycastResult> InternalFireRay(Ray r, float length, Solid attachedSolid, int rayContactGroup);

		/// <summary>
		/// Helper function used for volume queries.
		/// </summary>
		/// <param name="volume"></param>
		/// <param name="attachedSolid"></param>
		/// <returns></returns>
		public abstract VolumeQueryResult InternalQueryVolume(Solid volume, Solid attachedSolid);

		public Simulator()
		{
			timeBuffer = 0;
			StepSize = Defaults.StepSize;
			MaxCorrectingVel = Defaults.MaxCorrectingVel;
			MaxContacts = Defaults.MaxContacts / 2;
			UserData = null;
			isSolidDestructionSafe = true;
			isJointDestructionSafe = true;
			staticSleepingContactsEnabled = Defaults.StaticSleepingContactsEnabled;

			for (int i = 0; i < 32; ++i)
			{
				contactGroupFlags[i] = Defaults.ContactGroupFlags;
			}

			rootSpace = null;
		}

		/// <summary>
		/// Creates selected Simulator with some default values
		/// </summary>
		/// <returns></returns>
		public static Simulator Create()
		{
			Simulator sim = CreateMethod();

			sim.InitData(new SimulatorData());

			sim.Gravity = new Vector3(0, -9.81f, 0);

			return sim;
		}

		public virtual void InitData(SimulatorData data)
		{
			this.data = data;
		}

		/// <summary>
		/// Deallocates everything within the Simulator.  This should be
		/// called when finished with everything.  Simulator implementations
		/// should call "delete this" within this function.
		/// </summary>
		public abstract void Destroy();

		/// <summary>
		/// This function performs collision detection and simulates
		/// everything ahead by the given dt.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		/// <remarks>
		/// Internally, it steps through
		/// the simulation iteratively using a fixed step size (so you can
		/// pass in whatever dt you want; it will be broken up into fixed
		/// increments automatically).  Any "leftover" time will be saved
		/// until the next time this is called.  The function returns true
		/// if at least one time step has been taken.  During collision
		/// detection, the following cases are ignored when deciding whether
		/// to collide two Solids (ignored both for physical contact
		/// generation and collision event handling):
		/// 1. Two static Solids, each without a CollisionEventHandler.
		/// 2. Two Shapes that are part of the same Solid.
		/// 3. Two sleeping Solids.
		/// 4. Two Solids connected by a fixed Joint.
		/// 5. Two Solids connected by a Joint with contacts disabled.
		/// 6. Solid0 is static, Solid1 dynamic and is sleeping,
		///    static-to-sleeping contacts are ignored by the
		///    Simulator, and neither Solid has a
		///    CollisionEventHandler.
		/// 7. Solid1 is static, Solid0 dynamic and is sleeping,
		///    static-to-sleeping contacts are ignored by the
		///    Simulator, and neither Solid has a
		///    CollisionEventHandler.
		/// 8. The two Solids' contact groups do not generate
		///    contacts when they collide, and neither Solid has a
		///    CollisionEventHandler.
		/// </remarks>
		public virtual bool Simulate(float dt)
		{
			timeBuffer += dt;

			// Use this to return true if at least one step occurred.
			bool stepOccurred = false;
			if (timeBuffer >= stepSize)
			{
				stepOccurred = true;
			}

#if PHYSICS_VARIABLE_TIMESTEP
		    float stepSizeBackup = stepSize;
		    int currentStep = 0;
		    int maxStepCount = 1;
#endif
			while (timeBuffer >= stepSize)
			{
#if PHYSICS_VARIABLE_TIMESTEP
                if(currentStep == maxStepCount-1)
                {
                    stepSize = timeBuffer;
                }
#endif

				// Update Sensors.
				foreach (Sensor s in sensorList)
					s.InternalUpdate();

				// Update Motors.
				foreach (Motor m in motorList)
					m.InternalUpdate();

				// Apply forces/torques to Solids.
				foreach (Solid s in solidList)
					s.InternalApplyForces(stepSize);

				// Update Joints.
				isJointDestructionSafe = false;
				// If a Joint gets broken here, it will automatically
				// send a Joint break event to the Joint's
				// JointBreakEventHandler (assuming one exists).

				foreach (Joint j in jointList)
					j.InternalUpdate();

				isJointDestructionSafe = true;

				// Now do physics engine-specific stuff; collision events will
				// be sent from here.
				StepPhysics();

				// Loop over Solids again to handle a few more things...

				isSolidDestructionSafe = false;

				foreach (Solid s in solidList)
				{
					// Get each dynamic, awake Solid's new transform from the
					// physics engine.
					if (!s.Static && !s.Sleeping)
					{
						s.InternalUpdateOPALTransform();
						s.Moving = true;
					}

					// Update the Solid's CollisionEventHandler if applicable.
					if (s.CollisionEventHandler != null)
					{
						s.CollisionEventHandler.InternalHandlePendingCollisionEvents();
					}

					if (s.MovementEventHandler != null)
					{
						if (s.Moving)
						{
							MovementEvent move = new MovementEvent(s);
							s.MovementEventHandler.HandleMovementEvent(move);
							//if (OnMove != null)
							//    OnMove(move);
						}
					}
				}
				isSolidDestructionSafe = true;

				// Fire an event to the PostStepEventHandler, if one exists.
				/*if (NumPostStepEventHandlers > 0)
				{
					for (int i = 0; i < postStepEventHandlers.Count; i++)
					{
						postStepEventHandlers[i].HandlePostStepEvent();
					}
				}
				if (OnPostStep != null)
					OnPostStep();*/

				if (NumGlobalCollisionEventHandlers > 0)
				{
					for (int i = 0; i < collisionEventHandlers.Count; i++)
						collisionEventHandlers[i].InternalHandlePendingCollisionEvents();
				}

				// Destroy garbage now that it's safe.
				DestroyGarbage();

				//Decrement the time buffer
				timeBuffer -= stepSize;

#if PHYSICS_VARIABLE_TIMESTEP
			    currentStep++;
#endif
			}

#if PHYSICS_VARIABLE_TIMESTEP
		    stepSize = stepSizeBackup;
#endif

			return stepOccurred;
		}

		/// <summary>
		/// Returns the constant step size used in the simulation.
		/// Sets the constant step size used in the simulation.  The step
		/// size must be greater than zero.
		/// </summary>
		public virtual float StepSize
		{
			get { return stepSize; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value", "StepSize must be positive");

				stepSize = value;
			}
		}

		/// <summary>
		/// Sets the Simulator's post-step event handler.
		/// </summary>
		/// <param name="eventHandler">you need to clean it up yourself</param>
		public virtual void AddPostStepEventHandler(PostStepEventProcessor eventHandler)
		{
			postStepEventHandlers.Add(eventHandler);
		}

		/// <summary>
		/// If such handler is stored it will be removed from the list.
		/// </summary>
		/// <param name="eventHandler"></param>
		public virtual void RemovePostStepEventHandler(PostStepEventProcessor eventHandler)
		{
			postStepEventHandlers.Remove(eventHandler);
		}

		/// <summary>
		/// Returns number of stored step handlers
		/// </summary>
		public virtual int NumPostStepEventHandlers
		{
			get
			{
				if (postStepEventHandlers == null)
					return 0;

				return postStepEventHandlers.Count;
			}
		}

		/// <summary>
		/// Returns the Simulator's post-step event handler.  If this
		/// returns NULL, the Simulator is not using one.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual PostStepEventProcessor GetPostStepEventHandler(int id)
		{
			return postStepEventHandlers[id];
		}

		/// <summary>
		/// Sets the Simulator's collision event handler.
		/// </summary>
		/// <param name="eventHandler">you need to clean it up yourself</param>
		public virtual void AddGlobalCollisionEventHandler(CollisionEventProcessor eventHandler)
		{
			collisionEventHandlers.Add(eventHandler);
		}

		/// <summary>
		/// If such handler is stored it will be removed from the list.
		/// </summary>
		/// <param name="eventHandler"></param>
		public virtual void RemoveGlobalCollisionEventHandler(CollisionEventProcessor eventHandler)
		{
			collisionEventHandlers.Remove(eventHandler);
		}

		/// <summary>
		/// Returns number of stored collision handlers
		/// </summary>
		public virtual int NumGlobalCollisionEventHandlers
		{
			get
			{
				if (collisionEventHandlers == null)
					return 0;

				return collisionEventHandlers.Count;
			}
		}

		/// <summary>
		/// Returns the Simulator's post-step event handler.  If this
		/// returns NULL, the Simulator is not using one.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual CollisionEventProcessor GetGlobalCollisionEventHandler(int id)
		{
			return collisionEventHandlers[id];
		}

		/// <summary>
		/// Returns or sets the gravity used in the simulation.
		/// </summary>
		public abstract Vector3 Gravity { get; set; }

		/// <summary>
		/// Returns or sets the accuracy level used by the physics engine's constraint
		/// solver.
		/// </summary>
		public virtual SolverAccuracyLevel SolverAccuracy { get { return solverAccuracyLevel; } set { solverAccuracyLevel = value; } }

		/// <summary>
		/// Returns or sets the maximum correcting velocity for interpenetrating
		/// objects.  The given velocity must be positive.
		/// </summary>
		public virtual float MaxCorrectingVel
		{
			get { return maxCorrectingVel; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value", "MaxCorrectingVel must be positive");

				maxCorrectingVel = value;
			}
		}

		/// <summary>
		/// Returns or sets the maximum number of physical contacts generated
		/// when two Solids collide. This number cannot be larger than the
		/// global "max max contacts" parameter.
		/// </summary>
		public virtual int MaxContacts
		{
			get { return maxContacts; }
			set
			{
				if (value > Globals.MaxMaxContacts)
					throw new ArgumentOutOfRangeException("value", "MaxContacts must be less or equal than Globals.MaxMaxContacts");

				maxContacts = value;
			}
		}

		/// <summary>
		/// Returns or sets the user data pointer to some external data. The user data
		/// is totally user-managed
		/// (i.e. it is not destroyed when the Simulator is destroyed).
		/// </summary>
		public virtual object UserData { get { return userData; } set { userData = value; } }

		/// <summary>
		/// Defines the interaction between two contact groups.  If the
		/// last argument is true, the two groups will generate physical
		/// points when they collide.  Otherwise, they will pass through
		/// each other.  Keep in mind that certain cases are already
		/// ignored when performing collision detection; see comments
		/// on Simulator::simulate for more details.
		/// Note that contact groups do not affect collision events; two
		/// colliding objects might not generate contacts and still
		/// generate collision events sent to their CollisionEventHandlers.
		/// </summary>
		/// <param name="group0"></param>
		/// <param name="group1"></param>
		/// <param name="makeContacts"></param>
		public virtual void SetupContactGroups(int group0, int group1, bool makeContacts)
		{
			if (group0 > 31)
			{
				/*OPAL_LOGGER( "warning" ) << "opal::Simulator::setupContactGroups: "
				<< "Invalid contact group " << group0
				<< ". Request will be ignored." << std::endl;*/
				// (KleMiX)
				return;
			}

			if (group1 > 31)
			{
				/*OPAL_LOGGER( "warning" ) << "opal::Simulator::setupContactGroups: "
				<< "Invalid contact group " << group1
				<< ". Request will be ignored." << std::endl;*/
				// (KleMiX)
				return;
			}

			// The interaction always goes both ways, so we need to set the bit
			// flags both ways.

			long group0Bit = (long)(1 << group0);
			long group1Bit = (long)(1 << group1);

			if (makeContacts)
			{
				// Raise the group1 bit in group0's array.
				contactGroupFlags[group0] |= group1Bit;

				// Raise the group0 bit in group1's array.
				contactGroupFlags[group1] |= group0Bit;
			}
			else
			{
				long tempMask = 0xFFFFFFFF;
				long notGroup0Bit = group0Bit ^ tempMask;
				long notGroup1Bit = group1Bit ^ tempMask;

				// Lower the group1 bit in group0's array.
				contactGroupFlags[group0] &= notGroup1Bit;

				// Lower the group0 bit in group1's array.
				contactGroupFlags[group1] &= notGroup0Bit;
			}
		}

		/// <summary>
		/// Similar to setupContactGroups.  Determines how a single contact
		/// group interacts with all other groups.  This is useful when
		/// you want to e.g. disable collisions between a group and
		/// everything else for a "trigger volume."  (See setupContactGroups
		/// for more details).
		/// </summary>
		/// <param name="group"></param>
		/// <param name="makeContacts"></param>
		public virtual void SetupContactGroup(int group, bool makeContacts)
		{
			int i = 0;
			for (i = 0; i < 32; ++i)
			{
				SetupContactGroups(group, i, makeContacts);
			}
		}

		/// <summary>
		/// Returns true if the two contact groups are setup to generate
		/// contacts when they collide.
		/// </summary>
		/// <param name="group0"></param>
		/// <param name="group1"></param>
		/// <returns></returns>
		public virtual bool GroupsMakeContacts(int group0, int group1)
		{
			// We only need to check for "one side" of the contact groups
			// here because the groups are always setup both ways (i.e.
			// the interaction between object 0's contact group and
            // object 1's contact group is always symmetric).

            long group1Bit = (long)(1 << group1);
            if ((InternalGetContactGroupFlags(group0) & group1Bit) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
		}

		/// <summary>
		/// Returns or sets whether contacts should be generated between static Solids
		/// and sleeping Solids.  Usually this isn't necessary, but
		/// sometimes you might want a static Solid to wake up a sleeping
		/// dynamic Solid by touching it.
		/// </summary>
		public virtual bool IsStaticSleepingContactsEnabled { get { return staticSleepingContactsEnabled; } set { staticSleepingContactsEnabled = value; } }

		#region Solids
		/// <summary>
		/// Creates and returns a pointer to a Solid.
		/// </summary>
		/// <returns></returns>
		public abstract Solid CreateSolid();

		/// <summary>
		/// Helper function for creating a static Solid with a Plane Shape.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public virtual Solid CreatePlane(float a, float b, float c, float d, Material m)
		{
			// Create the plane's Solid and make it static.
			Solid plane = CreateSolid();
			plane.Static = true;

			Plane p = new Plane(a, b, c, d);

			// Setup the plane's Shape data.
			PlaneShapeData planeData = new PlaneShapeData(p);
			planeData.Material = m;
			//planeData.abcd[0] = a;
			//planeData.abcd[1] = b;
			//planeData.abcd[2] = c;
			//planeData.abcd[3] = d;

			// Add the Shape to the Solid.
			plane.AddShape(planeData);

			return plane;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <returns></returns>
		public Solid CreatePlane(float a, float b, float c, float d)
		{
			return CreatePlane(a, b, c, d, Defaults.Shape.Material);
		}

		/// <summary>
		/// Returns the number of Solids in the Simulator.
		/// </summary>
		public virtual int NumSolids
		{
			get
			{
				return solidList.Count;
			}
		}

		/// <summary>
		/// Returns a pointer to the Solid at the given index.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual Solid GetSolid(int i)
		{
			if (i >= SolidCount)
				return null;

			return solidList[i];
		}

		/// <summary>
		/// Immediately destroys the given Solid.  All Joints, Motors, and
		/// Sensors that depend on this Solid will be automatically
		/// disabled.
		/// </summary>
		/// <param name="s"></param>
		public virtual void DestroySolid(Solid s)
		{
			if (isSolidDestructionSafe)
			{
				RemoveSolid(s);
			}
			else
			{
				solidGarbageList.Add(s);
			}
		}

		/// <summary>
		/// Immediately destroys all Solids in the Simulator.
		/// </summary>
		public virtual void DestroyAllSolids()
		{
			for (int i = 0; i < solidList.Count;)
			{
				RemoveSolid(solidList[i]);
			}
		}

		public int SolidCount { get { return solidList.Count; } }
		#endregion
		#region Joints
		/// <summary>
		/// Creates and returns a pointer to a Joint.
		/// </summary>
		/// <returns></returns>
		public abstract Joint CreateJoint();

		/// <summary>
		/// Immediately destroys the given Joint.  All Motors that depend
		/// on this Solid will be automatically disabled.
		/// </summary>
		/// <param name="j"></param>
		public virtual void DestroyJoint(Joint j)
		{
			if (isJointDestructionSafe)
			{
				RemoveJoint(j);
			}
			else
			{
				jointGarbageList.Add(j);
			}
		}

		public virtual Joint GetJoint(int i)
		{
			if (i >= JointCount)
				return null;

			return jointList[i];
		}

		/// <summary>
		/// Immediately destroys all Joints in the Simulator.
		/// </summary>
		public virtual void DestroyAllJoints()
		{
			for (int i = 0; i < jointList.Count;)
			{
				RemoveJoint(jointList[i]);
			}
		}

		public int JointCount { get { return jointList.Count; } }
		#endregion
		#region Motors
		/// <summary>
		/// Creates and returns a pointer to a ThrusterMotor.
		/// </summary>
		/// <returns></returns>
		public virtual ThrusterMotor CreateThrusterMotor()
		{
			ThrusterMotor newMotor = new ThrusterMotor();
			AddMotor(newMotor);
			return newMotor;
		}

		/// <summary>
		/// Creates and returns a pointer to a VelocityMotor.
		/// </summary>
		/// <returns></returns>
		public virtual VelocityMotor CreateVelocityMotor()
		{
			VelocityMotor newMotor = new VelocityMotor(this);
			AddMotor(newMotor);
			return newMotor;
		}

		/// <summary>
		/// Creates and returns a pointer to a GearedMotor.
		/// </summary>
		/// <returns></returns>
		public virtual GearedMotor CreateGearedMotor()
		{
			GearedMotor newMotor = new GearedMotor();
			AddMotor(newMotor);
			return newMotor;
		}

		/// <summary>
		/// Creates and returns a pointer to a ServoMotor.
		/// </summary>
		/// <returns></returns>
		public virtual ServoMotor CreateServoMotor()
		{
			ServoMotor newMotor = new ServoMotor();
			AddMotor(newMotor);
			return newMotor;
		}

		/// <summary>
		/// Creates and returns a pointer to an AttractorMotor.
		/// </summary>
		/// <returns></returns>
		public virtual AttractorMotor CreateAttractorMotor()
		{
			AttractorMotor newMotor = new AttractorMotor();
			AddMotor((Motor)newMotor);
			return newMotor;
		}

		/// <summary>
		/// Creates and returns a pointer to a SpringMotor.
		/// </summary>
		/// <returns></returns>
		public virtual SpringMotor CreateSpringMotor()
		{
			SpringMotor newMotor = new SpringMotor();
			AddMotor((Motor)newMotor);
			return newMotor;
		}

		public virtual Motor GetMotor(int i)
		{
			if (i >= MotorCount)
				return null;

			return motorList[i];
		}

		/// <summary>
		/// Immediately destroys the given Motor.
		/// </summary>
		/// <param name="m"></param>
		public virtual void DestroyMotor(Motor m)
		{
			RemoveMotor(m);
		}

		/// <summary>
		/// Immediately destroys all Motors in the Simulator.
		/// </summary>
		public virtual void DestroyAllMotors()
		{
			motorList.Clear();
		}

		public int MotorCount { get { return motorList.Count; } }
		#endregion
		#region Sensors
		/// <summary>
		/// Creates and returns a pointer to an AccelerationSensor.
		/// </summary>
		/// <returns></returns>
		public virtual AccelerationSensor CreateAccelerationSensor()
		{
			AccelerationSensor newSensor = new AccelerationSensor(this);
			AddSensor(newSensor);
			return newSensor;
		}

		/// <summary>
		/// Creates and returns a pointer to an InclineSensor.
		/// </summary>
		/// <returns></returns>
		public virtual InclineSensor CreateInclineSensor()
		{
			InclineSensor newSensor = new InclineSensor();
			AddSensor(newSensor);
			return newSensor;
		}

		/// <summary>
		/// Creates and returns a pointer to a RaycastSensor.
		/// </summary>
		/// <returns></returns>
		public virtual RaycastSensor CreateRaycastSensor()
		{
			RaycastSensor newSensor = new RaycastSensor(this);
			AddSensor(newSensor);
			return newSensor;
		}

		/// <summary>
		/// Creates and returns a pointer to a VolumeSensor.
		/// </summary>
		/// <returns></returns>
		public virtual VolumeSensor CreateVolumeSensor()
		{
			VolumeSensor newSensor = new VolumeSensor(this);
			newSensor.Init(new VolumeSensorData());
			AddSensor(newSensor);
			return newSensor;
		}

		public virtual Sensor GetSensor(int i)
		{
			if (i >= SensorCount)
				return null;

			return sensorList[i];
		}

		/// <summary>
		/// Immediately destroys the given Sensor.
		/// </summary>
		/// <param name="s"></param>
		public virtual void DestroySensor(Sensor s)
		{
			RemoveSensor(s);
		}

		/// <summary>
		/// Immediately destroys all Sensors in the Simulator.
		/// </summary>
		public virtual void DestroyAllSensors()
		{
			sensorList.Clear();
		}

		public int SensorCount { get { return sensorList.Count; } }
		#endregion
		#region Spaces
		/// <summary>
		/// Creates and returns a pointer to a Space which is a child of
		/// the Simulator's root Space.
		/// </summary>
		/// <returns></returns>
		public abstract SpaceBase CreateSpace();

		public virtual SpaceBase RootSpace { get { return rootSpace; } }

		public virtual SpaceBase GetSpace(int i)
		{
			if (i >= SpaceCount)
				return null;

			return spaceList[i];
		}

		public int SpaceCount { get { return spaceList.Count; } }
		#endregion

		/// <summary>
		/// Returns the Simulator's contact group flags.
		/// </summary>
		/// <param name="groupNum"></param>
		/// <returns></returns>
		public virtual long InternalGetContactGroupFlags(int groupNum)
		{
			return contactGroupFlags[groupNum];
		}

		/// <summary>
		/// Record collisions into global handlers
		/// </summary>
		/// <param name="cevent"></param>
		public virtual void InternalRecordCollision(CollisionEvent cevent)
		{
			foreach (CollisionEventProcessor ceh in collisionEventHandlers)
				ceh.InternalPushCollisionEvent(cevent);
		}

		~Simulator()
		{
			DestroyAllSolids();
			DestroyAllJoints();
			DestroyAllMotors();
			DestroyAllSensors();

			spaceList.Clear();
			solidGarbageList.Clear();
			jointGarbageList.Clear();
		}

		/// <summary>
		/// This function is physics engine-specific.  It handles collision
		/// detection and steps the simulation ahead by a constant step
		/// size.
		/// </summary>
		protected abstract void StepPhysics();

		/// <summary>
		/// Adds a Solid to the internal list of Solids.
		/// </summary>
		/// <param name="s"></param>
		protected void AddSolid(Solid s)
		{
			solidList.Add(s);
		}

		/// <summary>
		/// Removes a Solid from the internal list of Solids.
		/// </summary>
		/// <param name="s"></param>
		protected void RemoveSolid(Solid s)
		{
			// Disable Motors that depend on the given Solid.
			foreach (Motor m in motorList)
				if (m.InternalDependsOnSolid(s))
					m.Enabled = false;

			// Disable Joints that depend on the given Solid.
			foreach (Joint j in jointList)
				if (j.InternalDependsOnSolid(s))
					j.Enabled = false;

			// Disable Sensors that depend on the given Solid.
			foreach (Sensor sens in sensorList)
				if (sens.InternalDependsOnSolid(s))
					sens.Enabled = false;

			// Now remove the Solid.
			solidList.Remove(s);
			s.Dispose();
		}

		/// <summary>
		/// Adds a Joint to the internal list of Joints.
		/// </summary>
		/// <param name="j"></param>
		protected void AddJoint(Joint j)
		{
			jointList.Add(j);
		}

		/// <summary>
		/// Removes a Joint from the internal list of Joints.
		/// </summary>
		/// <param name="j"></param>
		protected void RemoveJoint(Joint j)
		{
			// Disable Motors that depend on the given Joint.
			foreach (Motor m in motorList)
				if (m.InternalDependsOnJoint(j))
					m.Enabled = false;

			// Now remove the Joint.
			jointList.Remove(j);
		}

		/// <summary>
		/// Adds a Motor to the internal list of Motors.
		/// </summary>
		/// <param name="m"></param>
		protected void AddMotor(Motor m)
		{
			motorList.Add(m);
		}

		/// <summary>
		/// Removes a Motor from the internal list of Motors.
		/// </summary>
		/// <param name="m"></param>
		protected void RemoveMotor(Motor m)
		{
			motorList.Remove(m);
		}

		/// <summary>
		/// Adds a Sensor to the internal list of Sensors.
		/// </summary>
		/// <param name="s"></param>
		protected void AddSensor(Sensor s)
		{
			sensorList.Add(s);
		}

		/// <summary>
		/// Removes a Sensor from the internal list of Sensors.
		/// </summary>
		/// <param name="s"></param>
		protected void RemoveSensor(Sensor s)
		{
			sensorList.Remove(s);
		}

		/// <summary>
		/// Adds a Space to the internal list of Spaces.
		/// </summary>
		/// <param name="s"></param>
		protected void AddSpace(SpaceBase s)
		{
			spaceList.Add(s);
		}

		/// <summary>
		/// Destroys all objects marked as garbage.  Useful for destroying
		/// objects at safe times.
		/// </summary>
		protected void DestroyGarbage()
		{
			// Destroy garbage Solids.
			for (int i = 0; i < solidGarbageList.Count; i++)
			{
				DestroySolid(solidGarbageList[i]);
			}
			solidGarbageList.Clear();

			// Destroy garbage Joints.
			for (int i = 0; i < jointGarbageList.Count; i++)
			{
				DestroyJoint(jointGarbageList[i]);
			}
			jointGarbageList.Clear();
		}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Simulator: ");
            sb.Append(GetType().Name);
            sb.Append(" solids: ");
            sb.Append(this.solidList.Count);
            sb.Append(" joints: ");
            sb.Append(this.jointList.Count);
            sb.Append(" motors: ");
            sb.Append(this.motorList.Count);
            sb.Append(" sensors: ");
            sb.Append(this.sensorList.Count);

            return sb.ToString();
        }


		/// <summary>
		/// The constant step size used to break arbitrary simulation dt
		/// values into constant chunks.
		/// </summary>
		protected float stepSize;

		/// <summary>
		/// Maintains leftover dt from previous simulation steps.  This is
		/// useful when a dt smaller than the step size is requested; the
		/// dt will accumulate until there is enough for a single step.
		/// </summary>
		protected float timeBuffer;

		/// <summary>
		/// The accuracy level used internally by the physics engine's
		/// constraint solver.
		/// </summary>
		protected SolverAccuracyLevel solverAccuracyLevel;

		/// <summary>
		/// Pointer to user data.  This is totally user-managed (i.e. OPAL
		/// will never delete it).
		/// </summary>
		protected object userData;

		/// <summary>
		/// An internal list of all Solids.
		/// </summary>
		protected List<Solid> solidList = new List<Solid>();

		/// <summary>
		/// An internal list of all Joints.
		/// </summary>
		protected List<Joint> jointList = new List<Joint>();

		/// <summary>
		/// An internal list of all Motors.
		/// </summary>
		protected List<Motor> motorList = new List<Motor>();

		/// <summary>
		/// An internal list of all Sensors.
		/// </summary>
		protected List<Sensor> sensorList = new List<Sensor>();

		/// <summary>
		/// True when it is safe to destroy a Solid (e.g. not in the middle
		/// of looping over all the Solid).
		/// </summary>
		protected bool isSolidDestructionSafe;

		/// <summary>
		/// True when it is safe to destroy a Joint (e.g. not in the middle
		/// of looping over all the Joint).
		/// </summary>
		protected bool isJointDestructionSafe;

		/// <summary>
		/// An internal list of Solids marked as garbage.
		/// </summary>
		protected List<Solid> solidGarbageList = new List<Solid>();

		/// <summary>
		/// An internal list of Joints marked as garbage.
		/// </summary>
		protected List<Joint> jointGarbageList = new List<Joint>();

		/// <summary>
		/// Spaces are stored here so the user doesn't have to destroy them;
		/// they get destroyed when the Simulator is destroyed.
		/// </summary>
		protected List<SpaceBase> spaceList = new List<SpaceBase>();

		/// <summary>
		/// parent of all spaces
		/// </summary>
		protected SpaceBase rootSpace;

		/// <summary>
		/// A set of bitfields used to describe how different contact groups
		/// interact.
		/// </summary>
		protected long[] contactGroupFlags = new long[32];

		/// <summary>
		/// True if contacts are generated between static Solids and
		/// sleeping Solids.
		/// </summary>
		protected bool staticSleepingContactsEnabled;

		/// <summary>
		/// Pointer to the Simulator's post-step event handler.
		/// </summary>
		protected List<PostStepEventProcessor> postStepEventHandlers = new List<PostStepEventProcessor>();

		/// <summary>
		/// Global collision handlers.
		/// </summary>
		protected List<CollisionEventProcessor> collisionEventHandlers = new List<CollisionEventProcessor>();

		/// <summary>
		/// The maximum correcting velocity used when forcing apart
		/// interpenetrating objects.
		/// </summary>
		protected float maxCorrectingVel;

		/// <summary>
		/// The maximum number of physical contacts generated when two
		/// Solids collide.
		/// </summary>
		protected int maxContacts;

		/// <summary>
		/// data
		/// </summary>
		protected SimulatorData data;
	}
}