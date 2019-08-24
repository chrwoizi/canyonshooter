using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// Math help functions
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// translates degree angle to (-180, 180]
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float NormalizeDegrees(float degrees)
        {
            while (degrees > 180.0)
            {
                degrees -= 360.0f;
            }
            while (degrees <= -180.0f)
            {
                degrees += 360.0f;
            }
            return degrees;
        }

        /// <summary>
        /// Returns rotations around x-, y-, and z- axis.
        ///  euler angles are in degrees
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static Vector3 GetEulerAngles(Matrix mat)
        {
            Vector3 angles;

            angles.Y = (float)Math.Asin(mat.M31);

            if (angles.Y < Globals.HalfPI)
            {
                if (angles.Y > -Globals.HalfPI)
                {
                    angles.X = (float)Math.Atan2(-mat.M32, mat.M33);
                    angles.Z = (float)Math.Atan2(-mat.M21, mat.M11);
                }
                else
                {
                    // This is not a unique solution.
                    float value = (float)Math.Atan2(mat.M12, mat.M22);
                    angles.Z = 0;
                    angles.X = angles.Z - value;
                }
            }
            else
            {
                // This is not a unique solution.
                float value = (float)Math.Atan2(mat.M12, mat.M22);
                angles.Z = 0;
                angles.X = value - angles.Z;
            }

            // convert to degrees
            angles.X = MathHelper.ToDegrees(angles.X);
            angles.Y = MathHelper.ToDegrees(angles.Y);
            angles.Z = MathHelper.ToDegrees(angles.Z);

            // normalize to (-180,180]
            angles.X = NormalizeDegrees(angles.X);
            angles.Y = NormalizeDegrees(angles.Y);
            angles.Z = NormalizeDegrees(angles.Z);

            return angles;

        }

        /// <summary>
        /// sets rotation of the matrix
        /// </summary>
        /// <param name="mat">matrix to rotate</param>
        /// <param name="degrees">angle of rotation</param>
        /// <param name="x">x-size of the axis vector</param>
        /// <param name="y">y-size of the axis vector</param>
        /// <param name="z">z-size of the axis vector</param>
        public static void MakeRotation(ref Matrix mat, float degrees, float x, float y, float z)
        {
            throw new Exception("CANYONSHOOTER TEAM: Do not use this method. It has a bug. Use SetQuaternion instead.");

            Vector3 axis = new Vector3(x, y, z);
            axis.Normalize();
            x = axis.X;
            y = axis.Y;
            z = axis.Z;

            // source: http://www.euclideanspace.com/maths/geometry/rotations/
            // conversions/angleToMatrix/
            float radians = MathHelper.ToRadians(degrees);
            float c = (float)Math.Cos(radians);
            float s = (float)Math.Sin(radians);
            float t = 1.0f - c;

            //				( *this ) ( 0, 0 ) = c + x * x * t;
            mat.M11 = c + x * x * t;
            //				( *this ) ( 1, 1 ) = c + y * y * t;
            mat.M22 = c + y * y * t;
            //				( *this ) ( 2, 2 ) = c + z * z * t;
            mat.M33 = c + z * z * t;

            float tmp1 = x * y * t;
            float tmp2 = z * s;

            //              ( *this ) ( 1, 0 ) = tmp1 + tmp2;
            mat.M21 = tmp1 + tmp2;
            //				( *this ) ( 0, 1 ) = tmp1 - tmp2;
            mat.M12 = tmp1 - tmp2;

            tmp1 = x * z * t;
            tmp2 = y * s;

            //				( *this ) ( 2, 0 ) = tmp1 - tmp2;
            mat.M31 = tmp1 - tmp2;
            //				( *this ) ( 0, 2 ) = tmp1 + tmp2;
            mat.M13 = tmp1 + tmp2;

            tmp1 = y * z * t;
            tmp2 = x * s;

            //				( *this ) ( 2, 1 ) = tmp1 + tmp2;
            mat.M32 = tmp1 + tmp2;
            //				( *this ) ( 1, 2 ) = tmp1 - tmp2;
            mat.M23 = tmp1 - tmp2;

            //				( *this ) ( 0, 3 ) = 0;
            mat.M14 = 0;
            //				( *this ) ( 1, 3 ) = 0;
            mat.M24 = 0;
            //				( *this ) ( 2, 3 ) = 0;
            mat.M34 = 0;

            //				( *this ) ( 3, 0 ) = 0;
            mat.M41 = 0;
            //				( *this ) ( 3, 1 ) = 0;
            mat.M42 = 0;
            //				( *this ) ( 3, 2 ) = 0;
            mat.M43 = 0;
            //				( *this ) ( 3, 3 ) = 1;
            mat.M44 = 1;
        }

        /// <summary>
        /// overwrites previous rotations of the matrix
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="degrees">angle of rotation</param>
        /// <param name="x">x-size of the axis vector</param>
        /// <param name="y">y-size of the axis vector</param>
        /// <param name="z">z-size of the axis vector</param>
        public static void SetRotation(ref Matrix mat, float degrees, float x, float y, float z)
        {
            throw new Exception("CANYONSHOOTER TEAM: Do not use this method. It has a bug. Use SetQuaternion instead.");

            Vector3 p = mat.Translation;
            MakeRotation(ref mat, degrees, x, y, z);
            mat.Translation = p;
        }

        /// <summary>
        /// x, y, z - axis can be zero
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="w"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetQuaternion(ref Matrix mat, float w, float x, float y, float z)
        {
            // CANYONSHOOTER EDIT BEGIN

            // auskommentiert
            /*Quaternion q = new Quaternion(x, y, z, w);
            Vector3 axis = Vector3.Zero;
            float angle = GetAngleAxis(q, ref axis);
            SetRotation(ref mat, angle, axis.X, axis.Y, axis.Z);*/

            // hinzugefügt
            Vector3 translation = mat.Translation;
            mat = Matrix.CreateFromQuaternion(new Quaternion(x, y, z, w));
            mat.Translation = translation;

            // CANYONSHOOTER EDIT END
        }

        public static float GetAngleAxis(Quaternion q, ref Vector3 axis)
        {
            float angle;
            float sqrLen = q.X * q.X + q.Y * q.Y + q.Z * q.Z;

            if (sqrLen > 0)
            {
                angle = 2 * (float)Math.Acos(q.W);
                float invLen = 1 / (float)Math.Sqrt(sqrLen);
                axis.X = q.X * invLen;
                axis.Y = q.Y * invLen;
                axis.Z = q.Z * invLen;
            }
            else
            {
                angle = 0;
                axis.X = 1;
                axis.Y = 0;
                axis.Z = 0;
            }

            // convert to degrees
            angle = MathHelper.ToDegrees(angle);

            return angle;
        }

        /// <summary>
        /// Returns true if the two values are equal within some tolerance,
        /// using a combination of absolute and relative (epsilon is scaled
        /// by the magnitudes of the values) tolerance, depending on whether
        /// both values are both less than 1.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool AreEqual(float x, float y)
        {
            float maxVal = 1;
            float xAbs = Math.Abs(x);
            float yAbs = Math.Abs(y);

            if (xAbs > maxVal)
                maxVal = xAbs;

            if (yAbs > maxVal)
                maxVal = yAbs;


            if (Math.Abs(x - y) <= Globals.Epsilon * maxVal)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns angle in degrees between two prenormalized vectors
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float AngleBetweenPreNorm(Vector3 u, Vector3 v)
        {
            float val = Vector3.Dot(u, v);

            // Clamp to avoid rounding errors; acos will puke with values outside
            // this range.
            if (val < -1)
            {
                val = -1;
            }
            else if (val > 1)
            {
                val = 1;
            }

            return MathHelper.ToDegrees((float)Math.Acos(val));
        }

        /// <summary>
        /// Returns angle in degrees between two vectors
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float AngleBetween(Vector3 u, Vector3 v)
        {
            u.Normalize();
            v.Normalize();
            return AngleBetweenPreNorm(u, v);
        }

        public static Vector3 Project(Vector3 u, Vector3 v)
        {
            u.Normalize();
            return ProjectPreNorm(u, v);
        }

        public static Vector3 ProjectPreNorm(Vector3 u, Vector3 v)
        {
            return Vector3.Dot(u, v) * u;
        }

        /// <summary>
        /// returns true if two given vectors are colleniar
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool AreCollinear(Vector3 u, Vector3 v)
        {
            u.Normalize();
            v.Normalize();

            if (AreEqual(u.X, v.X) && AreEqual(u.Y, v.Y) && AreEqual(u.Z, v.Z))
                return true;

            if (AreEqual(u.X, -v.X) && AreEqual(u.Y, -v.Y) && AreEqual(u.Z, -v.Z))
                return true;

            return false;
        }

        /// <summary>
        /// Transfroms a Ray by given matrix
        /// </summary>
        /// <param name="mat">transform matrix</param>
        /// <param name="ray">ray to transform</param>
        /// <returns>Transformed ray</returns>
        public static Ray TransformRay(Matrix mat, Ray ray)
        {
            Ray result = new Ray(Vector3.Transform(ray.Position, mat), Vector3.TransformNormal(ray.Direction, mat));

            return result;
        }
    }
}
