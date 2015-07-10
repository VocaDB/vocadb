using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.SongImport {

	public class UnableToImportException : Exception {
		public UnableToImportException(string message) : base(message) {}
		public UnableToImportException() {}
		public UnableToImportException(string message, Exception innerException) : base(message, innerException) {}
		protected UnableToImportException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}

}
