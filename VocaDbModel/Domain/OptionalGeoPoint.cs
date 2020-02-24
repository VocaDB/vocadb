namespace VocaDb.Model.Domain {

	public interface IOptionalGeoPoint {

		double? Latitude { get; }

		double? Longitude { get; }

	}

	public class OptionalGeoPoint : IOptionalGeoPoint {

		public virtual double? Latitude { get; set; }

		public virtual double? Longitude { get; set; }

		public OptionalGeoPoint() { }

		public OptionalGeoPoint(IOptionalGeoPoint geoPoint) {

			ParamIs.NotNull(() => geoPoint);

			Latitude = geoPoint.Latitude;
			Longitude = geoPoint.Longitude;

		}

		public virtual bool HasValue => Latitude.HasValue && Longitude.HasValue;

		public virtual bool IsEmpty => !Latitude.HasValue && !Longitude.HasValue;

		public virtual bool Equals(IOptionalGeoPoint other) {

			if (other == null)
				return IsEmpty;

			return (Latitude == other.Latitude) && (Longitude == other.Longitude);

		}

		public override string ToString() => $"({Latitude}, {Longitude})";

	}

}
