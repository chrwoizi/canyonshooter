using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a plane Shape.
    /// </summary>
    public class PlaneShapeData : ShapeData
    {
        
		private float[] abcd;

        /// <summary>
        /// Parameters used to define the plane equation: 
        /// a*x + b*y + c*z = d.
        /// </summary>
        public float[] Abcd {
            get {
                return abcd;
            }
            set {
                abcd = value;
            }
        }

        public PlaneShapeData(Plane plane)
        {
   			shapeType =  ShapeType.Plane;

            Abcd = new float[4];

            Abcd[0] = plane.Normal.X;
            Abcd[1] = plane.Normal.Y;
            Abcd[2] = plane.Normal.Z;
            Abcd[3] = plane.D;

            //for (int i=0; i<4; ++i)
            //{
            //    abcd[i] = Defaults.Shape.planeABCD[i];
            //}
        }

        // CANYONSHOOTER BEGIN
        public override ShapeData Clone()
        {
            PlaneShapeData n = new PlaneShapeData(new Plane(Abcd[0], Abcd[1], Abcd[2], Abcd[3]));

            DeepCopy(n);

            return n;
        }

        public override void Scale(float scale)
        {
            throw new Exception("PlaneShapeData cannot be scaled.");
        }
        // CANYONSHOOTER END

        /// <summary>
        /// Planes don't have a bounding box, so this does nothing.
        /// </summary>
        public override BoundingBox LocalAABB
        {
            get 
            {
                return new BoundingBox();
            }
        }
    }
}
