using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Exceptions
{

	/// <summary>
	/// Thrown when an entity was not found.
	/// </summary>
	public class EntityNotFoundException : Exception
	{

		public static void Throw<T>(int id)
		{
			throw new EntityNotFoundException(string.Format("Entity of type {0} with ID {1} not found", typeof(T).Name, id));
		}

		public EntityNotFoundException() { }
		public EntityNotFoundException(string message) : base(message) { }
		public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}
}
