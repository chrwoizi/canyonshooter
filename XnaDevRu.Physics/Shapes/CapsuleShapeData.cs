using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a capsule Shape.  Capsules start out 
    /// aligned along their local Z axis.
    /// </summary>
    public class CapsuleShapeData : ShapeData
    {
        
        private float radius;
        private float length;

        /// <summary>
        /// The capsule's radius.
        /// </summary>
        public float Radius {
            get {
                return radius;
            }
            set {
                radius = value;
            }
        }

        /// <summary>
        /// The capsule's length, not including the round caps.
        /// </summary>
        public float Length {
            get {
                return length;
            }
            set {
                length = value;
            }
        }


        public CapsuleShapeData()
        {
            // CANYONSHOOTER BEGIN
            Offset = Matrix.Identity;
            // CANYONSHOOTER END

			shapeType =  ShapeType.Capsule;
			Radius = Defaults.Shape.CapsuleRadius;
			Length = Defaults.Shape.CapsuleLength;
        }

        // CANYONSHOOTER BEGIN
        public override ShapeData Clone()
        {
            CapsuleShapeData n = new CapsuleShapeData();

            DeepCopy(n);

            n.Radius = Radius;
            n.Length = Length;

            return n;
        }

        public override void Scale(float scale)
        {
            base.Scale(scale);

            Radius = scale * Radius;
            Length = scale * Length;
        }
        // CANYONSHOOTER END

        public override BoundingBox LocalAABB
        {
            get 
            {
                BoundingBox bbox = new BoundingBox();
                // The standard initial capsule orientation in OPAL is to 
                // align it along the Z axis.
                //aabb[0] = -radius;
                bbox.Min.X = -Radius;
                //aabb[1] = radius;
                bbox.Max.X = Radius;
                //aabb[2] = -radius;
                bbox.Min.Y = -Radius;
                //aabb[3] = radius;
                bbox.Max.Y = Radius;
                //aabb[4] = -(real)0.5 * length - radius;
                bbox.Min.Z = -0.5f * Length - Radius;
                //aabb[5] = (real)0.5 * length + radius;
                bbox.Max.Z = 0.5f * Length + Radius;

                return bbox;
            }
        }
    }
}
