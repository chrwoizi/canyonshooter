using System;

namespace CanyonShooter.Engine.Graphics.Models
{
    /// <summary>
    /// Die Geometrie mehrerer statischer Objekte
    /// </summary>
    public class StaticInstancingModel : Model
    {
        public StaticInstancingModel(ICanyonShooterGame game, string name) 
            : base(game, "")
        {
            Load(name, InstancingType.Hardware);
        }

        public StaticInstancingModel(ICanyonShooterGame game, IMesh[] meshes, IMaterial[] materials, string name)
            : base(game, meshes, materials, "")
        {
            foreach (IMesh mesh in meshes)
            {
                if (!(mesh is HardwareInstancingModelMeshAdapterMesh) && !(mesh is VbIbAdapterMesh))
                {
                    throw new Exception("HardwareInstancedModel needs HardwareInstancedModelMeshAdapterMeshs by definition.");
                }
            }
        }
    }
}
