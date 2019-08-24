using System;
using System.Collections.Generic;
using CanyonShooter.GameClasses.World.Canyon;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World
{
    public class StaticsFactory
    {
        private struct Triangle
        {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;
            public Vector3 normal;
            public float area;
        }

        public static void createWithRandomRotation(ICanyonShooterGame game, Segment segment, string modelName, float density, float minSize, float maxSize)
        {
            Random random = new Random();

            List<Vector3> positions = FindPositions(segment, game, density);
            
            if (positions.Count > 0)
            {
                float[] scales = new float[positions.Count];
                Quaternion[] rotations = new Quaternion[positions.Count];

                for (int i = 0; i < scales.Length; i++)
                {
                    scales[i] = minSize + (maxSize - minSize)*(float) random.NextDouble();
                    rotations[i] =
                        Quaternion.CreateFromYawPitchRoll((float) random.NextDouble(), (float) random.NextDouble(),
                                                          (float) random.NextDouble());
                }

                game.World.AddObject(new Statics(game, segment, modelName, positions.ToArray(), rotations, scales, false));
            }
        }

        public static void createWithRandomAxisRotation(ICanyonShooterGame game, Segment segment, string modelName, float density, float minSize, float maxSize, Vector3 rotationAxis, float minRotationAngle, float maxRotationAngle)
        {
            Random random = new Random();

            List<Vector3> positions = FindPositions(segment, game, density);

            if (positions.Count > 0)
            {
                float[] scales = new float[positions.Count];
                Quaternion[] rotations = new Quaternion[positions.Count];

                for (int i = 0; i < scales.Length; i++)
                {
                    scales[i] = minSize + (maxSize - minSize)*(float) random.NextDouble();

                    float angle = minRotationAngle + (maxRotationAngle - minRotationAngle)*(float) random.NextDouble();
                    rotations[i] = Quaternion.CreateFromAxisAngle(rotationAxis, angle);
                }

                game.World.AddObject(new Statics(game, segment, modelName, positions.ToArray(), rotations, scales, false));
            }
        }

        private static List<Vector3> FindPositions(Segment segment, ICanyonShooterGame game, float density)
        {
            Random random = new Random();

            List<Triangle> triangles = new List<Triangle>();

            float minArea = float.PositiveInfinity;
            float sumArea = 0;

            float minU = float.PositiveInfinity;
            float minV = float.PositiveInfinity;
            float maxU = 0;
            float maxV = 0;

            for (int id = 0; id < segment.Index.Length / 3; id++)
            {
                Segment.VertexPositionNormalBinormalTangentTexture v0 = segment.VertexArray[segment.Index[id * 3 + 0]];
                Segment.VertexPositionNormalBinormalTangentTexture v1 = segment.VertexArray[segment.Index[id * 3 + 1]];
                Segment.VertexPositionNormalBinormalTangentTexture v2 = segment.VertexArray[segment.Index[id * 3 + 2]];

                Vector3 cross = Vector3.Cross(v1.Position - v0.Position, v2.Position - v0.Position);
                float area = cross.Length();

                Triangle triangle;
                triangle.a = v0.Position + segment.GlobalPosition;
                triangle.b = v1.Position + segment.GlobalPosition;
                triangle.c = v2.Position + segment.GlobalPosition;
                triangle.area = area;
                triangle.normal = -Vector3.Normalize(cross);

                triangles.Add(triangle);

                minArea = Math.Min(minArea, area);
                sumArea += area;


                minU = Math.Min(minU, v0.TextureCoordinate.X);
                minU = Math.Min(minU, v1.TextureCoordinate.X);
                minU = Math.Min(minU, v2.TextureCoordinate.X);
                minV = Math.Min(minV, v0.TextureCoordinate.Y);
                minV = Math.Min(minV, v1.TextureCoordinate.Y);
                minV = Math.Min(minV, v2.TextureCoordinate.Y);

                maxU = Math.Max(maxU, v0.TextureCoordinate.X);
                maxU = Math.Max(maxU, v1.TextureCoordinate.X);
                maxU = Math.Max(maxU, v2.TextureCoordinate.X);
                maxV = Math.Max(maxV, v0.TextureCoordinate.Y);
                maxV = Math.Max(maxV, v1.TextureCoordinate.Y);
                maxV = Math.Max(maxV, v2.TextureCoordinate.Y);
            }

            List<Vector3> positions = new List<Vector3>();

            double carry = 0;

            foreach (Triangle triangle in triangles)
            {
                double count = density * (carry + (double)triangle.area) / (double)sumArea;

                double residuum = count - Math.Truncate(count);

                carry = residuum*(double) sumArea;

                for (int id = 0; id < (int)count; id++)
                {
                    if (Vector3.Dot(triangle.normal, new Vector3(0, 1, 0)) < 0.7) continue;

                    float r1 = (float)random.NextDouble();
                    float r2 = (float)random.NextDouble();
                    float lambda0 = Math.Min(r1, r2);
                    float lambda1 = Math.Max(r1, r2) - lambda0;
                    float lambda2 = 1 - lambda0 - lambda1;

                    Vector3 point = lambda0 * triangle.a + lambda1 * triangle.b + lambda2 * triangle.c;

                    positions.Add(point);
                }
            }

            //System.Diagnostics.Debug.Print("id: " + segmentId + "  sumArea: " + sumArea + "  minArea: " + minArea + "  triangles: " + triangles.Count + "  count: " + positions.Count);

            return positions;
        }
    }
}
