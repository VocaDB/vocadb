namespace VocaDb.Model.Domain {

	public class OptionalGeoPoint {

		public virtual double? Latitude { get; set; }

		public virtual double? Longitude { get; set; }

		public OptionalGeoPoint() { }

		public virtual bool HasValue => Latitude.HasValue && Longitude.HasValue;

		public override string ToString() => $"({Latitude}, {Longitude})";

	}

}
