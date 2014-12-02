using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Exceptions {

	public class RequestNotValidException : Exception {
		public RequestNotValidException() {}
		public RequestNotValidException(string message) : base(message) {}
		public RequestNotValidException(string message, Exception innerException) : base(message, innerException) {}
		protected RequestNotValidException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
