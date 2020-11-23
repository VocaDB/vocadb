using System;

namespace VocaDb.Model.Domain
{
	public interface IOptionalGeoPoint
	{
		double? Latitude { get; }

		double? Longitude { get; }
	}

	/// <remarks>
	/// If Latitude or Longitude is specified, the other one needs to be specified too, but it's okay for both to be empty.
	/// </remarks>
	public class OptionalGeoPoint : IOptionalGeoPoint
	{
		public virtual double? Latitude { get; set; }

		public virtual double? Longitude { get; set; }

		public OptionalGeoPoint() { }

		public OptionalGeoPoint(IOptionalGeoPoint geoPoint)
		{
			ParamIs.NotNull(() => geoPoint);

			Validate(geoPoint.Latitude, geoPoint.Longitude);
			Latitude = geoPoint.Latitude;
			Longitude = geoPoint.Longitude;
		}

		public static bool IsValid(double? latitude, double? longitude)
		{
			if (!latitude.HasValue && !longitude.HasValue)
				return true;

			if (!latitude.HasValue || !longitude.HasValue)
				return false;

			if (latitude.Value < -90.0 || latitude.Value > 90.0)
				return false;

			if (longitude.Value < -180.0 || longitude.Value > 180.0)
				return false;

			return true;
		}

		public static void Validate(double? latitude, double? longitude)
		{
			if (!IsValid(latitude, longitude))
				throw new FormatException("Invalid coordinates");
		}

		public virtual bool HasValue => Latitude.HasValue && Longitude.HasValue;

		public virtual bool IsEmpty => !Latitude.HasValue && !Longitude.HasValue;

		public virtual bool Equals(IOptionalGeoPoint other)
		{
			if (other == null)
				return IsEmpty;

			return (Latitude == other.Latitude) && (Longitude == other.Longitude);
		}

		public override int GetHashCode() => ToString().GetHashCode();

		public override bool Equals(object obj) => (obj is OptionalGeoPoint) && Equals((OptionalGeoPoint)obj);

		public override string ToString() => HasValue ? $"{Latitude}, {Longitude}" : string.Empty;
	}
}
