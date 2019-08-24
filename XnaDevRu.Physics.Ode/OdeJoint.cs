using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics.Ode
{
	public class OdeJoint : Joint
	{
		protected dJointID aMotorID;
		protected dJointID jointID;
		protected dWorldID worldID;
		protected Tao.Ode.Ode.dJointFeedback jointFeedback;

		#region Joint Implementation
		public override float GetAngle(int axisNum)
		{
			if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum", "axisNum must pe positive number that is less that numAxes");

			float value = 0f;

			switch (data.Type)
			{
				case JointType.Hinge:
					value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetHingeAngle(jointID));
					break;
				case JointType.Universal:
					if (0 == axisNum)
					{
						value =
							MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetUniversalAngle1(jointID));
					}
					else
					{
						value =
							MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetUniversalAngle2(jointID));
					}
					break;
				case JointType.Ball:
					value =
						MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetAMotorAngle(aMotorID, axisNum));
					break;
				case JointType.Slider:
					value = 0;
					break;
				case JointType.Wheel:
					if (0 == axisNum)
						value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetHinge2Angle1(jointID));
					else
						throw new PhysicsException("Wheel Joint: ODE does not give us axis 1 angle");
					break;
				case JointType.Fixed:
					throw new PhysicsException("Fixed Joints don't have any kind of state info");
					//break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			return value;
		}

		public override JointAxis GetAxis(int axisNum)
		{
			// Silently ignore invalid axes.
			if (axisNum < 0 || axisNum >= numAxes)
			{
				return new JointAxis();
			}

			// First we need to get an updated direction vector from ODE.
			Tao.Ode.Ode.dVector3 direction = new Tao.Ode.Ode.dVector3();

			switch (data.Type)
			{
				case JointType.Hinge:
					Tao.Ode.Ode.dJointGetHingeAxis(jointID, ref direction);
					break;
				case JointType.Universal:
					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointGetUniversalAxis1(jointID, ref direction);
					}
					else
					{
						Tao.Ode.Ode.dJointGetUniversalAxis2(jointID, ref direction);
					}
					break;
				case JointType.Ball:
					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointGetAMotorAxis(aMotorID, 0, ref direction);
					}
					else if (1 == axisNum)
					{
						Tao.Ode.Ode.dJointGetAMotorAxis(aMotorID, 1, ref direction);
					}
					else
					{
						Tao.Ode.Ode.dJointGetAMotorAxis(aMotorID, 2, ref direction);
					}
					break;
				case JointType.Slider:
					Tao.Ode.Ode.dJointGetSliderAxis(jointID, ref direction);
					break;
				case JointType.Wheel:
					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointGetHinge2Axis1(jointID, ref direction);
					}
					else
					{
						Tao.Ode.Ode.dJointGetHinge2Axis2(jointID, ref direction);
					}
					break;
				case JointType.Fixed:
					// Fixed Joints don't have any axes.
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			JointAxis axis = data.Axis[axisNum];

			// All data in this JointAxis is valid except for the direction
			// vector.
			axis.Direction = direction;

			return axis;
		}

		public override float GetDistance(int axisNum)
		{
			if(axisNum<0||axisNum>=numAxes)
				throw new ArgumentOutOfRangeException("axisNum", "axisNum must be positive number that is less than numAxes");

			float value = 0;

			switch (data.Type)
			{
				case JointType.Hinge:
					value = 0;
					break;
				case JointType.Universal:
					value = 0;
					break;
				case JointType.Ball:
					value = 0;
					break;
				case JointType.Slider:
					value = Tao.Ode.Ode.dJointGetSliderPosition(jointID);
					break;
				case JointType.Wheel:
					value = 0;
					break;
				case JointType.Fixed:
					throw new PhysicsException("Fixed Joints don't have any kind of state info");
					//break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			return value;
		}

		public override float GetVelocity(int axisNum)
		{
			if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum", "axisNum must pe positive number that is less that numAxes");

			float value = 0;

			switch (data.Type)
			{
				case JointType.Hinge:
					value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetHingeAngleRate(jointID));
					break;
				case JointType.Universal:
					if (0 == axisNum)
					{
						value=MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetUniversalAngle1Rate(jointID));
					}
					else
					{
						value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetUniversalAngle2Rate(jointID));
					}
					break;
				case JointType.Ball:
					value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetAMotorAngleRate(aMotorID, axisNum));
					break;
				case JointType.Slider:
					value = Tao.Ode.Ode.dJointGetSliderPositionRate(jointID);
					break;
				case JointType.Wheel:
					if (0 == axisNum)
					{
						value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetHinge2Angle1Rate(jointID));
					}
					else
					{
						value = MathHelper.ToDegrees(Tao.Ode.Ode.dJointGetHinge2Angle2Rate(jointID));
					}
					break;
				case JointType.Fixed:
					throw new PhysicsException("Fixed Joints don't have rates");
					//break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			return value;
		}

		protected override void InternalSetDesiredVel(int axisNum, float value)
		{
			if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum", "axisNum must pe positive number that is less that numAxes");

			switch (axisNum)
			{
				case 0:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamVel, value);
					break;
				case 1:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamVel2, value);
					break;
				case 2:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamVel3, value);
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		protected override void InternalSetMaxTorque(int axisNum, float value)
		{
			if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum", "axisNum must pe positive number that is less that numAxes");

			switch (axisNum)
			{
				case 0:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFMax, value);
					break;
				case 1:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFMax2, value);
					break;
				case 2:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFMax3, value);
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		protected override void InternalUpdate()
		{
			if (data.Enabled)
			{
				// Update the Joint's damage status.
				UpdateDamage(CalcStress());
			}
		}
		#endregion

		public OdeJoint(dWorldID worldID)
		{
			aMotorID = null;
			jointID = null;
			this.worldID = worldID;
		}

		~OdeJoint()
		{
			jointID.Destroy();
			if (aMotorID != null)
			{
				aMotorID.Destroy();
			}
		}

		/// Note: For a perfect save/restore in ODE, the "warm starting"
		/// data should be stored in JointData.  However, there is
		/// currently no easy way to get this data.
		public override void Init(JointData data)
		{
			if (initCalled)
			{
				// If this Joint has already been initialized, destroy the ODE
				// Joint ID.
				jointID.Destroy();
				if (aMotorID != null)
				{
					aMotorID.Destroy();
				}

				// Reset event handler.
				jointBreakEventHandler = null;
			}

			base.Init(data);
			initCalled = true;

			switch (data.Type)
			{
				case JointType.Hinge:
					// Create an ODE hinge Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateHinge(worldID, IntPtr.Zero));
					numAxes = 1;

					// Set the rotational property of each axis.
					axisRotational[0] = true;
					axisRotational[1] = false;
					axisRotational[2] = false;

					// Set the ODE fudge factor for each axis.
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor,
								   Defaults.Ode.JointFudgeFactor);
					break;
				case JointType.Universal:
					// Create an ODE universal Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateUniversal(worldID, IntPtr.Zero));
					numAxes = 2;

					// Set the rotational property of each axis.
					axisRotational[0] = true;
					axisRotational[1] = true;
					axisRotational[2] = false;

					// Set the ODE fudge factor for each axis.
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor,
								   Defaults.Ode.JointFudgeFactor);
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor2,
								   Defaults.Ode.JointFudgeFactor);
					break;
				case JointType.Ball:
					// Create an ODE ball Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateBall(worldID, IntPtr.Zero));
					numAxes = 3;

					// Set the rotational property of each axis.
					axisRotational[0] = true;
					axisRotational[1] = true;
					axisRotational[2] = true;

					// ODE ball Joints need this special "angular motor" to
					// restrain their movement e.g. when limits are used.
					aMotorID = new dJointID(Tao.Ode.Ode.dJointCreateAMotor(worldID, IntPtr.Zero));
					Tao.Ode.Ode.dJointSetAMotorMode(aMotorID, (int)Tao.Ode.Ode.dAMotorMode.dAMotorEuler);

					// Set the ODE fudge factor for each axis.
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor,
								   Defaults.Ode.JointFudgeFactor);
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor2,
								   Defaults.Ode.JointFudgeFactor);
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor3,
								   Defaults.Ode.JointFudgeFactor);
					break;
				case JointType.Slider:
					// Create an ODE slider Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateSlider(worldID, IntPtr.Zero));
					numAxes = 1;

					// Set the rotational property of each axis.
					axisRotational[0] = false;
					axisRotational[1] = false;
					axisRotational[2] = false;

					// Set the ODE fudge factor for each axis.
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor,
								   Defaults.Ode.JointFudgeFactor);
					break;
				case JointType.Wheel:
					// Create an ODE hinge2 Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateHinge2(worldID, IntPtr.Zero));
					numAxes = 2;

					// Set the rotational property of each axis.
					axisRotational[0] = true;
					axisRotational[1] = true;
					axisRotational[2] = false;

					// Set the ODE fudge factor for each axis.
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor,
								   Defaults.Ode.JointFudgeFactor);
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamFudgeFactor2,
								   Defaults.Ode.JointFudgeFactor);
					break;
				case JointType.Fixed:
					// Create an ODE fixed Joint.
					jointID = new dJointID(Tao.Ode.Ode.dJointCreateFixed(worldID, IntPtr.Zero));
					numAxes = 0;

					// Set the rotational property of each axis.
					axisRotational[0] = false;
					axisRotational[1] = false;
					axisRotational[2] = false;
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			// Tell ODE about the JointFeedback struct for this Joint.
			jointFeedback = new Tao.Ode.Ode.dJointFeedback();
			Tao.Ode.Ode.dJointSetFeedback(jointID, ref jointFeedback);

			// Setup the Solids.
			FilterSolidForStaticness(data.Solid0, data.Solid1);

			if (!data.IsBroken)
			{
				// Attach the Joint to the ODE bodies.
				AttachODEBodies(data.Solid0, data.Solid1);
			}

			// Setup the Joint's anchor.
			SetAnchor(data.Anchor);

			// Setup the Joint's axes.
			SetAxis(0, data.Axis[0]);
			SetAxis(1, data.Axis[1]);
			SetAxis(2, data.Axis[2]);

			// Set the ODE joint's userdata pointer.
			if (JointType.Ball == data.Type)
			{
				Tao.Ode.Ode.dJointSetData(aMotorID, GCHandle.ToIntPtr(GCHandle.Alloc(this)));
			}
			else
			{
				Tao.Ode.Ode.dJointSetData(jointID, GCHandle.ToIntPtr(GCHandle.Alloc(this)));
			}
		}

		public override void SetLimitsEnabled(int axisNum, bool e)
		{
			base.SetLimitsEnabled(axisNum, e);

			if (e)
			{
				float low = data.Axis[axisNum].Limits.Low;
				float high = data.Axis[axisNum].Limits.High;

				if (IsRotational(axisNum))
				{
					low = MathHelper.ToRadians(low);
					high = MathHelper.ToRadians(high);
				}

				switch (axisNum)
				{
					case 0:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, high);
						break;
					case 1:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, high);
						break;
					case 2:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, high);
						break;
					default:
						throw new PhysicsException("Unknown bug");
						//break;
				}
			}
			else
			{
				switch (axisNum)
				{
					case 0:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, float.PositiveInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, float.PositiveInfinity);
						break;
					case 1:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, float.PositiveInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, float.PositiveInfinity);
						break;
					case 2:
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, float.PositiveInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, float.NegativeInfinity);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, float.PositiveInfinity);
						break;
					default:
						throw new PhysicsException("Unknown bug");
						//break;
				}
			}
		}

		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				if (!initCalled)
				{
					return;
				}

				// if are enabling a joint, we need to properly alter the solids
				if (value == true)
					FilterSolidForStaticness(data.Solid0, data.Solid1);

				if (null == data.Solid0 && null == data.Solid1)
				{
					return;
				}
				else
				{
					base.Enabled = value;

					if (value)
					{
						// Enable the joint.
						AttachODEBodies(data.Solid0, data.Solid1);
					}
					else
					{
						// Disable the joint by setting both bodies to 0.
						AttachODEBodies(null, null);
					}
				}
			}
		}

		public override void SetLimitRange(int axisNum, float low,
											  float high)
		{
			base.SetLimitRange(axisNum, low, high);

			// If this axis is rotational, convert the limit angles from
			// degrees to radians.
			if (IsRotational(axisNum))
			{
				low = MathHelper.ToRadians(low);
				high = MathHelper.ToRadians(high);
			}

			// Only set the ODE limits if the limits for this particular axis
			// are enabled.
			switch (axisNum)
			{
				case 0:
					if (data.Axis[axisNum].LimitsEnabled)
					{
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop, high);
					}
					break;
				case 1:
					if (data.Axis[axisNum].LimitsEnabled)
					{
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop2, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop2, high);
					}
					break;
				case 2:
					if (data.Axis[axisNum].LimitsEnabled)
					{
						// Both limits must be set twice because of a ODE bug in
						// the limit setting function.
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, high);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamLoStop3, low);
						SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamHiStop3, high);
					}
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		public override void SetLimitHardness(int axisNum, float h)
		{
			base.SetLimitHardness(axisNum, h);

			// Calculate the ODE ERP value by scaling hardness by a normalized
			// ERP range.
			float value = h * (Defaults.Ode.MaxERP - Defaults.Ode.MinERP) +
						 Defaults.Ode.MinERP;

			// Set the ODE ERP parameter.
			switch (axisNum)
			{
				case 0:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamStopERP, value);
					break;
				case 1:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamStopERP2, value);
					break;
				case 2:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamStopERP3, value);
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			// ODE's Hinge2 Joint also has a suspension parameter.  Use axis 1
			// for this since axis 1 doesn't use limits anyway.
			if (1 == axisNum && JointType.Wheel == data.Type)
			{
				SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamSuspensionERP, value);
			}
		}

		public override void SetLimitBounciness(int axisNum, float b)
		{
			base.SetLimitBounciness(axisNum, b);

			// Calculate the ODE bounce value by scaling hardness by a
			// normalized ERP range.
			float value = b * (Defaults.Ode.MaxERP - Defaults.Ode.MinERP) +
						 Defaults.Ode.MinERP;

			// Set the ODE bounce parameter.
			switch (axisNum)
			{
				case 0:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamBounce, value);
					break;
				case 1:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamBounce2, value);
					break;
				case 2:
					SetJointParam((int)Tao.Ode.Ode.dJointParams.dParamBounce3, value);
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		public override Vector3 Anchor
		{
			get
			{
				// We need to get an updated anchor point from ODE.
				Tao.Ode.Ode.dVector3 anchor = new Tao.Ode.Ode.dVector3();

				switch (data.Type)
				{
					case JointType.Hinge:
						Tao.Ode.Ode.dJointGetHingeAnchor(jointID, ref anchor);
						break;
					case JointType.Universal:
						Tao.Ode.Ode.dJointGetUniversalAnchor(jointID, ref anchor);
						break;
					case JointType.Ball:
						Tao.Ode.Ode.dJointGetBallAnchor(jointID, ref anchor);
						break;
					case JointType.Slider:
						// Slider Joints don't have an anchor point.
						break;
					case JointType.Wheel:
						Tao.Ode.Ode.dJointGetHinge2Anchor(jointID, ref anchor);
						break;
					case JointType.Fixed:
						// Fixed Joints don't have an anchor point.
						break;
					default:
						throw new PhysicsException("Unknown bug");
						//break;
				}

				return anchor;
			}
		}

		/// Returns the ODE joint ID of this ODEJoint.
		public dJointID InternalGetJointID()
		{
			if (JointType.Ball == data.Type)
			{
				return aMotorID;
			}
			else
			{
				return jointID;
			}
		}

		protected override void SetAxis(int axisNum, JointAxis axis)
		{
			// Silently ignore invalid axes.
			if (axisNum < 0 || axisNum >= numAxes)
			{
				return;
			}

			// Normalize the direction vector.
			JointAxis normAxis = axis;
			normAxis.Direction.Normalize();

			// Store the axis (with normalized direction vector).
			base.SetAxis(axisNum, normAxis);

			// Create an ODE-style vector for the direction vector.
			Vector3 newAxis = normAxis.Direction;

			// Set the ODE Joint axis direction vector.
			switch (data.Type)
			{
				case JointType.Hinge:
					Tao.Ode.Ode.dJointSetHingeAxis(jointID, newAxis.X, newAxis.Y,
										newAxis.Z);
					break;
				case JointType.Universal:
					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointSetUniversalAxis1(jointID, newAxis.X, newAxis.Y,
												 newAxis.Z);
					}
					else
					{
						Tao.Ode.Ode.dJointSetUniversalAxis2(jointID, newAxis.X, newAxis.Y,
												 newAxis.Z);
					}
					break;
				case JointType.Ball:
					// From ODE docs:
					// For dAMotorEuler mode:
					// 1. Only axes 0 and 2 need to be set. Axis 1 will be
					//	determined automatically at each time step.
					// 2. Axes 0 and 2 must be perpendicular to each other.
					// 3. Axis 0 must be anchored to the first body, axis 2 must
					//	be anchored to the second body.

					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointSetAMotorAxis(aMotorID, 0, 1, newAxis.X,
											 newAxis.Y, newAxis.Z);
					}
					else if (2 == axisNum)
					{
						Tao.Ode.Ode.dJointSetAMotorAxis(aMotorID, 2, 2, newAxis.X,
											 newAxis.Y, newAxis.Z);
					}
					break;
				case JointType.Slider:
					Tao.Ode.Ode.dJointSetSliderAxis(jointID, newAxis.X, newAxis.Y,
										 newAxis.Z);
					break;
				case JointType.Wheel:
					if (0 == axisNum)
					{
						Tao.Ode.Ode.dJointSetHinge2Axis1(jointID, newAxis.X, newAxis.Y,
											  newAxis.Z);
					}
					else
					{
						Tao.Ode.Ode.dJointSetHinge2Axis2(jointID, newAxis.X, newAxis.Y,
											  newAxis.Z);
					}
					break;
				case JointType.Fixed:
					// Fixed Joints don't have any axes.
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			if (normAxis.Limits != null)
			{
				// Set whether the axis limits are enabled.
				SetLimitsEnabled(axisNum, normAxis.LimitsEnabled);

				// Set the axis' limit range.
				SetLimitRange(axisNum, normAxis.Limits.Low, normAxis.Limits.High);

				// Set the limits' hardness.
				SetLimitHardness(axisNum, normAxis.Limits.Hardness);

				// Set the limits' bounciness.
				SetLimitBounciness(axisNum, normAxis.Limits.Bounciness);
			}
		}

		protected override void SetAnchor(Vector3 anchor)
		{
			base.SetAnchor(anchor);
			Vector3 newAnchor = anchor;

			switch (data.Type)
			{
				case JointType.Hinge:
					Tao.Ode.Ode.dJointSetHingeAnchor(jointID, newAnchor.X, newAnchor.Y,
										  newAnchor.Z);
					break;
				case JointType.Universal:
					Tao.Ode.Ode.dJointSetUniversalAnchor(jointID, newAnchor.X,
											  newAnchor.Y, newAnchor.Z);
					break;
				case JointType.Ball:
					Tao.Ode.Ode.dJointSetBallAnchor(jointID, newAnchor.X, newAnchor.Y,
										 newAnchor.Z);
					break;
				case JointType.Slider:
					// Slider Joints don't have an anchor point.
					break;
				case JointType.Wheel:
					Tao.Ode.Ode.dJointSetHinge2Anchor(jointID, newAnchor.X, newAnchor.Y,
										   newAnchor.Z);
					break;
				case JointType.Fixed:
					// Fixed Joints don't have an anchor point.
					break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		/// Helper function to make it easier to set parameters for
		/// various ODE Joint types.
		protected void SetJointParam(int parameter, float value)
		{
			switch (data.Type)
			{
				case JointType.Hinge:
					Tao.Ode.Ode.dJointSetHingeParam(jointID, (Tao.Ode.Ode.dJointParams)parameter, value);
					break;
				case JointType.Universal:
					Tao.Ode.Ode.dJointSetUniversalParam(jointID, (Tao.Ode.Ode.dJointParams)parameter, value);
					break;
				case JointType.Ball:
					Tao.Ode.Ode.dJointSetAMotorParam(aMotorID, (Tao.Ode.Ode.dJointParams)parameter, value);
					break;
				case JointType.Slider:
					Tao.Ode.Ode.dJointSetSliderParam(jointID, (Tao.Ode.Ode.dJointParams)parameter, value);
					break;
				case JointType.Wheel:
					Tao.Ode.Ode.dJointSetHinge2Param(jointID, (Tao.Ode.Ode.dJointParams)parameter, value);
					break;
				case JointType.Fixed:
					throw new PhysicsException("Fixed Joints does not have params");
					//break;
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}
		}

		/// Returns the current amount of stress on this Joint.
		protected virtual float CalcStress()
		{
			float currentStress = 0;

			switch (data.BreakMode)
			{
				case JointBreakMode.Unbreakable:
					// Nothing to do.
					break;
				case JointBreakMode.Threshold:
				// Fall through...
				case JointBreakMode.Accumulated:
					{
						Tao.Ode.Ode.dJointFeedback jf = Tao.Ode.Ode.dJointGetFeedback(jointID);
						Vector3 f1 = jf.f1;
						Vector3 t1 = jf.t1;
						Vector3 f2 = jf.f2;
						Vector3 t2 = jf.t2;

						f1 -= f2;
						t1 -= t2;

						// This is a simplification, but it should still work.
						currentStress = f1.Length() + t1.Length();
						break;
					}
				default:
					throw new PhysicsException("Unknown bug");
					//break;
			}

			return currentStress;
		}

		/// Attaches the ODE Joint to the given ODESolids.
		protected virtual void AttachODEBodies(Solid s0, Solid s1)
		{
			OdeSolid solid0 = s0 as  OdeSolid;
			OdeSolid solid1 = s1 as OdeSolid;

			if (null == solid0 && null == solid1)
			{
				Tao.Ode.Ode.dJointAttach(jointID, IntPtr.Zero, IntPtr.Zero);
				if (JointType.Ball == data.Type)
				{
					Tao.Ode.Ode.dJointAttach(aMotorID, IntPtr.Zero, IntPtr.Zero);
				}
			}
			else
			{
				if (null == solid0)
				{
					Tao.Ode.Ode.dJointAttach(jointID, IntPtr.Zero, solid1.InternalGetBodyID());
					if (JointType.Ball == data.Type)
					{
						Tao.Ode.Ode.dJointAttach(aMotorID, IntPtr.Zero, solid1.InternalGetBodyID());
					}
				}
				else if (null == solid1)
				{
					Tao.Ode.Ode.dJointAttach(jointID, solid0.InternalGetBodyID(), IntPtr.Zero);
					if (JointType.Ball == data.Type)
					{
						Tao.Ode.Ode.dJointAttach(aMotorID, solid0.InternalGetBodyID(), IntPtr.Zero);
					}
				}
				else
				{
					Tao.Ode.Ode.dJointAttach(jointID, solid0.InternalGetBodyID(),
								  solid1.InternalGetBodyID());
					if (JointType.Ball == data.Type)
					{
						Tao.Ode.Ode.dJointAttach(aMotorID, solid0.InternalGetBodyID(),
									  solid1.InternalGetBodyID());
					}
				}

				// Special call for fixed Joints so they remember the current
				// relationship between the Solids or between a Solid and the
				// static environment.
				if (JointType.Fixed == data.Type)
				{
					Tao.Ode.Ode.dJointSetFixed(jointID);
				}
			}
		}

		//! Static solids are set to NULL internally
		/*!
		 * @note this handles the cases when either both are being set
		 *       to NULL or both are the same Solid.
		 */
		protected void FilterSolidForStaticness(Solid s0, Solid s1)
		{
			Solid temp0 = s0;
			Solid temp1 = s1;
			data.Enabled = true;
			if (s0 == s1)
			{
				temp0 = null;
				temp1 = null;
				data.Enabled = false;
			}
			else
			{
				if (s0 != null && s0.Static)
					temp0 = null;
				if (s1 != null && s1.Static)
					temp1 = null;

				if (temp0 == null && temp1 == null)
					data.Enabled = false;
			}

			// now we can safely set
			base.SetSolids(temp0, temp1);
		}
	}
}
