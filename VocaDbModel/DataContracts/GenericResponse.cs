using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace=Schemas.VocaDb)]
	public class GenericResponse {

		public GenericResponse() {
			Successful = true;
		}

		public GenericResponse(bool successful, string message) {
			Successful = successful;
			Message = message;
		}

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool Successful { get; set; }

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class GenericResponse<TResult> : GenericResponse {

		public GenericResponse(TResult result) {
			Result = result;
		}

		public GenericResponse(bool successful, string errorMessage)
			: base(successful, errorMessage) {}

		[DataMember]
		public TResult Result { get; set; }

	}

}
