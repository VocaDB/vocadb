using System;
using System.Runtime.Serialization;

namespace VocaDb.Web.Code.Exceptions
{
	/// <summary>
	/// Exception indicating that invalid web form was posted.
	/// </summary>
	public class InvalidFormException : Exception
	{
		public InvalidFormException() { }
		public InvalidFormException(string message) : base(message) { }
		public InvalidFormException(string message, Exception innerException) : base(message, innerException) { }
		protected InvalidFormException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}