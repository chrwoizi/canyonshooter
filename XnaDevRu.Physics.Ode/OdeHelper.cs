using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics.Ode
{
	internal static class OdeHelper
	{
        static internal void CollisionCallbackRunner(IntPtr data, IntPtr o0, IntPtr o1)
        {
            CollisionCallback(data, o0, o1);
        }

		/// Main collision callback functor.
        static unsafe internal void CollisionCallback(IntPtr data, dGeomID o0, dGeomID o1)
		{
			if (o0.IsSpace || o1.IsSpace)
			{
				// Colliding a space with either a geom or another space.
				Tao.Ode.Ode.dSpaceCollide2(o0, o1, data, CollisionCallbackRunner);

				if (o0.IsSpace)
				{
					// Colliding all geoms internal to the space.
                    Tao.Ode.Ode.dSpaceCollide(o0, data, CollisionCallbackRunner);
				}

				if (o1.IsSpace)
				{
					// Colliding all geoms internal to the space.
                    Tao.Ode.Ode.dSpaceCollide(o1, data, CollisionCallbackRunner);
				}
			}
			else
			{
				// Colliding two geoms.

				// The following is a set of special cases where we might
				// want to skip collision detection (but not all are
				// enforced here for various reasons):
				// 1. Two static Solids (neither geom has a body) AND
				//    neither Solid has a CollisionEventHandler AND there are
				//    not global handlers: this is enforced.
				// 2. Two Shapes that are part of the same Solid (they
				//    share a body): this is not enforced because ODE
				//    already takes care of it.
				// 3. Two sleeping Solids (note: both must have bodies to
				//    check this): this is enforced.  (ODE should handle
				//    this, but it doesn't.)
				// 4. Two Solids connected by a fixed Joint: this is
				//    enforced.
				// 5. Two Solids connected by a Joint (besides ODE
				//    contact joints, which should never generate more
				//    contacts) with contacts disabled (note: both must have
				//    bodies to check this): this is enforced.
				// 6. Solid0 is static, Solid1 is dynamic and is sleeping,
				//    static-to-sleeping contacts are ignored by the
				//    Simulator, and neither Solid has a
				//    CollisionEventHandler AND there are no global handlers:
				//    this is enforced.
				// 7. Solid1 is static, Solid0 is dynamic and is sleeping,
				//    static-to-sleeping contacts are ignored by the
				//    Simulator, and neither Solid has a
				//    CollisionEventHandler AND there are no global handlers:
				//    this is enforced.
				// 8. The two Solids' contact groups do not generate
				//    contacts when they collide AND neither Solid has a
				//    CollisionEventHandler AND there are no global handlers.

				// Get the geoms' ODE body IDs.
                dBodyID o0BodyID = o0.RigidBody;
				dBodyID o1BodyID = o1.RigidBody;

                bool solid0Static = PtrWrapper.IsEmpty(o0BodyID); // (mike)
				bool solid1Static = PtrWrapper.IsEmpty(o1BodyID); // {mike}

				// Check if both Solids are dynamic (i.e. have ODE bodies).
				bool bothHaveBodies = true;
				if (solid0Static || solid1Static)
				{
					bothHaveBodies = false;
				}

				// If the two Solids are connected by a common Joint, get
				// a pointer to that Joint.
				Joint commonJoint = null;

                if (bothHaveBodies)
                {
                    int connExcluding = Tao.Ode.Ode.dAreConnectedExcluding(o0BodyID, o1BodyID, (int)Tao.Ode.Ode.dJointTypes.dJointTypeContact);

                    if (connExcluding != 0)
                    {
                        // This will become non-NULL only if there exists an ODE
                        // joint connecting the two bodies.
                        commonJoint = GetCommonJoint(o0BodyID, o1BodyID);
                    }
                }

				// Get pointers to the geoms' GeomData structures.
				GeomData geomData0 = GCHandle.FromIntPtr(Tao.Ode.Ode.dGeomGetData(o0)).Target as GeomData;
				GeomData geomData1 = GCHandle.FromIntPtr(Tao.Ode.Ode.dGeomGetData(o1)).Target as GeomData;

				// Get pointers to the geoms' ShapeData structures.
				ShapeData shape0 = geomData0.Shape;
				ShapeData shape1 = geomData1.Shape;

				// Get a pointer to the ODESimulator.
				OdeSimulator sim = GCHandle.FromIntPtr(data).Target as OdeSimulator;

				// Check if the two Solids' contact groups generate contacts
				// when they collide.
				bool makeContacts = sim.GroupsMakeContacts(shape0.ContactGroup, shape1.ContactGroup);

				// Find out whether the Simulator has static-to-sleeping
				// contacts disabled.
				bool ignoreStaticSleepingContacts = !sim.IsStaticSleepingContactsEnabled;

				// Get pointers to the geoms' Solids.
				Solid solid0 = geomData0.Solid;
				Solid solid1 = geomData1.Solid;

				// Get pointers to the two Solids' CollisionEventHandlers.
				// These will be NULL if the Solids don't use
				// CollisionEventHandlers.
				CollisionEventProcessor handler0 = solid0.CollisionEventHandler;
				CollisionEventProcessor handler1 = solid1.CollisionEventHandler;

				bool neitherHasEventHandler = !(handler0 != null || handler1 != null);

				bool hasNoGlobalHandler = sim.NumGlobalCollisionEventHandlers == 0;

				// Now do the actual tests to see if we should return early.
				// It is important here that we don't call dBodyIsEnabled on
				// a static body because that crashes ODE.

				bool case1 = neitherHasEventHandler && hasNoGlobalHandler
							 && solid0Static && solid1Static;
				//bool case2= o0BodyID == o1BodyID;
				bool case3 = bothHaveBodies && Tao.Ode.Ode.dBodyIsEnabled(o0BodyID) == 0
							 && Tao.Ode.Ode.dBodyIsEnabled(o1BodyID) == 0;
				bool case4 = commonJoint != null &&
							 commonJoint.Type == JointType.Fixed;
				bool case5 = commonJoint != null
							 && !commonJoint.ContactsEnabled;
				bool case6 = solid0Static && null != o1BodyID && o1BodyID.IsNotNull()
							 && Tao.Ode.Ode.dBodyIsEnabled(o1BodyID) == 0
							 && ignoreStaticSleepingContacts
							 && neitherHasEventHandler && hasNoGlobalHandler;
				bool case7 = solid1Static && null != o0BodyID && o0BodyID.IsNotNull()
							 && Tao.Ode.Ode.dBodyIsEnabled(o0BodyID) == 0
							 && ignoreStaticSleepingContacts
							 && neitherHasEventHandler && hasNoGlobalHandler;
				bool case8 = !makeContacts && neitherHasEventHandler
							 && hasNoGlobalHandler;

				if (case1 || case3 || case4 || case5 || case6 || case7 || case8)
					return;

				// Now actually test for collision between the two geoms.
				// This is one of the more expensive operations.
				IntPtr theWorldID = sim.World;
				IntPtr theJointGroupID = sim.JointGroup;
				int contGeomSize = sizeof(Tao.Ode.Ode.dContactGeom);
                Tao.Ode.Ode.dContactGeom[] contactArray = new Tao.Ode.Ode.dContactGeom[sim.MaxContacts];

                int numContacts = 0;

                /*try
                {*/
                    numContacts = Tao.Ode.Ode.dCollide(o0, o1, sim.MaxContacts /*was 15*/, contactArray, contGeomSize); 
					// Was big perfomance problem here (KleMiX)
                /*}
                catch(Exception)
                {
                    return;
                }*/

				// If the two objects didn't make any contacts, they weren't
				// touching, so just return.
				if (0 == numContacts)
				{
					return;
				}

				// If at least one of the Solids has a CollisionEventHandler,
				// send it a CollisionEvent.
				if (handler0 != null || handler1 != null || !hasNoGlobalHandler)
				{
					// Call the CollisionEventHandlers.  Note that we only
					// use one contact point per collision: just the first one
					// in the contact array.  The order of the Solids
					// passed to the event handlers is important: the first
					// one should be the one whose event handler is
					// getting called.

					CollisionEvent e = new CollisionEvent();

					e.ThisSolid = solid0;
					e.OtherSolid = solid1;
					e.Position.X = contactArray[0].pos.X;
					e.Position.Y = contactArray[0].pos.Y;
					e.Position.Z = contactArray[0].pos.Z;
					e.Normal.X = contactArray[0].normal.X;
					e.Normal.Y = contactArray[0].normal.Y;
					e.Normal.Z = contactArray[0].normal.Z;
					e.Depth = contactArray[0].depth;

					if (handler0 != null)
					{
						handler0.HandleCollisionEvent(e);
					}

					if (handler1 != null)
					{
						// For the other Solid's CollisionEventHandler, we need
						// to invert the normal and swap the Solid pointers.
						e.Normal *= -1;
						e.ThisSolid = solid1;
						e.OtherSolid = solid0;
						handler1.HandleCollisionEvent(e);
					}

					sim.InternalRecordCollision(e);
				}

				if (makeContacts)
				{
					// Invalidate the "freely-spinning" parameters.
					((OdeSolid)solid0).InternalSetFreelySpinning(false);
					((OdeSolid)solid1).InternalSetFreelySpinning(false);

					for (int i = 0; i < numContacts; ++i)
					{
						Material m0 = shape0.Material;
						Material m1 = shape1.Material;

						Tao.Ode.Ode.dContact tempContact = new Tao.Ode.Ode.dContact();
						tempContact.surface.mode = (int)Tao.Ode.Ode.dContactFlags.dContactBounce |
							(int)Tao.Ode.Ode.dContactFlags.dContactSoftERP;

						// Average the hardness of the two materials.
						float hardness = (m0.Hardness + m1.Hardness) * 0.5f;

						// Convert hardness to ERP.  As hardness goes from
						// 0.0 to 1.0, ERP goes from min to max.
						tempContact.surface.soft_erp = hardness *
							(Defaults.Ode.MaxERP - Defaults.Ode.MinERP) +
							Defaults.Ode.MinERP;

						// Don't use contact CFM anymore.  Just let it use
						// the global value set in the ODE world.
						//tempContact.surface.soft_cfm =
						//  defaults::ode::minCFM;

						// As friction goes from 0.0 to 1.0, mu goes from 0.0
						// to max, though it is set to dInfinity when
						// friction == 1.0.
						if (1.0 == m0.Friction && 1.0 == m1.Friction)
						{
							tempContact.surface.mu = float.PositiveInfinity;
						}
						else
						{
							tempContact.surface.mu = (float)Math.Sqrt(m0.Friction * m1.Friction) * Defaults.Ode.MaxFriction;
						}

						// Average the bounciness of the two materials.
						float bounciness = (m0.Bounciness + m1.Bounciness) * 0.5f;

						// ODE's bounce parameter, a.k.a. restitution.
						tempContact.surface.bounce = bounciness;

						// ODE's bounce_vel parameter is a threshold:
						// the relative velocity of the two objects must be
						// greater than this for bouncing to occur at all.
						tempContact.surface.bounce_vel = Defaults.BounceThreshold;

						tempContact.geom = contactArray[i];
						//ODE.Joints.Contact contactJoint = new ODE.Joints.Contact(theWorldID, tempContact, theJointGroupID);
						IntPtr contactJoint = Tao.Ode.Ode.dJointCreateContact(theWorldID, theJointGroupID, ref tempContact);

						//contactJoint.Attach(o0BodyID, o1BodyID);
						Tao.Ode.Ode.dJointAttach(contactJoint, o0BodyID, o1BodyID);
					}
				}
			}
		}

		/// Assuming the two ODE bodies are connected by an ODE joint, this
		/// function returns the OPAL Joint connecting the two bodies'
		/// Solids.
		static internal Joint GetCommonJoint(IntPtr body0, IntPtr body1)
		{
			// First we need to find the ODE joint connecting the bodies
			// (it would be ideal if ODE included this functionality...).
			// We only need to check one of the bodies' ODE joints
			// because it is assumed here that the two bodies are
			// connected, thus they should have a common ODE joint.
			int numJoints0 = Tao.Ode.Ode.dBodyGetNumJoints(body0);
			IntPtr theJoint = IntPtr.Zero;

			// Loop through body0's ODE joints.
			for (int i = 0; i < numJoints0; ++i)
			{
				/*ODE.Joints.Joint currentJoint = dBodyGetJoint(body0, i);
				ODE.Body jointBody0 = currentJoint.GetAttachedBody(0);
				ODE.Body jointBody1 = currentJoint.GetAttachedBody(1);*/
				IntPtr currentJoint = Tao.Ode.Ode.dBodyGetJoint(body0, i);
				IntPtr jointBody0 = Tao.Ode.Ode.dJointGetBody(currentJoint, 0);
				IntPtr jointBody1 = Tao.Ode.Ode.dJointGetBody(currentJoint, 1);

				if ((jointBody0 == body0 && jointBody1 == body1) ||
					(jointBody0 == body1 && jointBody1 == body0))
				{
					// This is the ODE joint connecting the two bodies.
					// Note that if the two bodies are connected by multiple
					// Joints, the behavior is undefined.
					theJoint = currentJoint;
				}
			}

			// Make sure the ODE joint was actually found.  This should
			// be guaranteed.
			if (theJoint == IntPtr.Zero)
				throw new PhysicsException("Some bug occured in XNA Physics API");

			// Now return the associated OPAL Joint.
			return GCHandle.FromIntPtr(Tao.Ode.Ode.dJointGetData(theJoint)).Target as Joint;
		}

		/// Special collision callback functor for volume collision
		/// checking.
		static unsafe internal void VolumeCollisionCallback(IntPtr data, IntPtr o0, IntPtr o1)
		{
			if (Tao.Ode.Ode.dGeomIsSpace(o0) != 0 || Tao.Ode.Ode.dGeomIsSpace(o1) != 0)
			{
				// Colliding a space with either a geom or another space.
				Tao.Ode.Ode.dSpaceCollide2(o0, o1, data, VolumeCollisionCallback);
			}
			else
			{
				// Get a pointer to the ODESimulator.
				OdeSimulator sim = GCHandle.FromIntPtr(data).Target as OdeSimulator;

				// Get pointers to the two geoms' GeomData structure.  Both
				// of these should always be non-NULL.
				/*GeomData geomData0 = (GeomData)Tao.Ode.Ode.dGeomGetData(o0);
				GeomData geomData1 = (GeomData)Tao.Ode.Ode.dGeomGetData(o1);*/
                // CANYONSHOOTER BEGIN
                dGeomID g0 = o0;
                dGeomID g1 = o1;
                GeomData geomData0 = GCHandle.FromIntPtr(Tao.Ode.Ode.dGeomGetData(g0)).Target as GeomData;
                GeomData geomData1 = GCHandle.FromIntPtr(Tao.Ode.Ode.dGeomGetData(g1)).Target as GeomData;
                /*
				GeomData geomData0 = GCHandle.FromIntPtr(o0).Target as GeomData;
				GeomData geomData1 = GCHandle.FromIntPtr(o1).Target as GeomData;
                */
                // CANYONSHOOTER END

				// Get pointers to the geoms' ShapeData structures.
				ShapeData shape0 = geomData0.Shape;
				ShapeData shape1 = geomData1.Shape;

				// Check if the two Solids' contact groups generate contacts
				// when they collide.
				bool makeContacts = sim.GroupsMakeContacts(shape0.ContactGroup, shape1.ContactGroup);
				if (!makeContacts)
				{
					return;
				}

				// Now actually test for collision between the two geoms.
				// This is a fairly expensive operation.
				//ODE.Joints.ContactGeom[] contactArray = new ODE.Joints.ContactGeom[1];
				Tao.Ode.Ode.dContactGeom[] contactArray = new Tao.Ode.Ode.dContactGeom[1];
				int numContacts = Tao.Ode.Ode.dCollide(o0, o1, 1, contactArray, sizeof(Tao.Ode.Ode.dContactGeom));

				if (0 == numContacts)
				{
					return;
				}
				else
				{
					// These two geoms must be intersecting.

					// Get pointers to the geoms' Solids.
					Solid solid0 = geomData0.Solid;
					Solid solid1 = geomData1.Solid;

					// Not sure at this point if we can know that o1 is the
					// volume object, so we'll just call this twice.  It
					// will automatically keep from adding the same Solid
					// multiple times by using its collision count.  Later,
					// the volume Solid will be removed from this list.
					sim.InternalAddCollidedSolid(solid0);
					sim.InternalAddCollidedSolid(solid1);
				}
			}
		}

		/// Collision callback functor for ray casting.  Stores results 
		/// in an unsorted array.
		static unsafe internal void RaycastCollisionCallback(IntPtr data, IntPtr o0, IntPtr o1)
		{
			if (Tao.Ode.Ode.dGeomIsSpace(o0) != 0 || Tao.Ode.Ode.dGeomIsSpace(o1) != 0)
			{
				// Colliding a space with either a geom or another space.
				Tao.Ode.Ode.dSpaceCollide2(o0, o1, data, RaycastCollisionCallback);
			}
			else
			{
				// Colliding two geoms.

				// Sometimes we get a case where the ray geom is passed in
				// as both objects, which is stupid.
				if (o0 == o1)
				{
					return;
				}

				// Get a pointer to the ODESimulator.
				OdeSimulator sim = GCHandle.FromIntPtr(data).Target as OdeSimulator;

				// Get pointers to the two geoms' GeomData structure.  One
				// of these (the one NOT belonging to the ray geom)
				// will always be non-NULL.
				/*GeomData geomData0 = (GeomData)Tao.Ode.Ode.dGeomGetData(o0);
				GeomData geomData1 = (GeomData)Tao.Ode.Ode.dGeomGetData(o1);*/
				GeomData geomData0 = GCHandle.FromIntPtr(o0).Target as GeomData;
				GeomData geomData1 = GCHandle.FromIntPtr(o1).Target as GeomData;

				// Find the contact group of the collided Solid.
				int geomContactGroup = Defaults.Shape.ContactGroup;
				if (geomData0 != null)
				{
					geomContactGroup = geomData0.Shape.ContactGroup;
				}
				else
				{
					geomContactGroup = geomData1.Shape.ContactGroup;
				}

				// Check if the two Solids' contact groups generate contacts
				// when they collide.
				bool makeContacts = sim.GroupsMakeContacts(geomContactGroup, sim.InternalGetRayContactGroup());
				if (!makeContacts)
				{
					return;
				}

				// Now actually test for collision between the two geoms.
				// This is a fairly expensive operation.
				//ODE.Joints.ContactGeom[] contactArray = new ODE.Joints.ContactGeom[Defaults.Ode.maxRaycastContacts];
				Tao.Ode.Ode.dContactGeom[] contactArray = new Tao.Ode.Ode.dContactGeom[Defaults.Ode.MaxRaycastContacts];
				int numContacts = Tao.Ode.Ode.dCollide(o0, o1, Defaults.Ode.MaxRaycastContacts, contactArray, sizeof(Tao.Ode.Ode.dContactGeom));

				if (0 == numContacts)
				{
					return;
				}
				else
				{
					// These two geoms must be intersecting.  We will store 
					// only the closest RaycastResult.
					int closest = 0;
					for (int i = 0; i < numContacts; i++)
					{
						if (contactArray[i].depth < contactArray[closest].depth)
						{
							closest = i;
						}
					}

					// Only one of the geoms will be part of a Solid we 
					// want to store; the other is the ray.
					Solid solid = null;
					if (geomData0 != null)
					{
						solid = geomData0.Solid;
					}
					else
					{
						solid = geomData1.Solid;
					}

					Vector3 intersection = FromOdeVector3(contactArray[closest].pos);
					Vector3 normal = FromOdeVector3(contactArray[closest].normal);

					sim.InternalAddRaycastResult(solid, intersection, normal, contactArray[closest].depth);
				}
			}
		}

		static internal Tao.Ode.Ode.dVector3 ToOdeVector3(Vector3 vector)
		{
			return new Tao.Ode.Ode.dVector3(vector.X, vector.Y, vector.Z);
		}

		static internal Vector3 FromOdeVector3(Tao.Ode.Ode.dVector3 vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}
	}
}
