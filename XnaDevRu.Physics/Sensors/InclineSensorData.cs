using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing an InclineSensor.
    /// </summary>
    public class InclineSensorData : SensorData
    {
        /// <summary>
        /// The local rotation axis around which the angle of rotation will be measured.
        /// </summary>
        public Vector3 Axis;

        public InclineSensorData()
        {
            type = SensorType.Incline;
            Axis = Defaults.Sensor.Incline.Axis;
        }
    }
}
