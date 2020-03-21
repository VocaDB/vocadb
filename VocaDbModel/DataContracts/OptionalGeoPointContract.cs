using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class OptionalGeoPointContract : IOptionalGeoPoint {

		[DataMember]
		public string Formatted { get; set; }

		[DataMember]
		public bool HasValue { get; set; }

		[DataMember]
		public double? Latitude { get; set; }

		[DataMember]
		public double? Longitude { get; set; }

		public OptionalGeoPointContract() { }

		public OptionalGeoPointContract(OptionalGeoPoint geoPoint) {

			ParamIs.NotNull(() => geoPoint);

			Formatted = geoPoint.ToString();
			HasValue = geoPoint.HasValue;
			Latitude = geoPoint.Latitude;
			Longitude = geoPoint.Longitude;

		}

	}

}
