using System;

namespace XnaDevRu.Physics
{
	public class PhysicsDataException : PhysicsException
	{
		public PhysicsDataException()
			: base("Physics Data object initialized incorrect.")
		{ }

		public PhysicsDataException(string message)
			: base(message)
		{ }

		public PhysicsDataException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
