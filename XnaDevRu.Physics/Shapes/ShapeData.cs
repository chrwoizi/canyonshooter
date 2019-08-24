using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a Shape.  Note that there is no other
    /// class associated with Shapes besides this one.
    /// </summary>
    public abstract class ShapeData
    {

        #region fields

        /// <summary>
        /// The offset transform from a Solid's transform.
        /// </summary>
        public Matrix Offset;

		private Material material;
        private int contactGroup;

		/// The Shape type.
		protected ShapeType shapeType; 

    	#endregion

        #region Properties

        /// <summary>
        /// The Shape's material.
        /// </summary>
        public Material Material {
            get {
                return material;
            }
            set {
                material = value;
            }
        }

        /// <summary>
        /// The Shape's contact group.  If this exceeds 31, bad things
        /// might happen since there are only 32 groups.
        /// </summary>
        public int ContactGroup {
            get {
                return contactGroup;
            }
            set {
                contactGroup = value;
            }
        }

        #endregion

        public ShapeData()
        {
            // The initial type doesn't matter since the abstract base
            // class will never be instantiated.
            shapeType =  ShapeType.Sphere;
            // Leave the offset as an identity matrix.
            //Offset = Matrix.Identity;
            Material = Defaults.Shape.Material;
            ContactGroup = Defaults.Shape.ContactGroup;
        }

        // CANYONSHOOTER BEGIN
        public abstract ShapeData Clone();

        protected void DeepCopy(ShapeData d)
        {
            d.ContactGroup = ContactGroup;
            d.Offset = Offset;
            d.Material.Bounciness = d.Material.Bounciness;
            d.Material.Density = d.Material.Density;
            d.Material.Friction = d.Material.Friction;
            d.Material.Hardness = d.Material.Hardness;
        }

        public virtual void Scale(float scale)
        {
            Offset.Translation = scale * Offset.Translation;
        }
        // CANYONSHOOTER END

        /// <summary>
   		/// Returns the ShapeData's type.
        /// </summary>
		public virtual ShapeType Type
		{
            get { return shapeType; }
		}

        public abstract BoundingBox LocalAABB { get; }

        public override string ToString()
        {
            return "ShapeData: " + this.Type;
        }


    }

    /// <summary>
    /// The types of Shapes currently available.
    /// </summary>
    public enum ShapeType
    {
        Box,
        Sphere,
        Capsule,
        Plane,
        //RAY_SHAPE,
        Mesh
    };
}
