// Zuständigkeit: Danny und Thorben

#region Using Statements

using System;
using System.Collections.Generic;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;
using Material=CanyonShooter.Engine.Graphics.Material;
using Model=CanyonShooter.Engine.Graphics.Models.Model;

#endregion

namespace CanyonShooter.GameClasses.World.Canyon
{

    public class Segment : GameObject
    {
        public struct VertexPositionNormalBinormalTangentTexture
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 Binormal;
            public Vector3 Tangent;
            public Vector2 TextureCoordinate;

            public static readonly int SizeInBytes = 4 * 12 + 8;
            public static readonly VertexElement[] VertexElements = {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0), 
                new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0), 
                new VertexElement(0, 24, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0), 
                new VertexElement(0, 36, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0), 
                new VertexElement(0, 48, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0), 
            };

            public VertexPositionNormalBinormalTangentTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
            {
                Position = position;
                Normal = normal;
                TextureCoordinate = textureCoordinate;
                Tangent = new Vector3(1, 0, 0);
                Binormal = new Vector3(0, 1, 0);
            }
        }

        private ICanyonShooterGame game = null;
        const int canyonLengthPoints = 8;
        Points fields = null;
        int globalIndex;
        private List<XnaDevRu.Physics.BoxShapeData> boundingBoxes;

        public int GlobalIndex
        {
            get { return globalIndex; }
            set { globalIndex = value; }
        }

        //static int var = 0;

        public Points Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        Segment lastCanyonSegment = null;

        public Segment LastCanyonSegment
        {
            get { return lastCanyonSegment; }
            set { lastCanyonSegment = value; }
        }

        Segment nextCanyonSegement = null;

        public Segment NextCanyonSegement
        {
            get { return nextCanyonSegement; }
            set { nextCanyonSegement = value; }
        }

        SegmentDescription canyonValues;

        public SegmentDescription CanyonValues
        {
            get { return canyonValues; }
            set { canyonValues = value; }
        }

        VertexDeclaration vertexDeclaration;
        VertexPositionNormalBinormalTangentTexture[] vertexArray;

        public VertexPositionNormalBinormalTangentTexture[] VertexArray
        {
            get { return vertexArray; }
            set { vertexArray = value; }
        }
        VertexBuffer vertexBuffer;
        short[] index;

        public short[] Index
        {
            get { return index; }
            set { index = value; }
        }
        IndexBuffer indexBuffer;
        MeshShapeData collision;
        int vertexCount;
        int indexCount;

        public Vector3 PlaneCoordinates
        {
            get { return LocalPosition; }
            set { LocalPosition = value; }
        }

        private VbIbAdapterMesh[] myMesh = new VbIbAdapterMesh[1];

        public Segment(ICanyonShooterGame game)
            : base(game, "Canyon")
        {
            this.game = game;

            ConnectedToXpa = true;
            Static = true;
            ContactGroup = Engine.Physics.ContactGroup.Canyon;
            this.boundingBoxes = new List<XnaDevRu.Physics.BoxShapeData>();
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here            
            if (lastCanyonSegment == null)
            {
                LocalPosition = Vector3.Zero;//new Vector3(-3.5f, 2, 0);
            }
            else
            {
                LocalPosition = lastCanyonSegment.LocalPosition + canyonValues.Direction;
            }
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        /// <summary>
        /// Initializes the point list with the vertex buffers.
        /// </summary>
        private void InitializePointList()
        {
            fields = new Points(canyonLengthPoints, this.lastCanyonSegment.Fields, game);

            for (int i = 1; i < canyonValues.CanyonForm.Count; i++)
            {
                Vector3 currentStart;
                Vector3 currentDirection;
                Vector3 lastCurrentDirection;
                Vector3 lastCurrentStart;
                float curve = 0.15f;
                if (i == 1)
                {
                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1)) *
                            (1.0f - curve);
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1)) *
                            (1.0f - curve);

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1);
                }
                else if (i == canyonValues.CanyonForm.Count)
                {

                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1)) *
                            (1.0f - curve);
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1)) *
                            (1.0f - curve);

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1) +
                            (currentDirection * curve);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1) +
                            (lastCurrentDirection * curve);
                }
                else
                {
                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1));
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1));

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1) +
                            (currentDirection * curve);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1) +
                            (lastCurrentDirection * curve);

                    currentDirection *= 1.0f - 2 * curve;
                    lastCurrentDirection *= 1.0f - 2 * curve;
                }
                Vector3[,] tmp = createStraightWall(lastCurrentStart, currentStart, lastCurrentDirection, currentDirection, 3);
                //createBox()
                fields.Add(tmp);
            }

            fields.CreateCompletePlain();

            // Hier muss nun das Feld bearbeitet werden
            //CalculateNormals();
            //CalculateNoiseFromNormals();
            //CalculateNormals();
        }

        private Vector3[,] createStraightWall(Vector3 currentStart, Vector3 nextCurrentStart, Vector3 direction, Vector3 nextDirection, int points)
        {

            Vector3[,] tmp;
            tmp = new Vector3[points, canyonLengthPoints];
            for (int i = 0; i < points; i++)
            {
                Vector3 currentDirection = (nextCurrentStart + nextDirection / (points - 1) * i) - (currentStart + direction / (points - 1) * i);
                for (int j = 0; j < canyonLengthPoints; j++)
                {
                    Vector3 position = currentStart + (direction / (points - 1) * i) + currentDirection / (canyonLengthPoints - 1) * j;
                    tmp[i, j] = position;
                }
            }
            return tmp;
        }



        private Vector3 GetPointFromPlane(SegmentDescription desc, int position)
        {
            return desc.CanyonForm[position].X * desc.PlaneX + desc.CanyonForm[position].Y * desc.PlaneY;
        }

        private void ComputeTangentFrame()
        {
            for (int i = 0; i < index.Length / 3; i++)
            {
                VertexPositionNormalBinormalTangentTexture v1 = vertexArray[index[i * 3 + 0]];
                VertexPositionNormalBinormalTangentTexture v2 = vertexArray[index[i * 3 + 1]];
                VertexPositionNormalBinormalTangentTexture v3 = vertexArray[index[i * 3 + 2]];


                // Edges
                VertexPositionTexture e12 = new VertexPositionTexture();
                VertexPositionTexture e13 = new VertexPositionTexture();
                VertexPositionTexture e21 = new VertexPositionTexture();
                VertexPositionTexture e31 = new VertexPositionTexture();
                VertexPositionTexture e23 = new VertexPositionTexture();
                VertexPositionTexture e32 = new VertexPositionTexture();

                e12.Position = v2.Position - v1.Position;
                e12.TextureCoordinate = v2.TextureCoordinate - v1.TextureCoordinate;

                e13.Position = v3.Position - v1.Position;
                e13.TextureCoordinate = v3.TextureCoordinate - v1.TextureCoordinate;

                e21.Position = v1.Position - v2.Position;
                e21.TextureCoordinate = v1.TextureCoordinate - v2.TextureCoordinate;

                e31.Position = v1.Position - v3.Position;
                e31.TextureCoordinate = v1.TextureCoordinate - v3.TextureCoordinate;

                e32.Position = v2.Position - v3.Position;
                e32.TextureCoordinate = v2.TextureCoordinate - v3.TextureCoordinate;

                e23.Position = v3.Position - v2.Position;
                e23.TextureCoordinate = v3.TextureCoordinate - v2.TextureCoordinate;


                // Binormals
                Vector3 vec = e12.TextureCoordinate.X * e13.Position - e13.TextureCoordinate.X * e12.Position;
                vec.Normalize();
                v1.Binormal = new Vector3(vec.X, vec.Y, vec.Z);

                vec = e23.TextureCoordinate.X * e21.Position - e21.TextureCoordinate.X * e23.Position;
                vec.Normalize();
                v2.Binormal = new Vector3(vec.X, vec.Y, vec.Z);

                vec = e31.TextureCoordinate.X * e32.Position - e32.TextureCoordinate.X * e31.Position;
                vec.Normalize();
                v3.Binormal = new Vector3(vec.X, vec.Y, vec.Z);


                // Tangents
                vec = e12.TextureCoordinate.Y * e13.Position - e13.TextureCoordinate.Y * e12.Position;
                vec.Normalize();
                v1.Tangent = new Vector3(vec.X, vec.Y, vec.Z);

                vec = e23.TextureCoordinate.Y * e21.Position - e21.TextureCoordinate.Y * e23.Position;
                vec.Normalize();
                v2.Tangent = new Vector3(vec.X, vec.Y, vec.Z);

                vec = e31.TextureCoordinate.Y * e32.Position - e32.TextureCoordinate.Y * e31.Position;
                vec.Normalize();
                v3.Tangent = new Vector3(vec.X, vec.Y, vec.Z);


                vertexArray[index[i * 3 + 0]] = v1;
                vertexArray[index[i * 3 + 1]] = v2;
                vertexArray[index[i * 3 + 2]] = v3;
            }
        }

        private void InitModel()
        {
            ComputeTangentFrame();

            vertexDeclaration = new VertexDeclaration(
                game.Graphics.Device,
                VertexPositionNormalBinormalTangentTexture.VertexElements);

            vertexBuffer = new VertexBuffer(game.Graphics.Device,
                VertexPositionNormalBinormalTangentTexture.SizeInBytes * (vertexArray.Length),
                BufferUsage.None);

            vertexBuffer.SetData(vertexArray);

            indexBuffer = new IndexBuffer(
                game.Graphics.Device,
                sizeof(short) * index.Length,
                BufferUsage.None,
                IndexElementSize.SixteenBits
                );

            // Set the data in the index buffer to our array
            indexBuffer.SetData(
                index);

            myMesh[0] = new VbIbAdapterMesh(game, vertexBuffer, indexBuffer, vertexDeclaration, indexCount, vertexCount, "CanyonSegment");
            IMaterial[] tmp = new IMaterial[1];
            tmp[0] = material;

            Model = new Model(game, myMesh, tmp, "CanyonModel");

            CreateAllSegmentBoxes();
           /*
            if (globalIndex == 4)
            {
                //Vector3 forward = new Vector3(1, 1, -3);
                //Vector3 up = Vector3.Cross(forward, new Vector3(0,0,-1));

                //createBoxFromVectorsAndPostion(forward, up, 30, Vector3.Zero);
                createCanyonBoxes(new Vector3(0, 0, 400), new Vector3(0, 0, 200), new Vector3(50, 0, 0), new Vector3(0, 50, 0), 4);
            }
            */
            /*
            XnaDevRu.Physics.BoxShapeData = new BoxShapeData();


            Model.CollisionShapes.Add(b);
            AddShape(b);*/
        }

        public void CreateAllSegmentBoxes()
        {
            for (int i = 1; i < canyonValues.CanyonForm.Count; i++)
            {
                Vector3 currentStart;
                Vector3 currentDirection;
                Vector3 lastCurrentDirection;
                Vector3 lastCurrentStart;
                float curve = 0.15f;
                if (i == 1)
                {
                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1)) *
                            (1.0f - curve);
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1)) *
                            (1.0f - curve);

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1);
                }
                else if (i == canyonValues.CanyonForm.Count)
                {

                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1)) *
                            (1.0f - curve);
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1)) *
                            (1.0f - curve);

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1) +
                            (currentDirection * curve);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1) +
                            (lastCurrentDirection * curve);
                }
                else
                {
                    currentDirection =
                            (GetPointFromPlane(canyonValues, i) -
                            GetPointFromPlane(canyonValues, i - 1));
                    lastCurrentDirection =
                            (GetPointFromPlane(lastCanyonSegment.CanyonValues, i) -
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1));

                    currentStart =
                            LocalPosition +
                            GetPointFromPlane(canyonValues, i - 1) +
                            (currentDirection * curve);
                    lastCurrentStart =
                            lastCanyonSegment.PlaneCoordinates +
                            GetPointFromPlane(lastCanyonSegment.CanyonValues, i - 1) +
                            (lastCurrentDirection * curve);

                    currentDirection *= 1.0f - 2 * curve;
                    lastCurrentDirection *= 1.0f - 2 * curve;
                }
                createCanyonBoxes(lastCurrentStart, currentStart, lastCurrentDirection, currentDirection, 1,false);

            }
            CreateSkyCollision();
        }

        private Material material = null;

        private void createCanyonBoxes(Vector3 currentStart, Vector3 nextCurrentStart, Vector3 direction, Vector3 nextDirection, int boxCount, bool skytrigger)
        {
            
            Vector3[] lines = new Vector3[boxCount + 1];
            
            for (int i = 0; i < boxCount + 1; i++)
            {
                lines[i] = (nextCurrentStart + nextDirection / (boxCount) * i) - (currentStart + direction / (boxCount) * i); 
            }
            for (int i = 0; i < boxCount; i++)
            {
                Vector3 absP = currentStart + direction / (boxCount) * (i);
                Vector3 relP1 = direction / (boxCount) * (0.5f);
                Vector3 absP1 = absP + relP1;
                Vector3 P2    = nextDirection / (boxCount) * (0.5f);
                Vector3 relP2 = lines[i] + nextDirection / (boxCount) * (0.5f) ;
                Vector3 absP2 = absP + relP2;
                Vector3 relP3 = (relP1 * 2) + lines[i+1] / 2;
                Vector3 absP3 = absP + relP3;
                
                //Vector3 p2tmp = nextCurrentStart + nextDirection / (boxCount) * (i + 0.5f);
                //Vector3 p2 = p2tmp - (currentStart + direction / (boxCount) * i);
                //Vector3 relP2 = nextDirection / (boxCount) * (0.5f) + nextCurrentStart - currentStart;
                //Vector3 relP3 = lines[i+1] / 2;
                
                Vector3 test = MathUtil.Project(Vector3.Right, new Vector3(1, 1, 1));
                Vector3 test1 = MathUtil.Project(Vector3.Right, new Vector3(2, 2, 2));
                Vector3 test2 = MathUtil.Project(new Vector3(1, 1, 1) + Vector3.Up, Vector3.Right + Vector3.Up);
                
                Vector3 prj1 = MathUtil.Project(lines[i], relP1);
                Vector3 prj2 = MathUtil.Project(lines[i], relP2);
                Vector3 prj3 = MathUtil.Project(lines[i], relP3);

                Vector3 widthv = prj3 - relP3;
                Vector3 lengthv = prj2 - prj1;
                float width = widthv.Length() * 1.1f;
                float length = lengthv.Length() * 0.71f;
                float height = 25;

                Vector3 pos = absP + relP1 + (relP2 - relP1) / 2;
                Vector3 forward = relP2 - relP1;
                Vector3 up = Vector3.Normalize(Vector3.Cross(forward, relP1 + P2));
                
                //createBoxFromVectorsAndPostion(forward, up, width, pos);
                //createBoxFromVectors((relP2 - relP1), width, height, length, pos);

                createBoxFromVectorsAndPostion(forward, length, up, height, width, pos, skytrigger);

               
            }
            
            /*
            float x = 40;
            float y = 10;
            float z = 80;

            Vector3 dir1 = new Vector3(1, 1, 3);
            Vector3 pos1 = new Vector3(-800, 0, 0);
            createBoxFromVectors(dir1, x, y, z, pos1);

            Vector3 dir2 = new Vector3(3, 1, -1);
            Vector3 pos2 = new Vector3(-600, 0, 0);
            createBoxFromVectors(dir2, x, y, z, pos2);

            Vector3 dir3 = new Vector3(-1, 1, -3);
            Vector3 pos3 = new Vector3(-400, 0, 0);
            createBoxFromVectors(dir3, x, y, z, pos3);

            Vector3 dir4 = new Vector3(-3, 1, 1);
            Vector3 pos4 = new Vector3(-200, 0, 0);
            createBoxFromVectors(dir4, x, y, z, pos4);

            Vector3 dir5 = new Vector3(1, -1, 3);
            Vector3 pos5 = new Vector3(0, 0, 0);
            createBoxFromVectors(dir5, x, y, z, pos5);

            Vector3 dir6 = new Vector3(3, -1, -1);
            Vector3 pos6 = new Vector3(200, 0, 0);
            createBoxFromVectors(dir6, x, y, z, pos6);

            Vector3 dir7 = new Vector3(-1, -1, -3);
            Vector3 pos7 = new Vector3(400, 0, 0);
            createBoxFromVectors(dir7, x, y, z, pos7);

            Vector3 dir8 = new Vector3(-3, -1, 1);
            Vector3 pos8 = new Vector3(600, 0, 0);
            createBoxFromVectors(dir8, x, y, z, pos8);
           */
            /*
                        XnaDevRu.Physics.BoxShapeData box = new BoxShapeData();
                        box.ContactGroup = (int)Engine.Physics.ContactGroup.Canyon;
                        Vector3 width = (direction + nextDirection) / 2;
                        Vector3 length = canyonValues.Direction;
                        Vector3 height = Vector3.Cross(width,length);
                        box.Dimensions = new Vector3(width.Length, length, height);
                        //box.Offset = Matrix.Transform(Matrix.CreateTranslation(currentStart),Quaternion.
              */
        }

        public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix result)
        {
            Vector3 right;

            // Normalize forward vector
            Vector3.Normalize(ref forward, out forward);

            // Calculate right vector 
            Vector3.Cross(ref forward, ref up, out right);
            Vector3.Normalize(ref right, out right);

            // Recalculate up vector
            Vector3.Cross(ref right, ref forward, out up);
            Vector3.Normalize(ref up, out up);

            result.M11 = right.X;
            result.M12 = right.Y;
            result.M13 = right.Z;

            result.M21 = up.X;
            result.M22 = up.Y;
            result.M23 = up.Z;

            result.M31 = -forward.X;
            result.M32 = -forward.Y;
            result.M33 = -forward.Z;

            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;
        }

        public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        {
            Vector3 right;

            // Normalize forward vector
            forward.Normalize();
            up.Normalize();

            // Calculate right vector 
            Vector3.Cross(ref forward, ref up, out right);
            Vector3.Normalize(ref right, out right);

            // Recalculate up vector
            Vector3.Cross(ref right, ref forward, out up);
            Vector3.Normalize(ref up, out up);

            Matrix result = Matrix.Identity;

            result.M11 = right.X;
            result.M12 = right.Y;
            result.M13 = right.Z;

            result.M21 = up.X;
            result.M22 = up.Y;
            result.M23 = up.Z;

            result.M31 = -forward.X;
            result.M32 = -forward.Y;
            result.M33 = -forward.Z;

            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        

        private void CreateSkyCollision()
        {

            Vector3 currentStart;
            Vector3 currentDirection;
            Vector3 lastCurrentDirection;
            Vector3 lastCurrentStart;
            currentDirection =
                            (GetPointFromPlane(canyonValues, canyonValues.CanyonForm.Count - 1) -
                            GetPointFromPlane(canyonValues, 0));
            lastCurrentDirection =
                    (GetPointFromPlane(lastCanyonSegment.CanyonValues, lastCanyonSegment.canyonValues.CanyonForm.Count - 1) -
                    GetPointFromPlane(lastCanyonSegment.CanyonValues, 0));

            currentStart =
                    LocalPosition +
                    GetPointFromPlane(canyonValues, 0);
            lastCurrentStart =
                    lastCanyonSegment.PlaneCoordinates +
                    GetPointFromPlane(lastCanyonSegment.CanyonValues, 0);
            //Vector3 p1 = LocalPosition + GetPointFromPlane(canyonValues, 0);
            //Vector3 p2 = LocalPosition + GetPointFromPlane(canyonValues, canyonValues.CanyonForm.Count - 1);
            //Vector3 p3 = lastCanyonSegment.PlaneCoordinates + GetPointFromPlane(lastCanyonSegment.canyonValues, 0);
            //Vector3 p4 = lastCanyonSegment.PlaneCoordinates + GetPointFromPlane(lastCanyonSegment.canyonValues, lastCanyonSegment.canyonValues.CanyonForm.Count - 1);

            //Vector3 p1p2 = p2 - p1;
            //Vector3 p3p4 = p4 - p3;
            createCanyonBoxes(lastCurrentStart, currentStart, lastCurrentDirection, currentDirection, 1, true);
           //createCanyonBoxes(p3, p1, p3p4, p1p2, 1, false);

        }


        public void createBoxFromVectorsAndPostion(Vector3 forward, float forwardlength, Vector3 up, float uplength, float side, Vector3 pos, bool skytrigger)
        {
            XnaDevRu.Physics.BoxShapeData box = new BoxShapeData();
            box.Dimensions = new Vector3(side, uplength, forwardlength) * 1.42f;

            Matrix offset = CreateWorld(pos - LocalPosition, forward, up);
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(offset);

            offset = Matrix.CreateFromQuaternion(Quaternion.Inverse(rotation)) * Matrix.CreateTranslation(offset.Translation);
            box.Offset = offset;
            box.Material.Friction = 0.5f;
            box.Material.Bounciness = 1.0f;
            box.Material.Hardness = 1.0f;

            if (skytrigger)
            {
                skybox = box; 
            }
            else
            {
                this.boundingBoxes.Add(box);
            }
            
        }
        BoxShapeData skybox;
        public void LoadSkyBox()
        {
            //Model.CollisionShapes.Add(skybox);
            //AddShape(skybox);

            GameObject skyCollision = new GameObject(game);
            
            skyCollision.ConnectedToXpa = true;
            //skyCollision.Parent = this;
            skyCollision.ContactGroup = ContactGroup.Sky;
            skyCollision.AddShape(skybox);
            skyCollision.LocalPosition = this.GlobalPosition + Vector3.UnitY * 70;
            skyCollision.CanyonSegment = globalIndex;
            skyCollision.Static = true;
            game.World.AddObject(skyCollision);

            // TODO: Add SkyCollision Object .. Christian
        }

        public void LoadBoundingBoxes()
        {
            foreach (XnaDevRu.Physics.BoxShapeData box in this.boundingBoxes)
            {
                Model.CollisionShapes.Add(box);
                AddShape(box);
            }
        }

        //public void createBoxFromVectors(Vector3 direction, float x, float y, float z, Vector3 pos)
        //{
        //    XnaDevRu.Physics.BoxShapeData box = new BoxShapeData();
        //    box.Dimensions = new Vector3(x, y, z) * 1.42f;

        //    //Projektionen auf die Koordinatenebenen
        //    Vector3 prjxy = new Vector3(Vector3.Dot(Vector3.Right, direction), Vector3.Dot(Vector3.Up, direction), 0);
        //    Vector3 prjxz = new Vector3(Vector3.Dot(Vector3.Right, direction), 0, Vector3.Dot(Vector3.Backward, direction));
        //    Vector3 prjyz = new Vector3(0, Vector3.Dot(Vector3.Up, direction), Vector3.Dot(Vector3.Backward, direction));

        //    //Einschließende Winkel
        //    float yawDegree;
        //    if (prjxz.X > 0 )
        //    {
        //        yawDegree = MathUtil.AngleBetween(prjxz, Vector3.Backward);
        //    }
        //    else
        //    {
        //        yawDegree = MathUtil.AngleBetween(prjxz, Vector3.Forward);
        //    }
        //    float yaw = 0;// MathHelper.ToRadians(yawDegree);

        //    float pitchDegree;
        //    if (prjyz.Y > 0)
        //    {
        //        pitchDegree = MathUtil.AngleBetween(prjyz, Vector3.Backward);
        //    }
        //    else
        //    {
        //        pitchDegree = MathUtil.AngleBetween(prjyz, Vector3.Forward);
        //    }
        //    float pitch = MathHelper.ToRadians(pitchDegree);
        //    float rollDegree = 0;
        //    float roll = MathHelper.ToRadians(rollDegree);
        //    Matrix tmp = Matrix.Identity;// Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        //    tmp *= Matrix.CreateTranslation(pos - LocalPosition);
        //    box.Offset = tmp;
        //    Model.CollisionShapes.Add(box);
        //    AddShape(box);
        //}

        //public void createCanyonBoxes()
        //{
            /* collision = new MeshShapeData();
             collision.VertexArray = new Vector3[res.GetLength(0)];
             collision.TriangleArray = new int[index.GetLength(0)];
             for (int i = 0; i < res.GetLength(0); i++)
             {
                 collision.VertexArray[i] = res[i].Position;
             }
             for (int i = 0; i < index.GetLength(0); i++)
             {
                 collision.TriangleArray[i] = (int)index[i];
             }
            collision.VertexArray = new Vector3[3];
            collision.TriangleArray = new int[3];
            collision.VertexArray[0] = new Vector3(-300, -300, -1200);
            collision.VertexArray[1] = new Vector3(-300, 300, -1200);
            collision.VertexArray[2] = new Vector3(300, -300, -1200);
            //collision.VertexArray[3] = new Vector3(300, 300, -1200);
            collision.TriangleArray[0] = 0;
            collision.TriangleArray[1] = 1;
            collision.TriangleArray[2] = 2;
            //collision.TriangleArray[3] = 2;
            //collision.TriangleArray[4] = 3;
            //collision.TriangleArray[5] = 1;
            collision.ContactGroup = (int)Engine.Physics.ContactGroup.Canyon;
            Model.CollisionShapes.Add(collision);
            AddShape(collision);*/
        //}

        public void LoadSegmentPoints()
        {
            Initialize();
            InitializePointList();
            fields.CreateNoise();
            fields.ConnectPointsToLastSegement();
        }


        public void LoadCanyonModel()
        {
            fields.Next = nextCanyonSegement.Fields;
            fields.CalculateTexture();
            fields.CalculateNormals();
            fields.Create(out vertexArray, out index, out vertexCount, out indexCount);

            for(int i = 0; i < vertexArray.Length; i++)
            {
                vertexArray[i].Position -= LocalPosition;
            }
            //if (var == 4)
            //{
            //    collision = new MeshShapeData();
            //    //collision.ContactGroup = ((var % 2) + 1) * 2;

            //    collision.VertexArray = new Vector3[res.GetLength(0)];
            //    collision.TriangleArray = new int[index.GetLength(0)];
            //    for (int i = 0; i < res.GetLength(0); i++)
            //    {
            //        collision.VertexArray[i] = res[i].Position;
            //    }
            //    for (int i = 0; i < index.GetLength(0); i++)
            //    {
            //        collision.TriangleArray[i] = (int)index[i];
            //    }
            //    Solid.AddShape(collision);
            //}
            //var += 1;
            material = Material.Create(game, "Canyon", InstancingType.None);
            InitModel();
            base.LoadContent();
        }






        #region ICanyon Members

        public bool Generate(SegmentDescription data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}


