using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Rankings {

	public class InvalidFeedException : Exception {
		public InvalidFeedException(string message) : base(message) {}
		public InvalidFeedException() {}
		public InvalidFeedException(string message, Exception innerException) : base(message, innerException) {}
		protected InvalidFeedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
