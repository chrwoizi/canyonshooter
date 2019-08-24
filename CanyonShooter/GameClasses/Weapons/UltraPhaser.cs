using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Graphics;

namespace CanyonShooter.GameClasses.Weapons
{
    public class UltraPhaser : BaseWeapon
    {
        public UltraPhaser(ICanyonShooterGame game,WeaponHolderType weaponHolderType,WeaponManager weaponManager)
            :base(game,weaponHolderType,"UltraPhaser",weaponManager,"Weapon.UltraPhaser")
        {

            // Instance Data initialisieren:
            for (int i = 0; i < instanceCount; i++)
            {
                instanceData[i] = Matrix.Identity;
            }
            
        }

        public override void OnFiring(Vector3 startPosition, Vector3 direction)
        {
            base.OnFiring(startPosition, direction);
            if (projectiles.Count < instanceCount)
            {
                UltraPhaserProjectile projectile = new UltraPhaserProjectile(Game, startPosition, direction, this, WeaponHolderType);
                Game.World.AddObject(projectile);
                projectiles.Add(projectile);
            }

            //TODO: Implement WeaponUpgrades
            #region Dreckiger Hack für Rotating DUAL Lasers (AUS ALTER ULTRAPHASER)

            //Vector3 left = Vector3.Cross(Vector3.Transform(Vector3.Up, Parent.GlobalRotation), dir);
            //left.Normalize();
            //Quaternion t = Quaternion.CreateFromAxisAngle(dir, weaponSpin);
            //Vector3 pos1 = Vector3.Transform(left*(0.3f), t);
            //Vector3 pos2 = Vector3.Transform(left*(-0.3f), t);

            //Vector3 left = Vector3.Cross(Vector3.Transform(Vector3.Up, Parent.GlobalRotation), dir);
            //left.Normalize();
            //Vector3 pos1 = left*(0.5f);
            //Vector3 pos2 = left*(-0.5f);

            //UltraPhaserProjectile projectile;

            //switcher = !switcher;
            //if (switcher)
            //    projectile = new UltraPhaserProjectile(game, pos1 + start, dir, this);
            //else
            //    projectile = new UltraPhaserProjectile(game, pos2 + start, dir, this);

            //if (projectiles.Count < instanceCount)
            //{
            //    projectile = new UltraPhaserProjectile(game, pos1 + start, dir, this, weaponHolderType);
            //    game.World.AddObject(projectile);
            //    projectiles.Add(projectile);
            //}

            //if (projectiles.Count < instanceCount)
            //{
            //    projectile = new UltraPhaserProjectile(game, pos2 + start, dir, this, weaponHolderType);
            //    game.World.AddObject(projectile);
            //    projectiles.Add(projectile);
            //}

            #endregion
        }

        #region UltraPhaser Extra Stuff


        private IModel projectileInstances;

        private static readonly int instanceCount = 20; //Maximal 200 im Shader Model 2.0!!!

        private VertexBuffer instanceBuffer;

        private List<UltraPhaserProjectile> projectiles = new List<UltraPhaserProjectile>();

        private InstanceVertex[] instanceVertices = new InstanceVertex[instanceCount];

        private Matrix[] instanceData = new Matrix[instanceCount];

        private EffectParameter instanceParameter = null;


        protected override void LoadContent()
        {
            base.LoadContent();

            IMesh[] meshes = new IMesh[1];

            // vertex declaration
            VertexElement[] vertexElements = new VertexElement[]
            {
                // vertex data
                // vertex position
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                // texture coordinates
                new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),

                // instance data 
                // instance transform matrix 3x4
                new VertexElement(1, 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
            };
            VertexDeclaration vdecl = new VertexDeclaration(Game.Graphics.Device, vertexElements);

            // vertices
            VertexPositionTexture[] verts = new VertexPositionTexture[4];
            verts[0] = new VertexPositionTexture(new Vector3(-8f, 0, -4), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(-8f, 0, 0), new Vector2(0, 1));
            verts[2] = new VertexPositionTexture(new Vector3(8f, 0, -4), new Vector2(1, 0));
            verts[3] = new VertexPositionTexture(new Vector3(8f, 0, 0), new Vector2(1, 1));
            // vertexBuffer
            VertexBuffer vertexBuffer = new VertexBuffer(Game.Graphics.Device,
                VertexPositionTexture.SizeInBytes * verts.Length,
                BufferUsage.WriteOnly | BufferUsage.None);
            vertexBuffer.SetData<VertexPositionTexture>(verts);

            // instances
            instanceVertices = new InstanceVertex[instanceCount];
            for (int i = 0; i < instanceCount; ++i)
            {
                instanceVertices[i] = new InstanceVertex(i);
            }
            // instanceBuffer
            instanceBuffer = new VertexBuffer(Game.Graphics.Device,
                typeof(InstanceVertex),
                instanceCount,
                BufferUsage.WriteOnly);
            instanceBuffer.SetData<InstanceVertex>(instanceVertices);

            // indices
            short[] indices = new short[6];
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;
            indices[3] = 3;
            indices[4] = 1;
            indices[5] = 0;

            // ib
            IndexBuffer ib = new IndexBuffer(
                Game.Graphics.Device,
                sizeof(short) * indices.Length,
                BufferUsage.None,
                IndexElementSize.SixteenBits
                );
            ib.SetData<short>(indices);

            meshes[0] = new VbIbAdapterMesh(Game, vertexBuffer, instanceBuffer, ib, vdecl, 2, 4, instanceCount, "UltraPhaserProjectile");

            IMaterial[] materials = new IMaterial[1];
            materials[0] = Material.Create(Game, "PhaserTex", InstancingType.Constants);

            projectileInstances = new DynamicInstancingModel(Game, meshes, materials, "Phaser");
            foreach (EffectParameter parameter in projectileInstances.Materials[0].Effect.Parameters)
            {
                if (parameter.Semantic == "INSTANCE_DATA")
                {
                    instanceParameter = parameter;
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            projectileInstances.Draw(this);
        }

        public override void SetShaderConstant(EffectParameter constant)
        {
            if (constant.Semantic == "INSTANCE_DATA")
            {
                constant.SetValue(instanceData);
            }
            else base.SetShaderConstant(constant);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            #region Stuff for Instancing
            int id = 0;
            for (; id < projectiles.Count; )
            {
                // GlobalVertexdaten setzen?
                instanceData[id] = Matrix.Transpose(projectiles[id].GlobalTransformation);
                //Visible Prop in der Matrix mit setzen:
                instanceData[id].M41 = 1.0f;

                ++id;
            }
            for (int i = id; i < previousCount; ++i)
            {
                //Visible Prop in der Matrix mit setzen:
                instanceData[i].M41 = 0.0f;
            }

            previousCount = projectiles.Count;
            #endregion
        }


        public void RemoveProjectileReference(UltraPhaserProjectile projectile)
        {
            projectiles.Remove(projectile);
        }

        private int previousCount = 0;

        #endregion
    }
}
