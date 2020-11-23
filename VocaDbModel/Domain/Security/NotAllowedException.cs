using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Domain.Security
{

	public class NotAllowedException : Exception
	{
		public NotAllowedException() : base("Operation not allowed") { }
		public NotAllowedException(string message) : base(message) { }
		public NotAllowedException(string message, Exception innerException) : base(message, innerException) { }
		protected NotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

}
