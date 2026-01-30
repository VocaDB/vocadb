using System;
using System.Runtime.Serialization;

namespace VocaDb.NicoApi {

	/// <summary>
	/// Exception thrown if an API request failed.
	/// </summary>
	public class NicoApiException : Exception {
		public NicoApiException() {}
		public NicoApiException(string message, Exception innerException) : base(message, innerException) {}
		public NicoApiException(string message, string nicoError = null) : base(message) {
            NicoError = nicoError;
        }
		protected NicoApiException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public string NicoError { get; }

	}

}
