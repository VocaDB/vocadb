#nullable disable

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
			throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found");
		}

		public EntityNotFoundException() { }
		public EntityNotFoundException(string message) : base(message) { }
		public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
