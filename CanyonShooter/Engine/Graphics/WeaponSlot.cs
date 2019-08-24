using CanyonShooter.GameClasses.Weapons;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics
{
    public class WeaponSlot
    {
        #region Private Fields
        private WeaponType weaponType;
        private Vector3 position;
        private Vector3 rotationAxis;
        private float rotationAngle;
        private Vector3 scaling;
        #endregion


        public WeaponSlot(WeaponType weaponType, Vector3 position, Vector3 rotationAxis, float rotationAngle, Vector3 scaling)
        {
            this.weaponType = weaponType;
            this.position = position;
            this.rotationAxis = rotationAxis;
            this.rotationAngle = rotationAngle;
            this.scaling = scaling;
        }


        #region Properties

        public WeaponType WeaponType
        {
            get { return weaponType; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 RotationAxis
        {
            get { return rotationAxis; }
        }

        public float RotationAngle
        {
            get { return rotationAngle; }
        }

        public Vector3 Scaling
        {
            get { return scaling; }
        }

        #endregion
    }
}
