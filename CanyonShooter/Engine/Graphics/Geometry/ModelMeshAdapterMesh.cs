using CanyonShooter.Engine.Graphics.Geometry;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class ModelMeshAdapterMesh : Mesh
    {
        private ModelMesh data;
        private Effect effect;

        public ModelMeshAdapterMesh(ICanyonShooterGame game, ModelMesh data) : base(game)
        {
            this.data = data;
        }


        #region Mesh Member

        public override void Draw(GraphicsDevice device)
        {
            data.Draw(SaveStateMode.SaveState);
        }

        public override Effect Effect
        {
            get
            {
                return effect;
            }
            set
            {
                effect = value;
                foreach(ModelMeshPart mp in data.MeshParts)
                {
                    mp.Effect = effect;
                }
            }
        }

        public override string Name
        {
            get { return data.Name; }
        }

        #endregion
    }
}
