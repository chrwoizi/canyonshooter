using CanyonShooter.Engine.Graphics.Geometry;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class VbIbAdapterMesh : Mesh
    {
        private VertexBuffer vertexBuffer;
        private VertexBuffer instanceBuffer;
        private IndexBuffer indexBuffer;
        private VertexDeclaration vdecl;
        private int triangleCount;
        private int vertexCount;
        private int instanceCount;
        private Effect effect;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VbIbAdapterMesh"/> class.
        /// </summary>
        /// <param name="vertexBuffer">The vb.</param>
        /// <param name="indexBuffer">The ib.</param>
        /// <param name="vdecl">The vdecl.</param>
        /// <param name="triangleCount">The triangle count.</param>
        /// <param name="vertexCount">The vertex count.</param>
        public VbIbAdapterMesh(ICanyonShooterGame game, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, VertexDeclaration vdecl, int triangleCount, int vertexCount, string name)
            : base(game)
        {
            this.vertexBuffer = vertexBuffer;
            this.indexBuffer = indexBuffer;
            this.vdecl = vdecl;
            this.triangleCount = triangleCount;
            this.vertexCount = vertexCount;
            this.name = name;
        }
        public VbIbAdapterMesh(ICanyonShooterGame game, VertexBuffer vertexBuffer, VertexBuffer instanceBuffer, IndexBuffer indexBuffer, VertexDeclaration vdecl, int triangleCount, int vertexCount, int instanceCount, string name)
            : base(game)
        {
            this.vertexBuffer = vertexBuffer;
            this.instanceBuffer = instanceBuffer;
            this.indexBuffer = indexBuffer;
            this.vdecl = vdecl;
            this.triangleCount = triangleCount;
            this.vertexCount = vertexCount;
            this.name = name;
            this.instanceCount = instanceCount;
        }



        #region Mesh Member

        public override void Draw(GraphicsDevice device)
        {
            Effect.Begin(SaveStateMode.SaveState);
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.Indices = indexBuffer;
                device.VertexDeclaration = vdecl;
                device.Vertices[0].SetSource(vertexBuffer, 0, vdecl.GetVertexStrideSize(0));

                if (instanceBuffer != null)
                {
                    device.Vertices[1].SetSource(instanceBuffer, 0, vdecl.GetVertexStrideSize(1));

                    device.Vertices[0].SetFrequencyOfIndexData(instanceCount);
                    device.Vertices[1].SetFrequencyOfInstanceData(1);
                }

                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0, // vertex buffer offset to add to each element of the index buffer 
                    0, // minimum vertex index 
                    vertexCount, // number of vertices 
                    0, // first index element to read 
                    triangleCount // number of primitives to draw 
                    );
                pass.End();

                if (instanceBuffer != null)
                {
                    device.Vertices[1].SetSource(null, 0, 0);
                    device.Vertices[0].SetFrequencyOfIndexData(1);
                }
            }
            Effect.End();

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
            }
        }

        public override string Name
        {
            get { return name; }
        }

        #endregion
    }
}
