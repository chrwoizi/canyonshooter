using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Effects;

namespace CanyonShooter.GameClasses.Weapons
{
    public class Minigun : BaseWeapon
    {
        public Minigun(ICanyonShooterGame game,WeaponHolderType weaponHolderType, WeaponManager weaponManager)
            : base(game,weaponHolderType, "Minigun", weaponManager, "Minigun")
        {
            
            // Instance Data initialisieren:
            for (int i = 0; i < instanceCount; i++)
            {
                instanceData[i] = Matrix.Identity;
            }
            DualWeapon = WeaponType.MINIGUN2;
        }

        public override void OnFireStart(Vector3 startPosition, Vector3 direction)
        {
            base.OnFireStart(startPosition, direction);
            Model.GetMesh("Barrel").PlayRotationAnimation();
        }

        public override void OnFireStop(Vector3 startPosition, Vector3 direction)
        {
            base.OnFireStop(startPosition, direction);
            if (Model.GetMesh("Barrel").IsPlayingRotationAnimation())
                Model.GetMesh("Barrel").StopRotationAnimation();
        }

        public override void OnFiring(Vector3 startPosition, Vector3 direction)
        {
            base.OnFiring(startPosition, direction);
            direction.Normalize();


            Vector3 left = Vector3.Cross(Vector3.Transform(Vector3.Up, Parent.GlobalRotation), direction);
            left.Normalize();

            if (projectiles.Count < instanceCount)
            {
                MinigunProjectile projectile = new MinigunProjectile(Game, left + startPosition, direction, this, WeaponHolderType);
                Game.World.AddObject(projectile);
                projectiles.Add(projectile);
            }
        }

        public override void OnReload()
        {
            base.OnReload();
            // Create Reload Smoke for glowing GunBarrels
            IEffect fx = this.Game.Effects.CreateEffect("MinigunReloadSmoke");
            fx.Parent = this; 
            fx.LocalPosition = new Vector3(0,0,-15);
            this.Game.World.AddEffect(fx);
            fx.Play(TimeSpan.FromMilliseconds(4000),true);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int id = 0;
            for (; id < projectiles.Count; )
            {
                // Transformation der Instanz setzen
                instanceData[id] = Matrix.Transpose(projectiles[id].GlobalTransformation);
                // Visible Prop in der Matrix mit setzen:
                instanceData[id].M41 = 1.0f;

                ++id;
            }
            for (int i = id; i < previousCount; ++i)
            {
                // Visible Prop in der Matrix mit setzen:
                instanceData[i].M41 = 0.0f;
            }

            previousCount = projectiles.Count;
        }


        #region Minigun Extra Code

        private IModel projectileInstances;

        private static readonly int instanceCount = 50; //Maximal 200 im Shader Model 2.0!!!

        private VertexBuffer instanceBuffer;

        private List<MinigunProjectile> projectiles = new List<MinigunProjectile>();

        private InstanceVertex[] instanceVertices = new InstanceVertex[instanceCount];

        private Matrix[] instanceData = new Matrix[instanceCount];

        private EffectParameter instanceParameter = null;

        private int previousCount = 0;
        
        private Vector3 start;

        private Vector3 dir;
        
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
            verts[0] = new VertexPositionTexture(new Vector3(-1f, 0, -12), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(-1f, 0, 0), new Vector2(0, 1));
            verts[2] = new VertexPositionTexture(new Vector3(1f, 0, -12), new Vector2(1, 0));
            verts[3] = new VertexPositionTexture(new Vector3(1f, 0, 0), new Vector2(1, 1));
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

            meshes[0] = new VbIbAdapterMesh(Game, vertexBuffer, instanceBuffer, ib, vdecl, 2, 4, instanceCount, "MiniGun");

            IMaterial[] materials = new IMaterial[1];
            materials[0] = Material.Create(Game, "BulletTex", InstancingType.Constants);

            projectileInstances = new DynamicInstancingModel(Game, meshes, materials, "Bullet");
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

        public void RemoveProjectileReference(MinigunProjectile projectile)
        {
            projectiles.Remove(projectile);
        }

        #endregion

    }
}
