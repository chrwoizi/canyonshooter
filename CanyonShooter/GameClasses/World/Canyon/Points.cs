using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.World.Canyon
{
    public class Points
    {
        //VertexPositionNormalTexture[,] vertexPoints;
        List<Vector3[,]> plains;
        Vector3[,] completePlain;
        Vector2[,] texture;
        Vector3[,] normal;

        public Vector3[,] CompletePlain
        {
            get { return completePlain; }
            set { completePlain = value; }
        }

        Points last = null;

        internal Points Last
        {
            get { return last; }
            set { last = value; }
        }
        Points next = null;

        internal Points Next
        {
            get { return next; }
            set { next = value; }
        }
        int pointLength;
        int pointWidth;
        private ICanyonShooterGame game = null;
        private bool heightmapping = true;
        private float innerDivisor = 4.4f;
        private float outerDivisor = 4.8f;

        //public Points(int width, int length, VertexPositionNormalTexture[,] vertexPoints)
        public Points(int pointLength, Points last, ICanyonShooterGame game)
        {
            this.last = last;
            plains = new List<Vector3[,]>();
            this.pointLength = pointLength;
            this.pointWidth = 0;

            this.game = game;
        }

        public void Add(Vector3[,] pointfield)
        {
            if (pointfield.GetLength(1) == pointLength)
            {
                plains.Add(pointfield);
                pointWidth += pointfield.GetLength(0);
            }
            else
            {
                //Exception...
            }
            
        }

        

        public int TotalSize()
        {
            return pointWidth * pointLength;
        }

        public int IndexSize()
        {
            return (pointWidth - 1) * (pointLength - 1) * 6;
        }

        public void CreateCompletePlain()
        {
            completePlain = new Vector3[pointWidth, pointLength];
            int currentwidth = 0;
            foreach (Vector3[,] plain in plains)
            {
                for (int i = 0; i < plain.GetLength(0); i++)
                {
                    for (int j = 0; j < plain.GetLength(1); j++)
                    {
                        completePlain[currentwidth + i, j] = plain[i,j];
                    }
                }
                currentwidth += plain.GetLength(0);
            }
        }

        public void CalculateTexture()
        {
            texture = new Vector2[completePlain.GetLength(0), completePlain.GetLength(1)];
            // Berechnung der v-Richtung
            for (int i = 0; i < completePlain.GetLength(0); i++)
            {
                float reallength = 0;
                for (int j = 1; j < completePlain.GetLength(1); j++)
                {
                    reallength += Vector3.Distance(completePlain[i,j],completePlain[i,j-1]);
                }
                float currentlength = 0;
                for (int j = 0; j < completePlain.GetLength(1); j++)
                {
                    if (j == 0)
                    {
                        texture[i, j] = new Vector2(0, currentlength / reallength);
                    }
                    else
                    {
                        currentlength += Vector3.Distance(completePlain[i, j], completePlain[i, j - 1]);
                        texture[i, j] = new Vector2(0, currentlength / reallength);
                    }
                }
            }

            // Berechnung der u-Richtung
            for (int i = 0; i < completePlain.GetLength(1); i++)
            {
                float realwidth = 0;
                for (int j = 1; j < completePlain.GetLength(0); j++)
                {
                    realwidth += Vector3.Distance(completePlain[j, i], completePlain[j - 1, i]);
                }
                float currentwidth = 0;
                for (int j = 0; j < completePlain.GetLength(0); j++)
                {
                    if (j == 0)
                    {
                        texture[j, i].X = currentwidth / realwidth;
                    }
                    else
                    {
                        currentwidth += Vector3.Distance(completePlain[j, i], completePlain[j - 1, i]);
                        texture[j, i].X = currentwidth / realwidth;
                    }
                }
            }
        }

        public void CalculateNormals()
        {
            normal = new Vector3[completePlain.GetLength(0), completePlain.GetLength(1)];
            //Wenn next == null -> exception
            Vector3 pForward = Vector3.Zero;
            Vector3 pForwardLeft = Vector3.Zero;
            Vector3 pLeft = Vector3.Zero;
            Vector3 pBack = Vector3.Zero;
            Vector3 pBackRight = Vector3.Zero;
            Vector3 pRight = Vector3.Zero;
            Vector3 pThis = Vector3.Zero; 
            for (int i = 0; i < completePlain.GetLength(0); i++)
            {
                for (int j = 0; j < completePlain.GetLength(1); j++)
                {
                    Vector3 n1 = Vector3.Zero;
                    Vector3 n2 = Vector3.Zero;
                    Vector3 n3 = Vector3.Zero;
                    Vector3 n4 = Vector3.Zero;
                    Vector3 n5 = Vector3.Zero;
                    Vector3 n6 = Vector3.Zero;
                    pThis = completePlain[i, j];
                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            //Linker vorderer Eckpunkt
                            pForward = completePlain[i,j+1];
                            pRight = completePlain[i+1,j];
                            if (last != null)
                            {
                                pBackRight = last.CompletePlain[1, last.CompletePlain.GetLength(1) - 2];
                                pBack = last.CompletePlain[0, last.CompletePlain.GetLength(1) - 2];
                                n1 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                                n2 = Vector3.Cross(pBackRight - pThis, pRight - pThis); 
                            } 
                            n3 = Vector3.Cross(pRight - pThis, pForward - pThis); 
                        }
                        else if (j == (completePlain.GetLength(1) - 1))
                        {
                            //Linker hinterer Eckpunkt
                            
                            pRight = completePlain[i + 1, j];
                            pBackRight = completePlain[i + 1, j - 1];
                            pBack = completePlain[i, j - 1];
                            if (next != null)
                            {
                                pForward = next.CompletePlain[0, 1];
                                n1 = Vector3.Cross(pRight - pThis, pForward - pThis);                                
                            }
                            n2 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                            n3 = Vector3.Cross(pBackRight - pThis, pRight - pThis);
                        }
                        else
                        {
                            //Linke Kante ohne Eckpunkte
                            pForward = completePlain[i, j + 1];
                            pRight = completePlain[i + 1, j];
                            pBackRight = completePlain[i + 1, j - 1];
                            pBack = completePlain[i, j - 1];
                            n1 = Vector3.Cross(pRight - pThis, pForward - pThis);
                            n2 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                            n3 = Vector3.Cross(pBackRight - pThis, pRight - pThis);
                        }                  
                    }
                    else if (i == (completePlain.GetLength(0) - 1))
                    {
                        if (j == 0)
                        {
                            //Rechter vorderer Eckpunkt
                            pForward = completePlain[i, j + 1];
                            pForwardLeft = completePlain[i - 1, j + 1];
                            pLeft = completePlain[i - 1, j];
                            if (last != null)
                            {
                                pBack = last.CompletePlain[last.CompletePlain.GetLength(0) - 1, last.CompletePlain.GetLength(1) - 2];
                                n1 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                            }
                            n2 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                            n3 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                            
                            
                        }
                        else if (j == (completePlain.GetLength(1) - 1))
                        {
                            //Rechter hinterer Eckpunkt
                            pLeft = completePlain[i - 1, j];
                            pBack = completePlain[i, j - 1];
                            if (next != null)
                            {
                                pForward = next.CompletePlain[next.CompletePlain.GetLength(0) - 1, 1];
                                pForwardLeft = next.CompletePlain[next.CompletePlain.GetLength(0) - 2, 1];
                                n1 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                                n2 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                            }                            
                            n3 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                        }
                        else
                        {
                            //Rechte Kante
                            pForward = completePlain[i, j + 1];
                            pForwardLeft = completePlain[i - 1, j + 1];
                            pLeft = completePlain[i - 1, j];   
                            pBack = completePlain[i, j - 1];
                            n1 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                            n2 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                            n3 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            //Vordere Kante
                            pForward = completePlain[i, j + 1];
                            pForwardLeft = completePlain[i - 1, j + 1];
                            pLeft = completePlain[i - 1, j];
                            pRight = completePlain[i + 1, j];
                            if (last != null)
                            {
                                pBack = last.CompletePlain[i, last.CompletePlain.GetLength(1) - 2];
                                pBackRight = last.CompletePlain[i + 1, last.CompletePlain.GetLength(1) - 2];
                                n1 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                                n2 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                                n3 = Vector3.Cross(pBackRight - pThis, pRight - pThis);
                            }
                            
                            n4 = Vector3.Cross(pRight - pThis, pForward - pThis);
                            n5 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                            n6 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                        }
                        else if (j == (completePlain.GetLength(1) - 1))
                        {
                            //Hintere Kante                                       
                            pLeft = completePlain[i - 1, j];
                            pBack = completePlain[i, j - 1];
                            pBackRight = completePlain[i + 1, j - 1];
                            pRight = completePlain[i + 1, j];
                            if (next != null)
                            {
                                pForward = next.CompletePlain[i, 1];
                                pForwardLeft = next.CompletePlain[i - 1, 1];
                                n1 = Vector3.Cross(pRight - pThis, pForward - pThis);
                                n2 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                                n3 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                            }                            
                            n4 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                            n5 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                            n6 = Vector3.Cross(pBackRight - pThis, pRight - pThis);
                        }
                        else
                        {
                            pForward = completePlain[i, j + 1];
                            pForwardLeft = completePlain[i - 1, j + 1];
                            pLeft = completePlain[i - 1, j];
                            pBack = completePlain[i, j - 1];
                            pBackRight = completePlain[i + 1, j - 1];
                            pRight = completePlain[i + 1, j];
                            //Alle Punkte in der Mitte
                            n1 = Vector3.Cross(pRight - pThis, pForward - pThis);
                            n2 = Vector3.Cross(pForward - pThis, pForwardLeft - pThis);
                            n3 = Vector3.Cross(pForwardLeft - pThis, pLeft - pThis);
                            n4 = Vector3.Cross(pLeft - pThis, pBack - pThis);
                            n5 = Vector3.Cross(pBack - pThis, pBackRight - pThis);
                            n6 = Vector3.Cross(pBackRight - pThis, pRight - pThis);
                        }                        
                    }
                    normal[i, j] = Vector3.Normalize(n1 + n2 + n3 + n4 + n5 + n6);
                }
            }

        }

        public void CreateNoise()
        {
            //test redundanz
            CalculateTexture();
            //
            CalculateNormals();

            if (heightmapping)
            {
                Random rnd = new Random((int)DateTime.Now.Ticks);

                int hm = rnd.Next(0, 5);
            
                Texture2D tex = game.Content.Load<Texture2D>(".\\Content\\Textures\\heightmap\\hm" + hm);

                int HEIGHT = tex.Height;
                int WIDTH = tex.Width;

                Color[] data = new Color[WIDTH * HEIGHT];
                tex.GetData<Color>(data);

                int[,] height = new int[WIDTH, HEIGHT];

                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        height[WIDTH - 1 - i, HEIGHT - 1 - j] = data[j * WIDTH + i].R;
                    }
                }

                for (int i = 0; i < completePlain.GetLength(0); i++)
                {
                    for (int j = 0; j < completePlain.GetLength(1); j++)
                    {
                        //completePlain[i, j] += normal[i, j] *((height[i * 7, j * 6].R) / 3);
                        if (i > 7 && i < completePlain.GetLength(0) - 7)
                        {
                            completePlain[i, j] += normal[i, j] *
                                ((height[(int)(texture[i, j].X * (WIDTH - 1)), (int)(texture[i, j].Y * (HEIGHT - 1))]) / innerDivisor);
                        }
                        else
                        {
                            completePlain[i, j] += normal[i, j] *
                                ((height[(int)(texture[i, j].X * (WIDTH - 1)), (int)(texture[i, j].Y * (HEIGHT - 1))]) / outerDivisor);
                        }
                    }
                }
            }

        }


        public void ConnectPointsToLastSegement()
        {
            if (last != null)
            {
                for (int i = 0; i < completePlain.GetLength(0); i++)
                {
                    completePlain[i, 0] = last.CompletePlain[i, last.CompletePlain.GetLength(1) - 1];
                } 
            }
            
        }

        private Segment.VertexPositionNormalBinormalTangentTexture[] CreateVertexList()
        {
            Segment.VertexPositionNormalBinormalTangentTexture[] result = new Segment.VertexPositionNormalBinormalTangentTexture[this.TotalSize()];

            for (int i = 0; i < completePlain.GetLength(0); i++)
            {
                for (int j = 0; j < completePlain.GetLength(1); j++)
                {
                    Vector3 position = this.completePlain[i,j];

                    Vector3 normal = this.normal[i, j];

                    Vector2 texture = 0.5f * this.texture[i, j];

                    result[completePlain.GetLength(1) * i + j] = new Segment.VertexPositionNormalBinormalTangentTexture(position, normal, texture);
                }
            }
            return result;
        }
        /*
        private VertexPositionNormalTexture[] CreateVertexList2()
        {
            VertexPositionNormalTexture[] result = new VertexPositionNormalTexture[this.TotalSize()];
            int count = 0;

            foreach (Vector3[,] pointfield in plains)
	        {
                int width = pointfield.GetLength(0);
                int length = pointfield.GetLength(1);
			    for (int j = 0; j < width; j++)
                {
                    float reallength = 0;
                    float currentlength = 0;
                    for (int k = 1; k < length; k++)
                    {
                        reallength += Vector3.Distance(pointfield[j, k], pointfield[j, k - 1]);
                    }
                    for (int k = 0; k < length; k++)
                    {
                        Vector3 position;

                        if((last != null) && (k == 0))
                        {
                            position = last.plains[count][j,length-1];
                        }
                        else
                        {
                            position = pointfield[j, k];
                        }
                        
                        Vector3 normal = Vector3.Up;//TODO noch berechnen

                        if (k==0)
                        {
                            currentlength = 0;
                        }
                        else
                        {
                            currentlength += Vector3.Distance(pointfield[j, k], pointfield[j, k - 1]);
                        }
                        Vector2 texture = new Vector2(0, 0);
                        //Texturenberechnung v-richtung

                        texture += new Vector2(currentlength/reallength,0);
                        // Berechnung nach Punktmenge
                        //Vector2 texture = new Vector2(k / (float)(pointLength - 1), (count * width + j) / (float)(pointWidth - 1));

                        result[count * width * length + j * length + k] = new VertexPositionNormalTexture(position,normal,texture);
                    }
                }
                count += 1;
            }

            //Berechnung der Texturen in u-richtung
            for (int i = 0; i < pointLength; i++)
            {
                float realwidth = 0;
                float currentwidth = 0;
                for (int j = pointLength; j < this.TotalSize(); j += pointLength)
                {
                    realwidth += Vector3.Distance(result[j + i].Position, result[j - pointLength + i].Position);
                }
                for (int j = 0; j < this.TotalSize(); j+=pointLength)
                {
                    if (j==0)
	                {
		                currentwidth=0;
	                }
                    else
                    {
                        currentwidth += Vector3.Distance(result[j + i].Position, result[j - pointLength + i].Position);
                    }
                    result[i + j].TextureCoordinate += new Vector2(0,currentwidth / realwidth);           
                }
            }
            
            return result;
        }
        */

        private short[] CreateIndices()
        {
            short[] triangleListIndices = new short[this.IndexSize()];
            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < pointWidth - 1; i++)
            {
                for (int j = 0; j < pointLength - 1; j++)
                {
                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 0] = (short)(j + (i * pointLength));
                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 2] = (short)(j + ((i + 1) * pointLength));
                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 1] = (short)((j + 1) + (i * pointLength));

                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 3] = (short)(j + 1 + ((i + 1) * pointLength));
                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 5] = (short)((j + 1) + (i * pointLength));
                    triangleListIndices[(j + (i * (pointLength - 1))) * 6 + 4] = (short)(j + ((i + 1) * pointLength));

                }
            }
            return triangleListIndices;
        }

        public void Create(out Segment.VertexPositionNormalBinormalTangentTexture[] vertex, out short[] indices, out int vertexcount, out int trianglecount)
        {
            vertex = this.CreateVertexList();
            indices = this.CreateIndices();
            vertexcount = this.TotalSize();
            trianglecount = this.IndexSize() / 3;
        }
