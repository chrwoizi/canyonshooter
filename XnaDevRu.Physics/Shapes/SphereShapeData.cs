using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a sphere Shape.
    /// </summary>
    public class SphereShapeData : ShapeData
    {
        private float radius;

        /// <summary>
        /// The sphere's radius.
        /// </summary>
        public float Radius {
            get {
                return radius;
            }
            set {
                radius = value;
            }
        }

        public override BoundingBox LocalAABB
        {
            get 
            {
                BoundingBox bbox = new BoundingBox();

                bbox.Min.X = -Radius;
                bbox.Max.X = Radius;
                bbox.Min.Y = -Radius;
                bbox.Max.Y = Radius;
                bbox.Min.Z = -Radius;
                bbox.Max.Z = Radius;

                return bbox;
            }
        }

		public SphereShapeData()
        {
            // CANYONSHOOTER BEGIN
            Offset = Matrix.Identity;
            // CANYONSHOOTER END

			Radius = 1f;
        }

        // CANYONSHOOTER BEGIN
        public override ShapeData Clone()
        {
            SphereShapeData n = new SphereShapeData();

            DeepCopy(n);

            n.Radius = Radius;

            return n;
        }

        public override void Scale(float scale)
        {
            base.Scale(scale);

            Radius = scale*Radius;
        }
        // CANYONSHOOTER END
    }
}
