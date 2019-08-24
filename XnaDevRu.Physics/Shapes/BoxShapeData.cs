using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a box Shape.
    /// </summary>
    public class BoxShapeData : ShapeData
    {
        /// <summary>
        /// The box's dimensions.
        /// </summary>
        public Vector3 Dimensions;

        public BoxShapeData()
		{
            // CANYONSHOOTER BEGIN
            Offset = Matrix.Identity;
            // CANYONSHOOTER END

			shapeType =  ShapeType.Box;
			Dimensions = Defaults.Shape.BoxDimensions;
        }

        // CANYONSHOOTER BEGIN
        public override ShapeData Clone()
        {
            BoxShapeData n = new BoxShapeData();

            DeepCopy(n);

            n.Dimensions = Dimensions;

            return n;
        }

        public override void Scale(float scale)
        {
            base.Scale(scale);

            Dimensions = scale * Dimensions;
        }
        // CANYONSHOOTER END

		public override BoundingBox LocalAABB
		{
            get
            {
                BoundingBox bbox = new BoundingBox();

                bbox.Min.X = -0.5f * Dimensions.X;
                bbox.Max.X = 0.5f * Dimensions.X;
                bbox.Min.Y = -0.5f * Dimensions.Y;
                bbox.Max.Y = 0.5f * Dimensions.Y;
                bbox.Min.Z = -0.5f * Dimensions.Z;
                bbox.Max.Z = 0.5f * Dimensions.Z;

                return bbox;
            }
		}
    }
}
