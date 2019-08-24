using System;

namespace XnaDevRu.Physics
{
	public class PhysicsException : Exception
	{
		public PhysicsException()
			: base("XNA Physics API has thrown an exception.")
		{ }

		public PhysicsException(string message)
			: base(message)
		{ }

		public PhysicsException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
