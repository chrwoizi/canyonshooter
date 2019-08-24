using System;
using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace CanyonShooter.Engine.Helper
{
    public class Helper
    {
        /// <summary>
        /// This Dictionary holds each lock, and the time, when it is released again.
        /// </summary>
        private static Dictionary<string,DateTime> locks = new Dictionary<string, DateTime>();

        /// <summary>
        /// Locks the specified name for a given duration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>if a lock exists for the specified name [true], else [false]</returns>
        public static bool Lock(string name, TimeSpan duration)
        {
            if(locks.ContainsKey(name)) //this lock already exists
            {
                if(DateTime.Now >= locks[name]) //lock duration timed out
                {
                    locks[name] = DateTime.Now.Add(duration); //reassign lock
                    return false;

                }
                else
                    return true;
            }
            else
            {   // add lock
                locks.Add(name,DateTime.Now.Add(duration));
                return false;
            }
        }


        /// <summary>
        /// Waits for a durtation
        /// </summary>
        /// <param name="name">The namespace to wait</param>
        /// <param name="duration">The duration to wait.</param>
        /// <returns>true if duration timed out, else false (still waiting)</returns>
        public static bool WaitFor(string name, TimeSpan duration)
        {
            if (locks.ContainsKey(name)) //this lock already exists
            {
                if (DateTime.Now >= locks[name]) //lock duration timed out
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {   // add lock
                locks.Add(name, DateTime.Now.Add(duration));
                return false;
            }
        }

        /// <summary>
        /// Resets the wait.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void ResetWait(string name)
        {
            if (locks.ContainsKey(name)) //this lock already exists
                locks.Remove(name);
        }


        /// <summary>
        /// Vector3.Transform(front, result) == direction
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="front"></param>
        /// <returns></returns>
        public static Quaternion RotateTo(Vector3 direction, Vector3 front)
        {
            Debug.Assert(direction != Vector3.Zero);
            Debug.Assert(front != Vector3.Zero);

            if (front != direction)
            {
                direction.Normalize();
                front.Normalize();

                Vector3 rotationAxis = Vector3.Cross(front, direction);

                if (rotationAxis == Vector3.Zero)
                {
                    return Quaternion.Identity;
                }
                else
                {
                    rotationAxis.Normalize();
                    float angle = (float)Math.Acos(Vector3.Dot(direction, front));
                    return Quaternion.CreateFromAxisAngle(rotationAxis, angle);
                }
            }
            return Quaternion.Identity;
        }

        /// <summary>
        /// storage for time measurement methods
        /// </summary>
        private static readonly Dictionary<string, DateTime> timeMeasurements = new Dictionary<string, DateTime>();

        /// <summary>
        /// see EndTimeMeasurement
        /// </summary>
        private static readonly int OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN = -1;

        /// <summary>
        /// Begins a time measurement
        /// Does nothing if OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN < 0
        /// </summary>
        /// <param name="name"></param>
        public static void BeginTimeMeasurementDebugOutput(string name)
        {
            if (OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN >= 0)
                lock (timeMeasurements)
                {
                    if (!timeMeasurements.ContainsKey(name))
                        timeMeasurements.Add(name, DateTime.Now);
                }
        }

        /// <summary>
        /// Ends a time measurement and outputs the result if greater than OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN
        /// Does nothing if OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN < 0
        /// </summary>
        /// <param name="name"></param>
        public static void EndTimeMeasurementDebugOutput(string name)
        {
            if (OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN >= 0)
                lock (timeMeasurements)
                {
                    if (timeMeasurements.ContainsKey(name))
                    {
                        int ms = (DateTime.Now - timeMeasurements[name]).Milliseconds;
                        if (ms > OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN)
                        {
                            Debug.Print("time measurement: " + name + " = " + ms);
                        }
                        timeMeasurements.Remove(name);
                    }
                }
        }

        /// <summary>
        /// Begins a time measurement
        /// </summary>
        /// <param name="name"></param>
        public static void BeginTimeMeasurement(string name)
        {
            lock (timeMeasurements)
            {
                if (!timeMeasurements.ContainsKey(name))
                    timeMeasurements.Add(name, DateTime.Now);
            }
        }

        /// <summary>
        /// Ends a time measurement and outputs the result if greater than OUTPUT_TIME_MEASUREMENT_IF_GREATER_THAN
        /// </summary>
        /// <param name="name"></param>
        public static int EndTimeMeasurement(string name)
        {
            lock (timeMeasurements)
            {
                if (timeMeasurements.ContainsKey(name))
                {
                    int ms = (DateTime.Now - timeMeasurements[name]).Milliseconds;
                    timeMeasurements.Remove(name);
                    return ms;
                }
            }

            return 0;
        }

        /// <summary>
        /// creates the local rotation of a billboard
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="camera"></param>
        /// <param name="extraRotation"></param>
        /// <returns></returns>
        static public Quaternion BillboardRotation(ITransformable parent, Vector3 localPosition, ICamera camera, Quaternion extraRotation)
        {
            //Orthogonale Projektion des Betrachters auf die Gerade der Flugrichtung:
            //normalize(dir)
            //q = pos + dot( dir , ( cam - pos ) ) * dir
            //needed vector;
            //v = cam - q;

            Vector3 direction = -Vector3.UnitZ;
            Vector3 cameraPosition = parent == null ? camera.GlobalPosition : Vector3.Transform(camera.GlobalPosition, Matrix.Invert(parent.GlobalTransformation));
            cameraPosition = cameraPosition - localPosition;
            cameraPosition = Vector3.Transform(cameraPosition, Quaternion.Inverse(extraRotation));
            
            Vector3 projection = Vector3.Dot(direction, cameraPosition) * direction;
            Vector3 shortestVector = cameraPosition - projection;

            shortestVector.Normalize();

            float tmp = Vector3.Dot(shortestVector, Vector3.UnitX);
            float cosangle = Vector3.Dot(shortestVector, Vector3.UnitY);
            cosangle = Math.Min(cosangle, 1.0f);
            cosangle = Math.Max(cosangle, -1.0f);
            float angle = (float)Math.Acos(cosangle);
            if (tmp < 0) angle = -angle;

            return Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(direction, angle), extraRotation);
        }
    }
}
