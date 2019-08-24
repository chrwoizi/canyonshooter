using CanyonShooter.Engine.Graphics.Geometry;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CanyonShooter.Engine.Graphics
{
    struct HardwareInstanceVertex
    {
        public Vector4 worldMatrixRow1;
        public Vector4 worldMatrixRow2;
        public Vector4 worldMatrixRow3;

        public HardwareInstanceVertex(Vector4 worldMatrixRow1, Vector4 worldMatrixRow2, Vector4 worldMatrixRow3)
        {
            this.worldMatrixRow1 = worldMatrixRow1;
            this.worldMatrixRow2 = worldMatrixRow2;
            this.worldMatrixRow3 = worldMatrixRow3;
        }
    }

    public class HardwareInstancingModelMeshAdapterMesh : Mesh
    {
        private ModelMesh data;
        private Effect effect;
        private VertexDeclaration vdecl;
        private VertexBuffer instanceBuffer;
        private HardwareInstanceVertex[] instanceVertices;
        private ICanyonShooterGame game;

        public HardwareInstancingModelMeshAdapterMesh(ICanyonShooterGame game, ModelMesh data)
            : base(game)
        {
            if (!game.Graphics.ShaderModel3Supported) throw new Exception("hardware instancing cannot be used with a shader model less than 3.0");
            this.game = game;
            this.data = data;
        }

        private void InitializeInstancingStep1()
        {
            // copy vdecl elements
            VertexElement[] origElements = data.MeshParts[0].VertexDeclaration.GetVertexElements();
            VertexElement[] elements = new VertexElement[origElements.Length + 3];
            for (int i = 0; i < origElements.Length; ++i)
            {
                elements[i] = origElements[i];
                if (elements[i].VertexElementUsage == VertexElementUsage.TextureCoordinate)
                {
                    // increase all texcoord indices by 3 (frees usage indices 0,1,2)
                    elements[i].UsageIndex += 3;
                }
            }
            // add instancing vdelc element (texcoord 0 = instance id)
            elements[elements.Length - 3] = new VertexElement(1, 0, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0);
            elements[elements.Length - 2] = new VertexElement(1, 16, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1);
            elements[elements.Length - 1] = new VertexElement(1, 32, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2);

            // make vdecl
            vdecl = new VertexDeclaration(game.Graphics.Device, elements);
        }

        public void InitializeInstancing(Matrix[] worldMatrices)
        {
            InitializeInstancingStep1();

            // instance buffer
            instanceVertices = new HardwareInstanceVertex[worldMatrices.Length];
            for (int id = 0; id < worldMatrices.Length; ++id)
            {
                Matrix worldMatrix = worldMatrices[id];
                Vector4 row1 = new Vector4(worldMatrix.M11, worldMatrix.M21, worldMatrix.M31, worldMatrix.M41);
                Vector4 row2 = new Vector4(worldMatrix.M12, worldMatrix.M22, worldMatrix.M32, worldMatrix.M42);
                Vector4 row3 = new Vector4(worldMatrix.M13, worldMatrix.M23, worldMatrix.M33, worldMatrix.M43);
                instanceVertices[id] = new HardwareInstanceVertex(row1, row2, row3);
            }

            InitializeInstancingStep3(worldMatrices.Length);
        }

        public void InitializeInstancing(ITransformable[] instances)
        {
            InitializeInstancingStep1();

            // instance buffer
            instanceVertices = new HardwareInstanceVertex[instances.Length];
            for (int id = 0; id < instances.Length; ++id)
            {
                Matrix worldMatrix = instances[id].GlobalTransformation;
                Vector4 row1 = new Vector4(worldMatrix.M11, worldMatrix.M21, worldMatrix.M31, worldMatrix.M41);
                Vector4 row2 = new Vector4(worldMatrix.M12, worldMatrix.M22, worldMatrix.M32, worldMatrix.M42);
                Vector4 row3 = new Vector4(worldMatrix.M13, worldMatrix.M23, worldMatrix.M33, worldMatrix.M43);
                instanceVertices[id] = new HardwareInstanceVertex(row1, row2, row3);
            }

            InitializeInstancingStep3(instances.Length);
        }

        private void InitializeInstancingStep3(int instanceCount)
        {
            // instance buffer
            instanceBuffer = new VertexBuffer(game.Graphics.Device,
                                              typeof(HardwareInstanceVertex),
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
