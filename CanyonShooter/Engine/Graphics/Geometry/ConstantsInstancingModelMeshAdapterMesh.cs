using CanyonShooter.Engine.Graphics.Geometry;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    struct ConstantsInstanceVertex
    {
        public float InstanceIndex;

        public ConstantsInstanceVertex(int index)
        {
            InstanceIndex = (float)index;
        }
    }

    public class ConstantsInstancingModelMeshAdapterMesh : Mesh
    {
        private ModelMesh data;
        private Effect effect;
        private VertexDeclaration vdecl;
        private VertexBuffer instanceBuffer;
        private ConstantsInstanceVertex[] instanceVertices;
        private ICanyonShooterGame game;

        public ConstantsInstancingModelMeshAdapterMesh(ICanyonShooterGame game, ModelMesh data)
            : base(game)
        {
            this.game = game;
            this.data = data;
        }

        public void InitializeInstancing(int instanceCount)
        {
            // copy vdecl elements
            VertexElement[] origElements = data.MeshParts[0].VertexDeclaration.GetVertexElements();
            VertexElement[] elements = new VertexElement[origElements.Length + 1];
            for (int i = 0; i < origElements.Length; ++i )
            {
                elements[i] = origElements[i];
                if (elements[i].VertexElementUsage == VertexElementUsage.TextureCoordinate)
                {
                    // increase all texcoord indices by 1 (free usage index 0)
                    elements[i].UsageIndex++;
                }
            }
            // add instancing vdelc element (texcoord 0 = instance id)
            elements[elements.Length -1] = new VertexElement(1, 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0);

            // make vdecl
            vdecl = new VertexDeclaration(game.Graphics.Device, elements);

            // instance buffer
            instanceVertices = new ConstantsInstanceVertex[instanceCount];
            for (int i = 0; i < instanceCount; ++i)
            {
                instanceVertices[i] = new ConstantsInstanceVertex(i);
            }
            instanceBuffer = new VertexBuffer(game.Graphics.Device,
                typeof(ConstantsInstanceVertex),
                instanceCount,
                BufferUsage.WriteOnly);
            instanceBuffer.SetData(instanceVertices);
        }


        #region Mesh Member

        public override void Draw(GraphicsDevice device)
        {
            // draw mesh 0 only

            data.Effects[0].Begin();

            foreach (EffectPass pass in data.Effects[0].CurrentTechnique.Passes)
            {
                pass.Begin();

                device.VertexDeclaration = vdecl;

                device.Vertices[0].SetSource(data.VertexBuffer, data.MeshParts[0].StreamOffset, vdecl.GetVertexStrideSize(0));
                device.Vertices[1].SetSource(instanceBuffer, 0, vdecl.GetVertexStrideSize(1));

                device.Vertices[0].SetFrequencyOfIndexData(instanceVertices.Length);
                device.Vertices[1].SetFrequencyOfInstanceData(1);

                device.Indices = data.IndexBuffer;
                
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                      data.MeshParts[0].BaseVertex,
                                      0,
                                      data.MeshParts[0].NumVertices,
                                      data.MeshParts[0].StartIndex,
                                      data.MeshParts[0].PrimitiveCount);

                device.Vertices[1].SetSource(null, 0, 0);
                device.Vertices[0].SetFrequencyOfIndexData(1);

                pass.End();
            }

            data.Effects[0].End();
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
