using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
//using ODE;


namespace XnaDevRu.Physics.Ode
{
    /// <summary>
    /// The ODE implementation of the Solid class.  Each ODE geom's user 
    /// data pointer points to its corresponding ShapeData object.
    /// </summary>
    public class OdeSolid : Solid
    {
        #region fields
        /// The ODE body ID.  
        protected dBodyID bodyID;

        /// The ODE world ID that contains this ODESolid's ODE body, if this 
        /// Solid is dynamic.
        protected dWorldID worldID;

        /// The ODE space ID that contains this ODESolid's geoms.
        protected dSpaceID spaceID;

        /// An internal list of this ODESolid's geom data objects.  
        public List<GeomData> geomDataList = new List<GeomData>();

        /// Used internally by volume collision checking.  
        protected int collisionCount;

        /// True if this ODESolid is "placeable."  This is false only for 
        /// Solids containing planes Shapes.
        protected bool isPlaceable;

        /// True if the ODESolid has a non-symmetric inertia tensor.
        protected bool nonSymmetricInertia;

        /// Used to improve ODE's angular velocity calculations for objects 
        /// with non-symmetric inertia tensors.
        protected bool isFreelySpinning;

        /// Used to improve ODE's angular velocity calculations for objects 
        /// with non-symmetric inertia tensors.
        protected float prevAngVelMagSquared;

        #endregion

        public OdeSolid(dWorldID worldID, dSpaceID spaceID)
        {
            this.worldID = worldID;
            this.spaceID = spaceID;
            isPlaceable = true;
            collisionCount = 0;
            nonSymmetricInertia = false;
            isFreelySpinning = true;
            prevAngVelMagSquared = 0;

            data = new SolidData();

            if (!data.IsStatic)
            {
                // Create an ODE body with default ODE mass parameters (total
                // mass = 1).  This should have an initial mass of 0 until shapes
                // are added, but ODE won't allow a mass of 0.  This will be
                // adjusted appropriately when the first shape is added.

                bodyID = this.worldID.CreateBody();

                //mBodyID = new Body(mWorldID);
            }

            Init(data);
        }

		protected override void Dispose(bool disposing)
        {
			if (!Disposed && disposing)
			{
				DestroyGeoms();

				if (!data.IsStatic)
				{
					// This is a dynamic solid, so it has an ODE body that needs
					// to be destroyed.
					bodyID.Destroy();
				}
			}
		}

        public override void Init(SolidData data)
        {
            // The order of function calls here is important.

            // Destroy the old Shapes.
			//this.data.DestroyShapes(); //http://www.codeplex.com/WorkItem/View.aspx?ProjectName=xnadevru&WorkItemId=4301

			this.data = data;

            // Destroy the old ODE geoms.
            DestroyGeoms();

            // Set whether the Solid is static.
            Static = data.IsStatic;

            // Set the new transform.
            Transform = data.Transform;

            // Add the new Shapes.
            for (int i = 0; i < data.NumShapes; ++i)
            {
                AddShape(data.GetShapeData(i));
            }

            // Set whether the Solid is sleeping.
            Sleeping = data.Sleeping;

            // Set the Solid's sleepiness level.
            Sleepiness = data.Sleepiness;

            // Set whether the Solid is enabled.
            Enabled = data.Enabled;

            // Set damping levels.
            LinearDamping = data.LinearDamping;
            AngularDamping = data.AngularDamping;

            // Set velocities.
            GlobalLinearVel = data.GlobalLinearVel;
            GlobalAngularVel = data.GlobalAngularVel;

            // Set the Solid's name.
            Name = data.Name;

        }

        /// <summary>
        /// Destroys all of this Solid's ODE geoms.
        /// </summary>
        private void DestroyGeoms()
        {
            for (int i = 0; i < geomDataList.Count; ++i)
            {
				if (geomDataList[i].TransformID != null)
					geomDataList[i].TransformID.Destroy();


                if (geomDataList[i].TrimeshDataID != null)
                {
                    // This geom uses a trimesh.  Destroy it.  (This does NOT
                    // touch the user's mesh data, just internal ODE trimesh
                    // data.)
                    geomDataList[i].TrimeshDataID.Destroy();
                }

                // Destroy the ODE geom.

                if (geomDataList[i].GeomID != null)
                {
                    geomDataList[i].GeomID.Destroy();
                }

            }
            geomDataList.Clear();
        }

        public override void ClearShapes()
        {
            DestroyGeoms();
            ResetAABB();
            data.DestroyShapes();
        }


        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                bool e = value;

