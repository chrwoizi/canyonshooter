namespace XnaDevRu.Physics
{
    /// <summary>
    /// Spaces are used for space-partitioning to speed up collision 
    /// detection.  If a particular application contains a group of 
    /// Solids that will always be in close proximity, it can help to 
    /// create a Space and add those Solids to the Space.
    /// </summary>
    public abstract class SpaceBase
    {
        public SpaceBase() { }

        /// <summary>
        /// Removes the Space from its current parent Space and adds it to 
        /// the new Space.
        /// </summary>
        public abstract SpaceBase ParentSpace { set; }
        
    }
}
