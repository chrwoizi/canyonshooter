//using ODE.Geoms;
//using ODE;

namespace XnaDevRu.Physics.Ode
{
    /// <summary>
    /// A data structure used within ODESolids to store data describing 
    /// ODE geoms.
    /// </summary>
    public class GeomData
    {
        public Solid Solid;
        public ShapeData Shape;
        public dGeomID GeomID;
        public dSpaceID SpaceID;
        public dGeomID TransformID;
        public dTriMeshDataID TrimeshDataID; // only used for Solids with trimeshes

        public GeomData()
        {
        }
    }
}
