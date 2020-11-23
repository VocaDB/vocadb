using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Exceptions
{
	public class TooFastRegistrationException : Exception
	{
		public TooFastRegistrationException() { }
		public TooFastRegistrationException(string message) : base(message) { }
		public TooFastRegistrationException(string message, Exception innerException) : base(message, innerException) { }
		protected TooFastRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
