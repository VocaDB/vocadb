using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Exceptions
{
	/// <summary>
	/// Exception thrown when an email address format is invalid.
	/// </summary>
	public class InvalidEmailFormatException : Exception
	{
		public InvalidEmailFormatException() { }
		public InvalidEmailFormatException(string message) : base(message) { }
		public InvalidEmailFormatException(string message, Exception innerException) : base(message, innerException) { }
		protected InvalidEmailFormatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
