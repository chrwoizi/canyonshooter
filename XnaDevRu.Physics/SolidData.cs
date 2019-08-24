using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a Solid.
    /// </summary>
    public class SolidData
    {
        #region fields
        private bool enabled;

        /// <summary>
        /// Determines whether the Solid is enabled.
        /// </summary>
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }
        
        private string name;

        /// <summary>
        /// An identifier for the Solid.
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private bool sleeping;

        /// <summary>
        /// Determines whether the Solid is sleeping.
        /// </summary>
        public bool Sleeping {
            get { return sleeping; }
            set { sleeping = value; }
        }


        private float sleepiness;

        /// <summary>
        /// The Solid's sleepiness level which determines how fast the 
        /// Solid falls asleep.
        /// </summary>
        public float Sleepiness {
            get { return sleepiness; }
            set { sleepiness = value; }
        }

        
        private bool isStatic;

        /// <summary>
        /// Determines whether the Solid is static.
        /// </summary>
        public bool IsStatic {
            get { return isStatic; }
            set { isStatic = value; }
        }

        /// <summary>
        /// The Solid's transform relative to the global origin.
        /// </summary>
        public Matrix Transform;

        /// <summary>
        /// The Solid's linear velocity in global coordinates.
        /// </summary>
        public Vector3 GlobalLinearVel;

        /// <summary>
        /// The Solid's angular velocity in global coordinates.
        /// </summary>
        public Vector3 GlobalAngularVel;

        private float linearDamping;

        /// <summary>
        /// The amount of damping applied to the Solid's linear motion.
        /// </summary>
        public float LinearDamping {
            get { return linearDamping; }
            set { linearDamping = value; }
        }


        private float angularDamping;

        /// <summary>
        /// The amount of damping applied to the Solid's angular motion.
        /// </summary>
        public float AngularDamping {
            get { return angularDamping; }
            set { angularDamping = value; }
        }

        /// Pointers to the Solid's Shape data.
        protected List<ShapeData> shapes;

        #endregion

        public SolidData()
        {
            shapes = new List<ShapeData>();

            Enabled = Defaults.Solid.Enabled;
            Name = "";
            Sleeping = Defaults.Solid.Sleeping;
            Sleepiness = Defaults.Solid.Sleepiness;
            IsStatic = Defaults.Solid.IsStatic;
            
            // Leave the transform as an identity matrix.
            Transform = Matrix.Identity;

            // "globalLinearVel" is already initialized in its constructor.
            // "globalAngularVel" is already initialized in its constructor.
            LinearDamping = Defaults.Solid.LinearDamping;
            AngularDamping = Defaults.Solid.AngularDamping;
            // The Shape list doesn't need to be initialized.
        }

        /// <summary>
        /// Adds a new Shape to the SolidData.  This automatically 
        /// allocates the right ShapeData type.
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddShape(ShapeData data)
        {
			if (shapes.Contains(data))
				return;

            shapes.Add(data);
        }

        /// <summary>
        /// Returns the number of Shapes in this SolidData.
        /// </summary>
        public virtual int NumShapes
        {
            get { return shapes.Count; }
        }

        /// <summary>
        /// Returns a pointer to the ShapeData at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual ShapeData GetShapeData(int i)
        {
            return shapes[i];
        }

        /// <summary>
        /// Destroys all Shapes in the SolidData.
        /// </summary>
        public virtual void DestroyShapes()
        {
            shapes.Clear();
        }

        public override string ToString()
        {
            return "SolidData: " + Name + " shapes: " + shapes.Count;
        }

    }
}
