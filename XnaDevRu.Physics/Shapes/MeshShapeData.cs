using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a mesh Shape.  A mesh is represented 
    /// as an array of vertices and an array of triangles (the triangle 
    /// array contains indices referring to vertices in the vertex 
    /// array).  The data pointers to these arrays must remain valid; OPAL 
    /// simply stores pointers to the user-supplied arrays.  Additionally, 
    /// the arrays must be destroyed by the user when finished.  It is 
    /// critical that the size of the data type used in these arrays 
    /// (i.e. OPAL real) matches the size of the data type expected by 
    /// the underlying physics engine (don't mix floats with doubles).  
    /// One limitation is that other Shapes can only collide with a mesh's 
    /// surface.  There is no concept of "inside" or "outside" a mesh (like 
    /// there is with the primitive Shapes); an object that is "inside" a 
    /// closed mesh surface will not be detected as a collision.
    /// </summary>
    public class MeshShapeData : ShapeData
    {

        #region fields

        /// <summary>
        /// The mesh's local axis-aligned bounding box.  This array stores 
        /// data in the following order: min x, max x, min y, max y, min z, 
        /// max z.  This will be updated automatically when the mesh is 
        /// added to a Solid.
        /// </summary>
        protected BoundingBox localAABB;

        
        private Vector3[] vertexArray;
        private int[] triangleArray;

        #endregion

        #region Properties

        /// <summary>
        /// Pointer to a 1-dimensional array of vertices.  This data must 
        /// be allocated/deallocated by the user.  
        /// </summary>
        public Vector3[] VertexArray {
            get {
                return vertexArray;
            }
            set {
                vertexArray = value;
            }
        }

        /// <summary>
        /// Pointer to a 1-dimensional array of indexed triangles.  This 
        /// data must be allocated/deallocated by the user.  The size of 
        /// this array must be 3 * the number of triangles because each 
        /// triangle uses 3 elements in the array.  Each element is an 
        /// index into the vertex array.  To access the ith triangle's 
        /// vertices, for example, triangleArray[i * 3] is the index 
        /// for the 1st vertex, triangleArray[i* 3 + 1] is the index for 
        /// the 2nd vertex, and triangleArray[i * 3 + 2] is the index 
        /// for the 3rd vertex.
        /// </summary>
        public int[] TriangleArray {
            get {
                return triangleArray;
            }
            set {
                triangleArray = value;
            }
        }
        #endregion

        public MeshShapeData()
        {
            // CANYONSHOOTER BEGIN
            Offset = Matrix.Identity;
            // CANYONSHOOTER END

            shapeType =  ShapeType.Mesh;
        }

        // CANYONSHOOTER BEGIN
        public override ShapeData Clone()
        {
            MeshShapeData n = new MeshShapeData();

            DeepCopy(n);

            n.TriangleArray = TriangleArray;
            n.VertexArray = VertexArray;

            return n;
        }

        public override void Scale(float scale)
        {
            throw new Exception("MeshShapeData cannot be scaled.");
        }
        // CANYONSHOOTER END

        /// <summary>
        /// The number of vertices in the mesh.
        /// </summary>
        public int NumVertices
        {
            get
            {
                if(this.VertexArray == null)
                    return 0;

                return VertexArray.Length;
            }
        }

        /// <summary>
        /// The number of triangles in the mesh.
        /// </summary>
        public int NumTriangles
        {
            get
            {
                if(TriangleArray == null)
                    return 0;

                return TriangleArray.Length / 3;
            }
        }

        /// <summary>
        /// This recomputes the axis-aligned bounding box from the vertex 
        /// data, so it should not be called very often.
        /// </summary>
        public override BoundingBox LocalAABB
        {
            get
            {
                // Compute the AABB from the vertex data.
                localAABB = BoundingBox.CreateFromPoints(VertexArray); // XNA fw rulezz

                return localAABB;
            }       
        }
    }
}
