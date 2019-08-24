using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.GameClasses.World;
using CanyonShooter.Engine.Physics;
using CanyonShooter.Engine.Graphics.Cameras;
using System.Diagnostics;

namespace CanyonShooter.GameClasses.Weapons
{
    public class AfterBurner : GameObject
    {
        private ICanyonShooterGame game;
        private Quaternion rotation = Quaternion.Identity;

        private Vector3 maxSize = Vector3.One;

        private float power = 0.0f;

        public Vector3 MaxSize
        {
            get
            {
                return maxSize;
            }
        }

        public AfterBurner(ICanyonShooterGame game)
            : base(game)
        {
            this.game = game;
        }

        public void UnloadTheContent()
        {
            UnloadContent();
        }

        public void LoadTheContent()
        {
            LoadContent();
        }

        public float Power
        {
            set
            {
                power = value;
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // vertex declaration
            VertexElement[] vertexElements = new VertexElement[]
            {
                // vertex position
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                // texture coordinates
                new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            };
            VertexDeclaration vdecl = new VertexDeclaration(game.Graphics.Device, vertexElements);

            // vertices
            VertexPositionTexture[] verts = new VertexPositionTexture[7];
            verts[0] = new VertexPositionTexture(new Vector3( -0.5f,    0, 1), new Vector2(1, 1));
            verts[1] = new VertexPositionTexture(new Vector3(     0,    0, 1), new Vector2(1, 0.5f));
            verts[2] = new VertexPositionTexture(new Vector3(  0.5f,    0, 1), new Vector2(1, 0));
            verts[3] = new VertexPositionTexture(new Vector3(  0.5f,    0, 0), new Vector2(0, 0));
            verts[4] = new VertexPositionTexture(new Vector3( 0.1f, 0.5f, 0), new Vector2(0, 0.4f));
            verts[5] = new VertexPositionTexture(new Vector3(-0.1f, 0.5f, 0), new Vector2(0, 0.6f));
            verts[6] = new VertexPositionTexture(new Vector3( -0.5f,    0, 0), new Vector2(0, 1));
            // vertexBuffer
            VertexBuffer vertexBuffer = new VertexBuffer(game.Graphics.Device,
                VertexPositionTexture.SizeInBytes * verts.Length,
                BufferUsage.WriteOnly | BufferUsage.None);
            vertexBuffer.SetData(verts);

            // indices
            short[] indices = new short[15];
            indices[0]  = 0;
            indices[1]  = 6;
            indices[2]  = 1;
            indices[3]  = 1;
            indices[4]  = 3;
            indices[5]  = 2;
            indices[6]  = 6;
            indices[7]  = 5;
            indices[8]  = 1;
            indices[9]  = 5;
            indices[10] = 4;
            indices[11] = 1;
            indices[12] = 4;
            indices[13] = 3;
            indices[14] = 1;
            // indexBuffer
            IndexBuffer ib = new IndexBuffer(
                game.Graphics.Device,
                sizeof(short) * indices.Length,
                BufferUsage.None,
                IndexElementSize.SixteenBits
                );
            ib.SetData(indices);

            IMaterial[] materials = new IMaterial[1];
            materials[0] = Material.Create(game, "AfterBurner", InstancingType.None);
            
            IMesh[] meshes = new IMesh[1];
            meshes[0] = new VbIbAdapterMesh(game, vertexBuffer, ib, vdecl, 5, 7, "AfterBurner");

            Model = new Engine.Graphics.Models.Model(game, meshes, materials, "AfterBurner");

            maxSize = LocalScale;

            rotation = LocalRotation;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            LocalRotation = Helper.BillboardRotation(Parent, LocalPosition, game.Renderer.Camera, rotation);
        }

        public override void SetShaderConstant(EffectParameter constant)
        {
            if (constant.Semantic == "POWER")
            {
                constant.SetValue(power);
            }
            else base.SetShaderConstant(constant);
        }
    }
}
