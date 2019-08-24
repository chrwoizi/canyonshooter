namespace XnaDevRu.Physics.Ode
{
    /// <summary>
    /// The ODE implementation of the SpaceBase class.
    /// </summary>
	public class OdeSpace : SpaceBase
	{
		// The ODE space ID.
		protected dSpaceID spaceID;
		// The ODE space ID of this ODESpace's parent ODESpace.
		protected dSpaceID parentSpaceID;

		public OdeSpace()
		{
            bool useSpatialTrees = true;

            // Create the Space without adding it to another Space.
            if (!useSpatialTrees)
                spaceID = dSpaceID.Create();
            else
            {
                spaceID = dSpaceID.HashSpaceCreate();
                spaceID.SetLevels(3, 9);
            }
            parentSpaceID = null;
        }

		public OdeSpace(dSpaceID space)
		{
            spaceID = space;
            parentSpaceID = null;
        }

		~OdeSpace()
		{
		}

        /// <summary>
        /// Returns the ODE space ID.
        /// </summary>
        /// <returns></returns>
		protected internal virtual dSpaceID InternalGetSpaceID()
		{
            return spaceID;
		}

		public override SpaceBase ParentSpace
		{
			set 
            {
                dSpaceID tempSpaceID = ((OdeSpace)value).InternalGetSpaceID();

                // First remove this Space from its current parent Space, if one
                // exists.
                if (parentSpaceID != null)
                {
                    //mParentSpaceID.Remove(
                    Tao.Ode.Ode.dSpaceRemove(parentSpaceID, spaceID.Ptr);
                }

                // Now add this Space into the new Space.
                Tao.Ode.Ode.dSpaceAdd(tempSpaceID, spaceID.Ptr);

                parentSpaceID = tempSpaceID;
            }
		}
	}
}
