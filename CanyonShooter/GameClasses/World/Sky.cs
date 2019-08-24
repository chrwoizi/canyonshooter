// Zuständigkeit: Christian

#region Using Statements

using System;
using CanyonShooter.Engine;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.Engine.Graphics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model=CanyonShooter.Engine.Graphics.Models.Model;

#endregion

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// Skybox oder Skydome
    /// </summary>
    public class Sky : DrawableGameComponent, ISky
    {
        private ICanyonShooterGame game;

        private string name;

        private Material material;
        private VbIbAdapterMesh mesh;
        private Model model;

        private ISunlight sunlight;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sky"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="name">The name of the material.</param>
        public Sky(ICanyonShooterGame game, string name)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            DrawOrder = (int)DrawOrderType.Sky;

            sunlight = new Sunlight(game);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (game.Graphics.ShadowMappingSupported)
            {
                sunlight.Update(gameTime);
            }
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.  Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.Renderer.Draw(model, null);
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. Override this method to load any component-specific graphics resources.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            IMesh[] meshes = new IMesh[1];

            // vertex declaration
            VertexElement[] velements = new VertexElement[1];
            velements[0] = new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0);
            VertexDeclaration vdecl = new VertexDeclaration(game.Graphics.Device, velements);

            // vertices
            Vector3[] verts = new Vector3[8];
            verts[0] = new Vector3(-10, -10, -10);
            verts[1] = new Vector3(-10, -10,  10);
            verts[2] = new Vector3(-10,  10, -10);
            verts[3] = new Vector3(-10,  10,  10);
            verts[4] = new Vector3( 10, -10, -10);
            verts[5] = new Vector3( 10, -10,  10);
            verts[6] = new Vector3( 10,  10, -10);
            verts[7] = new Vector3( 10,  10,  10);

            // vb
            VertexBuffer vb = new VertexBuffer(game.Graphics.Device,
                vdecl.GetVertexStrideSize(0) * verts.Length,
                BufferUsage.None);
            vb.SetData<Vector3>(verts);

            // indices
            short[] indices = new short[36];
            indices[ 0] = 0;
            indices[ 1] = 2;
            indices[ 2] = 6;
            indices[ 3] = 6;
            indices[ 4] = 4;
            indices[ 5] = 0;
            indices[ 6] = 0;
            indices[ 7] = 1;
            indices[ 8] = 2;
            indices[ 9] = 2;
            indices[10] = 1;
            indices[11] = 3;
            indices[12] = 3;
            indices[13] = 1;
            indices[14] = 5;
            indices[15] = 5;
            indices[16] = 7;
            indices[17] = 3;
            indices[18] = 7;
            indices[19] = 5;
            indices[20] = 6;
            indices[21] = 6;
            indices[22] = 5;
            indices[23] = 4;
            indices[24] = 4;
            indices[25] = 5;
            indices[26] = 1;
            indices[27] = 1;
            indices[28] = 0;
            indices[29] = 4;
            indices[30] = 2;
            indices[31] = 3;
            indices[32] = 6;
            indices[33] = 6;
            indices[34] = 3;
            indices[35] = 7;

            // ib
            IndexBuffer ib = new IndexBuffer(
                game.Graphics.Device,
                sizeof(short) * indices.Length,
                BufferUsage.None,
                IndexElementSize.SixteenBits
                );
            ib.SetData<short>(indices);

            meshes[0] = mesh = new VbIbAdapterMesh(game, vb, ib, vdecl, 12, 8, "Sky");

            IMaterial[] materials = new IMaterial[1];
            materials[0] = material = Material.Create(game, name, InstancingType.None);

            model = new Model(game, meshes, materials, "Sky");
        }


        #region ISky Members

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ISunlight Sunlight
        {
            get { return sunlight; }
            set { sunlight = value; }
        }

        #endregion
    }
}