/*
        public VertexPositionNormalTexture[] getConstraintPoints()
        {
            VertexPositionNormalTexture[] result = new VertexPositionNormalTexture[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = this.vertexPoints[width - 1, i];
            }
            return result;
        }

        public static void CreatePointArray(LinkedList<Points> pointList, out VertexPositionNormalTexture[] result, out int pointCount)
        {
            int sumSize = 0;
            foreach (Points p in pointList)
            {
                sumSize += p.Size();
            }
            pointCount = sumSize;
            result = new VertexPositionNormalTexture[sumSize];
            int currentSize = 0;
            foreach (Points p in pointList)
            {
                for (int i = 0; i < p.width; i++)
			    {
                    for (int j = 0; j < p.length; j++)
                    {
                        result[currentSize + i * p.length + j] = p.vertexPoints[i, j];
                    }
			    }
                currentSize += p.Size();
            }
        }

        public static short[] CreateIndexBuffer(int pointsWidth,int pointsLength)
        {
            short[] triangleListIndices = new short[(pointsWidth - 1) * (pointsLength - 1) * 6];
            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < pointsLength - 1; i++)
            {
                for (int j = 0; j < pointsWidth - 1; j++)
                {
                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 0] = (short)(j + (i * pointsWidth));
                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 2] = (short)(j + ((i + 1) * pointsWidth));
                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 1] = (short)((j + 1) + (i * pointsWidth));

                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 3] = (short)(j + 1 + ((i + 1) * pointsWidth));
                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 5] = (short)((j + 1) + (i * pointsWidth));
                    triangleListIndices[(j + (i * (pointsWidth - 1))) * 6 + 4] = (short)(j + ((i + 1) * pointsWidth));

                }
            }
            return triangleListIndices;
        }


        public static void CreateIndexArray(LinkedList<Points> pointList, out short[] result,out int indexCount)
        {
            int indexSize = 0;

            foreach (Points p in pointList)
            {
                if (pointList.Last.Value == p)
                {
                    indexSize += p.IndexSize(); 
                }
                else
                {
                    indexSize += (p.Size()-p.width) * 6;
                }
                
            }
            result = new short[indexSize];
            indexCount = indexSize;
            int currentIndexSize = 0;
            int currentSize = 0;
            foreach (Points p in pointList)
            {
                if (pointList.Last.Value == p)
                {
                    for (int i = 0; i < p.width - 1; i++)
                    {
                        for (int j = 0; j < p.length - 1; j++)
                        {
                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 0] = (short)(currentSize + j + (i * p.length));
                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 2] = (short)(currentSize + j + ((i + 1) * p.length));
                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 1] = (short)(currentSize + (j + 1) + (i * p.length));

                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 3] = (short)(currentSize + j + 1 + ((i + 1) * p.length));
                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 5] = (short)(currentSize + (j + 1) + (i * p.length));
                            result[currentIndexSize + (j + (i * (p.length - 1))) * 6 + 4] = (short)(currentSize + j + ((i + 1) * p.length));

                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < p.width; i++)
                    {
                        for (int j = 0; j < p.length-1; j++)
                        {
                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 0] = (short)(currentSize + j + (i * p.length));
                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 2] = (short)(currentSize + j + ((i + 1) * p.length));
                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 1] = (short)(currentSize + (j + 1) + (i * p.length));

                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 3] = (short)(currentSize + j + 1 + ((i + 1) * p.length));
                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 5] = (short)(currentSize + (j + 1) + (i * p.length));
                            result[currentIndexSize + (j + (i * (p.length ))) * 6 + 4] = (short)(currentSize + j + ((i + 1) * p.length));

                        }
                    }
                }
                currentSize += p.Size();
                currentIndexSize += p.IndexSize();
            }

        }*/
    }
}
