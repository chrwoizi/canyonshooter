using System;

namespace CanyonShooter.Engine.Graphics.Models
{
    /// <summary>
    /// Die Geometrie mehrerer dynamischer Objekte
    /// </summary>
    public class DynamicInstancingModel : Model
    {
        public DynamicInstancingModel(ICanyonShooterGame game, string name) 
            : base(game, "")
        {
            Load(name, InstancingType.Constants);
        }

        public DynamicInstancingModel(ICanyonShooterGame game, IMesh[] meshes, IMaterial[] materials, string name)
            : base(game, meshes, materials, "")
        {
            foreach (IMesh mesh in meshes)
            {
                if(!(mesh is ConstantsInstancingModelMeshAdapterMesh) && !(mesh is VbIbAdapterMesh))
                {
                    throw new Exception("ConstantsInstancedModel needs ConstantsInstancingModelMeshAdapterMeshs by definition.");
                }
            }
        }
    }
}
