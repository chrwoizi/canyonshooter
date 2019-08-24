using System;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using CS = CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.Weapons
{

    struct InstanceVertex
    {
        public float InstanceIndex;

        public InstanceVertex(int index)
        {
            InstanceIndex = (float)index;
        }

        public static readonly int SizeInBytes = 4;
    }

    public class UltraPhaserProjectile : BasicProjectile, IWeaponProjectile
    {
        private static readonly Vector3 DefaultDirection = new Vector3(0, 0, -1);

        private Quaternion Rotation = Quaternion.Identity;

        private UltraPhaser Owner;

        public UltraPhaserProjectile(ICanyonShooterGame game, Vector3 startPosition, Vector3 direction, UltraPhaser owner, WeaponHolderType weaponHolderType)
            : base(game, startPosition, direction, "Lasergun", weaponHolderType)
        {
            this.game = game;
            this.Owner = owner;

            // ist nur instanz von einem model, daher nicht selbst anzeigen sondern durch die waffe
            Visible = false;

            if (direction.Length() > 0)
            {
                direction.Normalize();
            }
            
            Velocity = direction*500 + owner.Velocity;

            //add Collision Hit Object:
            SphereShapeData hitObject = new SphereShapeData();
            hitObject.Radius = 0.1f;
            hitObject.Material.Hardness = 0.1f;
            hitObject.Material.Bounciness = 0.0f;
            hitObject.Material.Friction = 1.0f;
            hitObject.Material.Density = 1000f;
            hitObject.ContactGroup = (int)ContactGroup;

            AddShape(hitObject);
            InfluencedByGravity = false;

            AutoDestruction = true;
            AutoDestructionTime = TimeSpan.FromSeconds(2);

            Rotation = Helper.RotateTo(direction, DefaultDirection);

            // Größe der Laserstrahlen
            LocalScale = new Vector3(0.5f, 0, 5);

            // initialize the rotation
            MakeRotation();
        }

        private ICanyonShooterGame game = null;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MakeRotation();
        }

        private void MakeRotation()
        {
            LocalRotation = Helper.BillboardRotation(Parent, LocalPosition, game.Renderer.Camera, Rotation);
        }

        public override void Dispose()
        {
            base.Dispose();

            Owner.RemoveProjectileReference(this);
        }

        #region Überflüssig, durch Instancing
        /* rausgenommen wegen instancing
         * 
         * protected override void LoadContent()
        {
            base.LoadContent();

            CS.IMesh[] meshes = new CS.IMesh[1];

            // vertex declaration
            VertexDeclaration vdecl = new VertexDeclaration(game.Graphics.Device, VertexPositionTexture.VertexElements);

            // vertices
            VertexPositionTexture[] verts = new VertexPositionTexture[4];
            verts[0] = new VertexPositionTexture(new Vector3(-.5f, 0, 0), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(new Vector3(-.5f, 0, 1), new Vector2(0, 1));
            verts[2] = new VertexPositionTexture(new Vector3( .5f, 0, 0), new Vector2(1, 0));
            verts[3] = new VertexPositionTexture(new Vector3( .5f, 0, 1), new Vector2(1, 1));
            // vb
            VertexBuffer vb = new VertexBuffer(game.Graphics.Device,
                vdecl.GetVertexStrideSize(0) * verts.Length,
                BufferUsage.None);
            vb.SetData<VertexPositionTexture>(verts);

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
                game.Graphics.Device,
                sizeof(short) * indices.Length,
                BufferUsage.None,
                IndexElementSize.SixteenBits
                );
            ib.SetData<short>(indices);

            meshes[0] = new CS.VbIbAdapterMesh(game, vb, ib, vdecl, 2, 4, "UltraPhaserProjectile");
            
            CS.IMaterial[] materials = new CS.IMaterial[1];
            materials[0] = new CS.Material(game, "PhaserTex");

            Model = new CS.Model(game, meshes, materials, "Phaser");
            Model.LocalScale = new Vector3(1,0,10);

            LocalPosition = LocalPosition - new Vector3(0, 0, 10);
            //Model.Parent = null;

        }*/
        #endregion

        public override void OnExplosion(ITransformable other)
        {
            // dont create an explosion or anything else:
            //base.OnExplosion(other);
        }

        ~UltraPhaserProjectile()
        {
            GraphicalConsole.GetSingleton(game).WriteLine("Projektil gelöscht!",0);
        }

        #region IWeaponProjectile Member

        private int damage = 80;
        public new int Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        #endregion
    }
}
