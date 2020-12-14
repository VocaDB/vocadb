#nullable disable

using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Exceptions
{
	public class RestrictedIPException : Exception
	{
		public RestrictedIPException() { }
		public RestrictedIPException(string message) : base(message) { }
		public RestrictedIPException(string message, Exception innerException) : base(message, innerException) { }
		protected RestrictedIPException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
