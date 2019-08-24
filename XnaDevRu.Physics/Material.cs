using System;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// Data structure describing material properties.  These properties
    /// determine the collision response when two Solids collide.
    /// </summary>
	public class Material
	{
		#region fields
		private float hardness, friction, bounciness, density;

		/// <summary>
		/// Determines how far Solids can interpenetrate.  This must
		/// be between 0 and 1.
		/// </summary>
		public float Hardness 
		{
			get { return hardness; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value", "Hardness must be between 0 and 1");

				hardness = value;
			}
		}

		/// <summary>
		/// Simple friction constant.  This must be between 0 and 1.
		/// </summary>
		public float Friction
		{
			get { return friction; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value", "Friction must be between 0 and 1");

				friction = value;
			}
		}

		/// <summary>
		/// Bounciness (i.e. restitution) determines how elastic the
		/// collisions will be between this Material and another.  In other
		/// words, the more bounciness, the farther the Solids will bounce
		/// when they collide (and, in real life, the less energy is lost
		/// due to heat and sound).  This must be between 0 and 1.
		/// </summary>
		public float Bounciness
		{
			get { return bounciness; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value", "Bounciness must be between 0 and 1");

				bounciness = value;
			}
		}

		/// <summary>
		/// Density combined with the volume of a Solid's shapes determine
		/// the Solid's mass.  This must be >= 0.
		/// </summary>
		public float Density
		{
			get { return density; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Density must be nonnegative");

				density = value;
			}
		}
		#endregion

		#region Static Fields
		// Pre-defined material settings
		public const float MetalHardness = 1.0f;
		public const float MetalFriction = 0.7f;
		public const float MetalBounciness = 0.05f;
		public const float MetalDensity = 1.0f;

		//NOTE: Max and mix friction settings result in faster calculations.
		public const float WoodHardness = 0.8f;
		public const float WoodFriction = 1.0f;
		public const float WoodBounciness = 0.15f;
		public const float WoodDensity = 0.2f;

		public const float RubberHardness = 0;
		public const float RubberFriction = 1.0f;
		public const float RubberBounciness = 1.0f;
		public const float RubberDensity = 0.4f;

		public const float IceHardness = 1.0f;
		public const float IceFriction = 0;
		public const float IceBounciness = 0.05f;
		public const float IceDensity = 0.25f;

		// Global materials
		public static Material MetalMaterial { get { return new Material(MetalHardness, MetalFriction, MetalBounciness, MetalDensity); } }
		public static Material WoodMaterial { get { return new Material(WoodHardness, WoodFriction, WoodBounciness, WoodDensity); } }
		public static Material RubberMaterial { get { return new Material(RubberHardness, RubberFriction, RubberBounciness, RubberDensity); } }
		public static Material IceMaterial { get { return new Material(IceHardness, IceFriction, IceBounciness, IceDensity); } }
		#endregion

		public Material(float h, float f, float b, float d)
		{
			Hardness = h;
			Friction = f;
			Bounciness = b;
			Density = d;
		}

        public Material()
        {
			hardness = WoodHardness;
			bounciness = WoodBounciness;
			friction = WoodFriction;
			density = WoodDensity;
        }

        public override string ToString()
        {
            
            return "Desity: " + density + " Friction: " + friction + " Hardness: " + hardness;
        }

    }
}
