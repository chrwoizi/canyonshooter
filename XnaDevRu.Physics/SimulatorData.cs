using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{ 
	/// <summary>
	/// Various configuration data for Simulator
	/// </summary>
	public class SimulatorData
	{
		// constructor with default values
		public SimulatorData()
		{
			UseOctreeInsteadHash = false;

			// octree defaults
			WorldSize = new Vector3(1000, 1000, 1000);
			OctreeDepth = 7;
			WorldCenter = new Vector3(0, 0, 0);

			// hash defaults
			HashMinLevel = -5;
			HashMaxLevel = 9;
		}

		
        private bool useOctreeInsteadHash;

        /// <summary>
        /// if true, octree space will be used
        /// if false, hash space will be used instead
        /// default is false = hash space
        /// </summary>
        public bool UseOctreeInsteadHash {
            get { return useOctreeInsteadHash; }
            set { useOctreeInsteadHash = value; }
        }

		/// <summary>
		/// used for octree space, default - (1000,1000,1000)
		/// </summary>
		public Vector3 WorldSize;

		/// <summary>
		/// used for octree space, default - (0,0,0)
		/// </summary>
		public Vector3 WorldCenter;
		
        private int octreeDepth;

        /// <summary>
        /// used for octree space, default - 7
        /// </summary>
        public int OctreeDepth {
            get { return octreeDepth; }
            set { octreeDepth = value; }
        }


        private int hashMinLevel;

        /// <summary>
        /// for hash space: 2^(this value)
        /// </summary>
        public int HashMinLevel {
            get { return hashMinLevel; }
            set { hashMinLevel = value; }
        }

        private int hashMaxLevel;

        /// <summary>
        /// for hash space: 2^(this value)
        /// </summary>
        public int HashMaxLevel {
            get { return hashMaxLevel; }
            set { hashMaxLevel = value; }
        }

        public override string ToString()
        {
            return "SimulatorData: world size = " + this.WorldSize;
        }
	}
}