                if (e)
                {
                    // If this Solid is dynamic, enable the ODE body.
                    if (!data.IsStatic)
                    {
                        //				dBodyEnable( mBodyID );
                        bodyID.Enabled = true;
                    }

                    // Enable the ODE geoms.

                    foreach (GeomData iter in geomDataList)
                        iter.GeomID.Enabled = true;
                }
                else // Disable the Solid.
                {
                    // If this Solid is dynamic, disable the ODE body.
                    if (!data.IsStatic)
                    {
                        //				dBodyDisable( mBodyID );
                        bodyID.Enabled = false;
                    }

                    // Disable the ODE geoms.
                    foreach (GeomData iter in geomDataList)
                        iter.GeomID.Enabled = false;
                }
            }
        }

        public override bool Static
        {
            get
            {
                return base.Static;
            }
            set
            {
                bool s = value;

                if (true == data.IsStatic)
                {
                    if (false == s)
                    {
                        // Make this object dynamic.
                        data.IsStatic = false;

                        // Create a new body.
                        bodyID = worldID.CreateBody();

                        // Set the ODE sleepiness params using the stored Solid
                        // sleepiness param.
                        this.Sleepiness = data.Sleepiness;

                        // Set the position of the new body.
                        //Matrix R = new Matrix(mData.transform[ 0 ], mData.transform[ 4 ], mData.transform[ 8 ], 0, 
                        //     mData.transform[ 1 ], mData.transform[ 5 ], mData.transform[ 9 ], 0,
                        //     mData.transform[ 2 ], mData.transform[ 6 ], mData.transform[ 10 ], 0};

                        Tao.Ode.Ode.dMatrix3 R = new Tao.Ode.Ode.dMatrix3();
                        R = data.Transform;

						//R.M00 = mData.Transform.M11;
						//R.M01 = mData.Transform.M21;
						//R.M02 = mData.Transform.M31;
						//R.M03 = 0;
						//R.M10 = mData.Transform.M12;
						//R.M11 = mData.Transform.M22;
						//R.M12 = mData.Transform.M32;
						//R.M13 = 0;
						//R.M20 = mData.Transform.M13;
						//R.M21 = mData.Transform.M23;
						//R.M22 = mData.Transform.M33;
						//R.M23 = 0;

                        Tao.Ode.Ode.dBodySetRotation(bodyID, R);
                        //mBodyID.Rotation = R;
                        Tao.Ode.Ode.dBodySetPosition(bodyID, data.Transform.Translation);


                        // Loop over the geoms.

                        for (int i = 0; i < geomDataList.Count; i++)
                        {
                            GeomData iter = geomDataList[i];
                            Tao.Ode.Ode.dMass newMass = new Tao.Ode.Ode.dMass();

                            // Get a pointer to this geom's ShapeData.
                            ShapeData shapeData = iter.Shape;

                            // Setup mass.
                            switch (shapeData.Type)
                            {
                                case ShapeType.Box:
                                    {
                                        BoxShapeData boxData = (BoxShapeData)shapeData;

                                        Tao.Ode.Ode.dMassSetBox(ref newMass,
                                                     shapeData.Material.Density,
                                                     boxData.Dimensions.X,
                                                     boxData.Dimensions.Y,
                                                     boxData.Dimensions.Z);

                                        //newMass = Mass.Box(shapeData.material.Density, boxData.Dimensions.X, boxData.Dimensions.Y, boxData.Dimensions.Z);
                                        break;
                                    }
                                case ShapeType.Sphere:
                                    {
                                        SphereShapeData sphereData = (SphereShapeData)shapeData;
                                        Tao.Ode.Ode.dMassSetSphere(ref newMass, shapeData.Material.Density, sphereData.Radius);
                                        break;
                                    }
                                case ShapeType.Capsule:
                                    {
                                        CapsuleShapeData capsuleData = (CapsuleShapeData)shapeData;
                                        Tao.Ode.Ode.dMassSetCapsule(ref newMass, shapeData.Material.Density, 3,
                                                                capsuleData.Radius,
                                                                capsuleData.Length);
                                        break;
                                    }
                                case ShapeType.Plane:
                                    // Planes have no mass.
                                    break;
                                //case RAY_SHAPE:
                                //	// Rays have no mass.
                                //	break;
                                case ShapeType.Mesh:
                                    // This is a simple way to set the mass of an
                                    // arbitrary mesh.  Ideally we would compute
                                    // the exact volume and intertia tensor.
                                    // Instead we just use a box inertia tensor
                                    // from its axis-aligned bounding box.
                                    //float[] aabb = new float[6];
                                    Tao.Ode.Ode.Aabb aabb = new Tao.Ode.Ode.Aabb();
                                    Tao.Ode.Ode.dGeomGetAABB( iter.GeomID, aabb );
                                    Tao.Ode.Ode.dMassSetBox(ref newMass, shapeData.Material.Density,
										aabb.maxx - aabb.minx, aabb.maxy - aabb.miny, aabb.maxz - aabb.minz);
                                    //aabb[ 1 ] - aabb[ 0 ], aabb[ 3 ] - aabb[ 2 ],
                                    //aabb[ 5 ] - aabb[ 4 ] );
                                    break;
                                default:
                                    throw new InvalidOperationException("OdeSolid.Static");
                            }

                            // Setup the new mass.
                            if (i == 0)
                                SetMass(newMass, shapeData.Offset);
                            else
                                AddMass(newMass, shapeData.Offset);

                            // Add each geom to the new body.
                            if (iter.TransformID == null)
                            {
                                // No geom transform.
                                //						dGeomSetBody( ( *iter ) ->geomID, mBodyID );
                                iter.GeomID.RigidBody = bodyID;
                            }
                            else
                            {
                                // Use geom transform.
                                //dGeomSetBody( ( *iter ) ->transformID, mBodyID );
                                iter.TransformID.RigidBody = bodyID;
                            }
                        }

                        MoveToSpace();
                    }
                    else
                    {
                        // do nothing
                    }
                }
                else // this object is not static
                {
                    if (true == s)
                    {
                        // make this object static
                        data.IsStatic = true;

                        // destroy the body
                        bodyID.Destroy();

                        MoveToSpace();
                    }
                    else
                    {
                        // do nothing
                    }

                }
            }
        }

        public override bool Sleeping
        {
            get
            {
                if (data.IsStatic)
                {
                    return true;
                }

                return !bodyID.Enabled;
            }
            set
            {
                if (data.IsStatic)
                {
                    return;
                }

                // Note: mData.sleeping gets updated in Solid::getData.

                bodyID.Enabled = !value;

            }
        }

        public override SpaceBase Space
        {
            set
            {
                // update solid's space which will be used for future shapes
                spaceID = ((OdeSpace)value).InternalGetSpaceID();

                MoveToSpace();
            }
        }

        public override float Sleepiness
        {
            get
            {
                return base.Sleepiness;
            }
            set
            {
                float s = value;
                base.Sleepiness = s;

                /// If this Solid has no ODE body, just return.  The sleepiness
                /// level will still be saved and applied if this Solid ever
                /// becomes dynamic.
                if (data.IsStatic)
                {
                    return;
                }

                if (0 == s)
                {
                    // No sleeping at all for the Solid.
                    bodyID.AutoDisable = false;
                }
                else
                {
                    // Enable sleeping for the Solid..
                    bodyID.AutoDisable = true;
                }

                // As value goes from 0.0 to 1.0:
                // AutoDisableLinearThreshold goes from min to max,
                // AutoDisableAngularThreshold goes from min to max,
                // AutoDisableSteps goes from max to min,
                // AutoDisableTime goes from max to min.

                float range = Defaults.Ode.AutoDisableLinearMax -
                             Defaults.Ode.AutoDisableLinearMin;

                bodyID.AutoDisableLinearThreshold = s * range + Defaults.Ode.AutoDisableLinearMin;

                range = Defaults.Ode.AutoDisableAngularMax -
                        Defaults.Ode.AutoDisableAngularMin;

                bodyID.AutoDisableAngularThreshold = s * range + Defaults.Ode.AutoDisableAngularMin;

                range = (float)(Defaults.Ode.AutoDisableStepsMax - Defaults.Ode.AutoDisableStepsMin);

                bodyID.AutoDisableSteps = (int)((float)Defaults.Ode.AutoDisableStepsMax - s * range);

                range = Defaults.Ode.AutoDisableTimeMax - Defaults.Ode.AutoDisableTimeMin;

                bodyID.AutoDisableTime = Defaults.Ode.AutoDisableTimeMax - s * range;
            }
        }

        public override void AddShape(ShapeData data)
        {
            if (data.Material.Density < 0)
                return;

            dGeomID newGeomID = null;
            dGeomID newTransformID = null;
            dTriMeshDataID newTrimeshDataID = null;
            dSpaceID tempSpaceID = null;
            Tao.Ode.Ode.dMass newMass = new Tao.Ode.Ode.dMass();

            if (new Matrix() == data.Offset)
            {
                // No offset transform.
                tempSpaceID = spaceID;
                newTransformID = null;
            }
            else
            {
                // Use ODE's geom transform object.
                tempSpaceID = IntPtr.Zero;
                newTransformID = spaceID.CreateGeomTransform();
            }

            // Allocate a new GeomData object.
            GeomData newGeomData = new GeomData();

            switch (data.Type)
            {
                case ShapeType.Box:
                    {
                        BoxShapeData boxData = data as BoxShapeData;

                        newGeomID = tempSpaceID.CreateBoxGeom(boxData.Dimensions.X,
                                                    boxData.Dimensions.Y,
                                                    boxData.Dimensions.Z);

                        Tao.Ode.Ode.dMassSetBox(ref newMass, data.Material.Density,
                                        boxData.Dimensions.X,
                                        boxData.Dimensions.Y,
                                        boxData.Dimensions.Z);
                        break;
                    }
                case ShapeType.Sphere:
                    {
                        SphereShapeData sphereData = data as SphereShapeData;

                        newGeomID = tempSpaceID.CreateSphereGeom(sphereData.Radius);

                        Tao.Ode.Ode.dMassSetSphere(ref newMass, data.Material.Density, sphereData.Radius);
                        break;
                    }
                case ShapeType.Capsule:
                    {
                        CapsuleShapeData capsuleData = data as CapsuleShapeData;

                        newGeomID = tempSpaceID.CreateCylinderGeom(capsuleData.Radius, capsuleData.Length);

                        // The "direction" parameter orients the mass along one of the
                        // body's local axes; x=1, y=2, z=3.  This axis MUST
                        // correspond with the axis along which the capsule is
                        // initially aligned, which is the capsule's local Z axis.
                        /*Tao.Ode.Ode.dMassSetCylinder(ref newMass, data.Material.Density,
                                                3, capsuleData.Radius, capsuleData.Length);*/
						Tao.Ode.Ode.dMassSetCapsule(ref newMass, data.Material.Density, 3, 
							capsuleData.Radius, capsuleData.Length);
                        break;
                    }
                case ShapeType.Plane:
                    {
                        PlaneShapeData planeData = data as PlaneShapeData;
                        if (!this.data.IsStatic)
                        {
                            //OPAL_LOGGER( "warning" ) << "opal::ODESolid::addPlane: " <<
                            //"Plane Shape added to a non-static Solid.  " <<
                            //"The Solid will be made static." << std::endl;

                            // ODE planes can't have bodies, so make it static.
                            this.Static = true;
                        }

                        // TODO: make this fail gracefully and print warning: plane
                        // offset transform ignored.
                        if (newTransformID != null)
                            break;

                        // ODE planes must have their normal vector (abc) normalized.
                        Vector3 normal = new Vector3(planeData.Abcd[0], planeData.Abcd[1], planeData.Abcd[2]);
                        normal.Normalize();

                        newGeomID = tempSpaceID.CreatePlaneGeom(normal.X, normal.Y, normal.Z, planeData.Abcd[3]);

                        // Note: ODE planes cannot have mass, but this is already
                        // handled since static Solids ignore mass.

                        // Solids with planes are the only non-"placeable" Solids.
                        isPlaceable = false;
                        break;
                    }
                //case RAY_SHAPE:
                //{
                //	RayShapeData& rayData = (RayShapeData&)data;
                //	newGeomID = dCreateRay(spaceID,
                //		(dReal)rayData.ray.getLength());
                //	Point3r origin = rayData.ray.getOrigin();
                //	Vec3r dir = rayData.ray.getDir();
                //	dGeomRaySet(newGeomID, (dReal)origin[0], (dReal)origin[1],
                //		(dReal)origin[2], (dReal)dir[0], (dReal)dir[1],
                //		(dReal)dir[2]);
                //	// Note: rays don't have mass.
                //	break;
                //}
                case ShapeType.Mesh:
                    {
                        MeshShapeData meshData = data as MeshShapeData;

                        // Setup trimesh data pointer.  It is critical that the
                        // size of OPAL reals at this point match the size of ODE's
                        // dReals.
                        newTrimeshDataID = dTriMeshDataID.Create();

                        // Old way... This is problematic because ODE interprets
                        // the vertex array as an array of dVector3s which each
                        // have 4 elements.
                        //dGeomTriMeshDataBuildSimple(newTrimeshDataID,
                        //	(dReal*)meshData.vertexArray, meshData.numVertices,
                        //	(int*)meshData.triangleArray, 3 * meshData.numTriangles);

                        //#ifdef dSINGLE
                        newTrimeshDataID.BuildSingle(meshData.VertexArray, 3 * sizeof(float),
                                                    meshData.NumVertices,
                                                    meshData.TriangleArray,
                                                    3 * meshData.NumTriangles,
                                                    3 * sizeof(int));
                        //#else
                        //                    dGeomTriMeshDataBuildDouble( newTrimeshDataID,
                        //                                                 ( void* ) meshData.vertexArray, 3 * sizeof( real ),
                        //                                                 meshData.numVertices, ( void* ) meshData.triangleArray,
                        //                                                 3 * meshData.numTriangles, 3 * sizeof( unsigned int ) );
                        //#endif

                        newGeomID = tempSpaceID.CreateTriMeshGeom(newTrimeshDataID);

                        // This is a simple way to set the mass of an arbitrary
                        // mesh.  Ideally we would compute the exact volume and
                        // intertia tensor.  Instead we just use a box
                        // inertia tensor from its axis-aligned bounding box.
                        float[] aabb = newGeomID.AABB;

                        //					dGeomGetAABB( newGeomID, aabb );
                        Tao.Ode.Ode.dMassSetBox(ref newMass, data.Material.Density,
                                        aabb[1] - aabb[0], aabb[3] - aabb[2],
                                        aabb[5] - aabb[4]);
                        break;
                    }
                default:
                    throw new InvalidOperationException("OdeSolid.AddShape");
            }

            // This will do nothing if this is a static Solid.
            AddMass(newMass, data.Offset);

            // Store new Shape.
			this.data.AddShape(data);

            // Update the Solid's local AABB.  The new shape's local AABB
            // must be transformed using the shape's offset from the Solid.
            BoundingBox shapeAABB = data.LocalAABB;
            //data.LocalAABBgetLocalAABB( shapeAABB );
            Vector3 minExtents = Vector3.Transform(shapeAABB.Min, data.Offset);
            Vector3 maxExtents = Vector3.Transform(shapeAABB.Max, data.Offset);

            shapeAABB.Min = minExtents;
            shapeAABB.Max = maxExtents;

            AddToLocalAABB(shapeAABB);

            if (newGeomData == null)
                return;

            // Setup GeomData.
            newGeomData.Solid = this;
			newGeomData.Shape = this.data.GetShapeData(this.data.NumShapes - 1);
            newGeomData.GeomID = newGeomID;
            newGeomData.TransformID = newTransformID;
            newGeomData.SpaceID = spaceID;
            newGeomData.TrimeshDataID = newTrimeshDataID;

            // Setup the geom.
            SetupNewGeom(newGeomData);

        }

        public override Vector3 LocalLinearVel
        {
            get
            {
                if (!data.IsStatic)
                {
                    Vector3 vel = bodyID.LinearVel;

                    // Convert the vector from global to local coordinates.
                    return bodyID.BodyVectorFromWorld( vel);
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            set
            {
                if (!data.IsStatic)
                {
                    Vector3 worldVel = bodyID.BodyVectorToWorld(value);
                    bodyID.LinearVel = worldVel;

                    // Invalidate the "freely-spinning" parameter.
                    InternalSetFreelySpinning(false);
                }
            }
        }

        public override Vector3 GetLocalLinearVelAtLocalPos(Vector3 p)
        {
            if (!data.IsStatic)
            {
                // First find the global velocity at the given point.

                Vector3 vel = bodyID.GetRelPointVel(p);

                // Now convert the velocity from global to local coordinates.
                vel = bodyID.BodyVectorFromWorld(vel);

                return vel;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public override Vector3 GetGlobalLinearVelAtLocalPos(Vector3 p)
        {
            if (!data.IsStatic)
            {
                // First find the global velocity at the given point.
                Vector3 vel = bodyID.GetRelPointVel(p);

                return vel;
            }
            else
            {
                return Vector3.Zero;
            }
        }


        /// <summary>
        /// Gets/Sets the linear velocity of a body
        ///
        /// The vector is valid until any changes are made to the rigid body system structure.
        /// </summary>
        public override Vector3 GlobalLinearVel
        {
            get
            {
                if (!data.IsStatic)
                {
                    return bodyID.LinearVel;
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            set
            {
                if (!data.IsStatic)
                {
                    bodyID.LinearVel = value;

                    // Invalidate the "freely-spinning" parameter.
                    InternalSetFreelySpinning(false);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the angular (in degrees) velocity of a body
        /// </summary>
        public override Vector3 GlobalAngularVel
        {
            get
            {
                if (!data.IsStatic)
                {
                    Vector3 vel = bodyID.AngularVel;
                    return new Vector3(MathHelper.ToDegrees(vel.X), MathHelper.ToDegrees(vel.Y), MathHelper.ToDegrees(vel.Z));
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            set
            {
                if (!data.IsStatic)
                {
                    Vector3 velRad = new Vector3(MathHelper.ToRadians(value.X), MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.Z));
                    bodyID.AngularVel = velRad;

                    // Invalidate the "freely-spinning" parameter.
                    InternalSetFreelySpinning(false);
                }
            }
        }

        public override Vector3 LocalAngularVel
        {
            get
            {
                if (!data.IsStatic)
                {
                    Vector3 vel = bodyID.AngularVel;

                    // Convert the vector from global to local coordinates.
                    Vector3 localVel = bodyID.BodyVectorFromWorld( vel);
                    return new Vector3(MathHelper.ToDegrees(localVel.X), MathHelper.ToDegrees(localVel.Y), MathHelper.ToDegrees(localVel.Z));
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            set
            {
                if (!data.IsStatic)
                {
                    Vector3 worldVel = bodyID.BodyVectorToWorld(value);

                    Vector3 velRad = new Vector3(MathHelper.ToRadians(worldVel.X), MathHelper.ToRadians(worldVel.Y), MathHelper.ToRadians(worldVel.Z));
                    bodyID.AngularVel = velRad;

                    // Invalidate the "freely-spinning" parameter.
                    InternalSetFreelySpinning(false);
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [influenced by gravity].
        /// </summary>
        /// <value><c>true</c> if [influenced by gravity]; otherwise, <c>false</c>.</value>
        public override bool InfluencedByGravity
        {
            get
            {
                return bodyID.InfluencedByGravity;
            }
            set
            {
                bodyID.InfluencedByGravity = value;
            }
        }


        /// <summary>
        /// Gets the mass of this solid
        /// </summary>
        public override float Mass
        {
            get 
            {
                if (data.IsStatic)
                {
                    return 0;
                }
                else
                {
                    Tao.Ode.Ode.dMass mass = bodyID.Mass;
                    return mass.mass;
                }
            }
        }

        public override void SetMass(Mass mass, Matrix offset)
        {
            if (data.IsStatic)
            {
                return;
            }

            		// First rotate and translate the new mass.
            Tao.Ode.Ode.dMatrix3 R = offset;

            Tao.Ode.Ode.dMass m = new Tao.Ode.Ode.dMass();
            m.c = new Tao.Ode.Ode.dVector4(mass.Center.X, mass.Center.Y, mass.Center.Z, 0);
            m.I = mass.Inertia;
            m.mass = mass.MassValue;

            Tao.Ode.Ode.dMassRotate(ref m, R);
            Tao.Ode.Ode.dMassTranslate(ref m, offset.M41, offset.M42, offset.M43);

            bodyID.Mass = m;

            // Update this since the mass changed.
            m = bodyID.Mass;
            nonSymmetricInertia = IsInertiaNonSymmetric(m);
        }

        public override void TranslateMass(Vector3 offset)
        {
            if (!data.IsStatic)
            {
                Tao.Ode.Ode.dMass m = bodyID.Mass;

                Tao.Ode.Ode.dMassTranslate(ref m, offset.X, offset.Y, offset.Z);
                bodyID.Mass = m;

                // Update this since the mass changed.
                nonSymmetricInertia = IsInertiaNonSymmetric(m);
            }
        }

        public override void ZeroForces()
        {
            if (!data.IsStatic)
            {
                bodyID.Force = Vector3.Zero;
                bodyID.Torque = Vector3.Zero;

                forceList.Clear();

            }
        }

        public override Matrix InertiaTensor
        {
            get 
            {
                Matrix m = Matrix.Identity;

                if (data.IsStatic)
                {
                    return m;
                }
                else
                {
                    Tao.Ode.Ode.dMass mass = bodyID.Mass;

                    m = mass.I;

                    //m.M11 = mass.I.M00;
                    //m.M12 = mass.I.M01;
                    //m.M13 = mass.I.M02;
                    //m.M14 = 0;
                    //m.M21 = mass.I.M10;
                    //m.M22 = mass.I.M11;
                    //m.M23 = mass.I.M12;
                    //m.M24 = 0;
                    //m.M31 = mass.I.M20;
                    //m.M32 = mass.I.M21;
                    //m.M33 = mass.I.M22;
                    m.M34 = 0;
                    m.M41 = 0;
                    m.M42 = 0;
                    m.M43 = 0;
                    m.M44 = 1;

                    return m;
                }
            }
        }

        public override void InternalUpdateOPALTransform()
        {
            Matrix R = bodyID.Rotation;
            Vector3 P = bodyID.Position;

            data.Transform = R;
            data.Transform.Translation = P;

        }


        public override void InternalUpdateEngineTransform()
        {
            Matrix R = data.Transform;
            R.Translation = Vector3.Zero;

            if (!data.IsStatic)
            {
                bodyID.Rotation = R;
                bodyID.Position = data.Transform.Translation;
            }
            else if (isPlaceable)
            {

                foreach (GeomData iter in geomDataList)
                {
                    GeomData geomData = iter;

                    if (geomData.TransformID == null)
                    {
                        // No geom transform.
                        geomData.GeomID.Rotation = R;
                        geomData.GeomID.Position = data.Transform.Translation;
                    }
                    else
                    {
                        // Using geom transform.
                        geomData.TransformID.Rotation = R;
                        geomData.TransformID.Position = data.Transform.Translation;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the ODE body ID of this ODESolid.  
        /// </summary>
        /// <returns></returns>
        protected internal dBodyID InternalGetBodyID()
        {
            return bodyID;
        }


        /// <summary>
        /// Used internally by volume collision checking.  
        /// </summary>
        /// <returns></returns>
        protected internal int CollisionCount
        {
            get { return collisionCount; }
            set { collisionCount = value; }
        }

        /// <summary>
        /// Returns all geom data objects in this ODESolid.  
        /// </summary>
        /// <returns></returns>
        protected internal List<GeomData> InternalGetGeomDataList()
        {
            return geomDataList;
        }


        /// <summary>
        /// Fix angular velocities for freely-spinning bodies that have 
        /// gained angular velocity through explicit integrator inaccuracy.  
        /// This usually only happens for long, thin objects.
        /// </summary>
        protected internal void InternalDoAngularVelFix()
        {
            if (nonSymmetricInertia)
            {
                Vector3 vel = GlobalAngularVel;
                float currentAngVelMagSquared = vel.LengthSquared();

                if (isFreelySpinning)
                {
                    // If the current angular velocity magnitude is greater than
                    // that of the previous step, scale it by that of the previous
                    // step; otherwise, update the previous value to that of the
                    // current step.  This ensures that angular velocity never
                    // increases for freely-spinning objects.

                    if (currentAngVelMagSquared > prevAngVelMagSquared)
                    {
                        float currentAngVelMag = (float)Math.Sqrt(currentAngVelMagSquared);
                        vel = vel / currentAngVelMag;

                        // Vel is now a unit vector.  Next, scale this vector
                        // by the previous angular velocity magnitude.
                        float prevAngVelMag = (float)Math.Sqrt(prevAngVelMagSquared);
                        GlobalAngularVel = vel * prevAngVelMag;
                    }
                }

                prevAngVelMagSquared = currentAngVelMagSquared;
            }

            // Reset the "freely-spinning" parameter for the next time step.
            InternalSetFreelySpinning(true);
        }

        /// <summary>
        /// Sets whether this object is freely-spinning (i.e. no recent 
        /// physical contacts, no user-generated forces, no user-defined 
        /// velocity changes).
        /// </summary>
        /// <param name="fs"></param>
        protected internal void InternalSetFreelySpinning(bool fs)
        {
            isFreelySpinning = fs;
        }

        /// <summary>
        /// Adds a new GeomData object to the internal list and sets up the ODE geom.
        /// </summary>
        /// <param name="newGeom"></param>
        protected void SetupNewGeom(GeomData newGeom)
        {
            if (!data.IsStatic)
            {
                if (newGeom.TransformID == null)
                {
                    // No geom transform.
                    newGeom.GeomID.RigidBody = bodyID;
                }
                else
                {
                    // Use geom transform.
                    newGeom.TransformID.RigidBody = bodyID;
                }
            }

            if (newGeom.TransformID != null)
            {
                // Setup geom transform.
                Tao.Ode.Ode.dGeomTransformSetGeom(newGeom.TransformID, newGeom.GeomID);

                Matrix R = newGeom.Shape.Offset;
                R.Translation = Vector3.Zero;

                if (newGeom.GeomID != null)
                {
                    newGeom.GeomID.Rotation = R;
                    newGeom.GeomID.Position = newGeom.Shape.Offset.Translation;
                }
            }

            // Set the GeomData reference for later use (e.g. in collision
            // handling).
            if (newGeom.TransformID == null)
            {
                // No geom transform.
                Tao.Ode.Ode.dGeomSetData(newGeom.GeomID, GCHandle.ToIntPtr(GCHandle.Alloc(newGeom)));
            }
            else
            {
                // Using geom transform.
                Tao.Ode.Ode.dGeomSetData(newGeom.TransformID, GCHandle.ToIntPtr(GCHandle.Alloc(newGeom)));
            }

            // Store the GeomData pointer.
            geomDataList.Add(newGeom);

            // Make sure the initial transform is setup; this needs to come after
            // the geom data has been added.
            if (data.IsStatic && isPlaceable)
            {
                this.Transform = data.Transform;
            }
        }

        /// <summary>
        /// move to geometry to current space
        /// </summary>
        protected void MoveToSpace()
        {
            // remove all current shapes from their spaces and add them to the
            // new one
            //std::vector<GeomData*>::iterator iter;
            //for ( iter = mGeomDataList.begin(); iter != mGeomDataList.end(); ++iter )
            for (int i = 0; i < geomDataList.Count; i++)
            {
                if (geomDataList[i].TransformID != null)
                {
                    // This geom uses a transform, so apply the new space only
                    // to the transform geom.
                    //				dSpaceRemove( ( *iter ) ->spaceID, ( *iter ) ->transformID );
                    geomDataList[i].SpaceID.Remove(geomDataList[i].TransformID);
                    //				dSpaceAdd( mSpaceID, ( *iter ) ->transformID );
                    spaceID.Add(geomDataList[i].TransformID);
                }
                else
                {
                    // Normal geom with no transform.
                    //				dSpaceRemove( ( *iter ) ->spaceID, ( *iter ) ->geomID );
                    geomDataList[i].SpaceID.Remove(geomDataList[i].GeomID);
                    //				dSpaceAdd( mSpaceID, ( *iter ) ->geomID );
                    spaceID.Add(geomDataList[i].GeomID);
                }

                geomDataList[i].SpaceID = spaceID;
            }
        }

        /// <summary>
        /// Adds the given mass to this Solid's existing mass.  The offset is 
        /// relative to the Solid's center.  This must be called before 
        /// setupNewGeom is called.
        /// </summary>
        /// <param name="newMass"></param>
        /// <param name="offset"></param>
        public void AddMass(Tao.Ode.Ode.dMass newMass, Matrix offset)
        {
            if (data.IsStatic)
            {
                return;
            }

            // First rotate and translate the new mass.
            Matrix R = offset;
            R.Translation = Vector3.Zero;

            Tao.Ode.Ode.dMassRotate(ref newMass, R);
            Vector3 t = offset.Translation;
            Tao.Ode.Ode.dMassTranslate(ref newMass, t.X, t.Y, t.Z);

            // If this mass is for the first Shape, just set the Solid's mass
            // equal to the new one.  ODE bodies start with an initial mass
            // already setup, but we want to ignore that, not add to it.
            if (geomDataList.Count == 0)
            {
                bodyID.Mass = newMass;
            }
            else
            {
                // Add new mass to the Solid's existing mass.  First get the
                // existing mass struct from ODE.
                Tao.Ode.Ode.dMass totalMass = bodyID.Mass;

                //Tao.Ode.Ode.dMassAdd(ref totalMass, newMass); // (KleMiX) strange bug
				float denom = 1 / (totalMass.mass + newMass.mass);
				totalMass.c.X = (totalMass.c.X * totalMass.mass + newMass.c.X * newMass.mass) * denom;
				totalMass.c.Y = (totalMass.c.Y * totalMass.mass + newMass.c.Y * newMass.mass) * denom;
				totalMass.c.Z = (totalMass.c.Z * totalMass.mass + newMass.c.Z * newMass.mass) * denom;
				totalMass.mass += newMass.mass;
				for (int i = 0; i < 12; i++)
					totalMass.I[i] += newMass.I[i];

                bodyID.Mass = totalMass;
            }

            // Update this since the mass changed.
            Tao.Ode.Ode.dMass m = bodyID.Mass;
            nonSymmetricInertia = IsInertiaNonSymmetric(m);
        }



        /// <summary>
        /// Returns true if the given mass has a non-symmetric inertia tensor.
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        protected bool IsInertiaNonSymmetric(Tao.Ode.Ode.dMass mass)
        {
            if (!MathUtil.AreEqual(mass.I.M00, mass.I.M11)
                || !MathUtil.AreEqual(mass.I.M11, mass.I.M22))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override void ApplyForce(Force f)
        {
            switch (f.Type)
            {
                case ForceType.LocalForce:
                    Tao.Ode.Ode.dBodyAddRelForce(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z);
                    break;
                case ForceType.GlobalForce:
                    Tao.Ode.Ode.dBodyAddForce(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z);
                    break;
                case ForceType.LocalTorque:
                    Tao.Ode.Ode.dBodyAddRelTorque(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z);
                    break;
                case ForceType.GlobalTorque:
                    Tao.Ode.Ode.dBodyAddTorque(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z);
                    break;
                case ForceType.LocalForceAtLocalPosition:
                    Tao.Ode.Ode.dBodyAddRelForceAtRelPos(bodyID,
                                                f.Direction.X, f.Direction.Y, f.Direction.Z,
                                                f.Position.X, f.Position.Y, f.Position.Z);
                    break;
                case ForceType.LocalForceAtGlobalPosition:
                    Tao.Ode.Ode.dBodyAddRelForceAtPos(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z,
                                           f.Position.X, f.Position.Y, f.Position.Z);
                    break;
                case ForceType.GlobalForceAtLocalPosition:
                    Tao.Ode.Ode.dBodyAddForceAtRelPos(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z,
                                           f.Position.X, f.Position.Y, f.Position.Z);
                    break;
                case ForceType.GlobalForceAtGlobalPosition:
                    Tao.Ode.Ode.dBodyAddForceAtPos(bodyID, f.Direction.X, f.Direction.Y, f.Direction.Z,
                                        f.Position.X, f.Position.Y, f.Position.Z);
                    break;
            }

            // Invalidate the "freely-spinning" parameter.
            InternalSetFreelySpinning(false);
        }


    }
}
