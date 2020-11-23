using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders
{

	public static class VenueQueryableExtender
	{

		private static double GetEarthRadius(DistanceUnit unit) => unit switch
		{
			DistanceUnit.Kilometers => 6371,
			DistanceUnit.Miles => 3960,
			_ => throw new ArgumentException()
		};

		public static IQueryable<Venue> OrderBy(this IQueryable<Venue> query, VenueSortRule sortRule, ContentLanguagePreference languagePreference, GeoPointQueryParams coordinates, DistanceUnit distanceUnit)
		{

			switch (sortRule)
			{
				case VenueSortRule.Name:
					return query.OrderByName(languagePreference);
				case VenueSortRule.Distance:
					return query.OrderByDistance(coordinates, distanceUnit);
			}

			return query;

		}

		public static IQueryable<Venue> OrderByDistance(this IQueryable<Venue> query, GeoPointQueryParams coordinates, DistanceUnit distanceUnit)
		{

			if (!coordinates.HasValue)
				return query;

			var earthRadius = GetEarthRadius(distanceUnit);

			// HACK: calculate the distance between two points on a sphere by using the haversine formula.
			return query.OrderBy(v => earthRadius * 2 * Math.Asin(Math.Sqrt(
				Math.Pow(Math.Sin((coordinates.Latitude.Value - Math.Abs(v.Coordinates.Latitude.Value)) * Math.PI / 180 / 2), 2)
				+ Math.Cos(coordinates.Latitude.Value * Math.PI / 180)
				* Math.Cos(Math.Abs(v.Coordinates.Latitude.Value) * Math.PI / 180)
				* Math.Pow(Math.Sin((coordinates.Longitude.Value - v.Coordinates.Longitude.Value) * Math.PI / 180 / 2), 2))));

		}

		public static IQueryable<Venue> OrderByName(this IQueryable<Venue> query, ContentLanguagePreference languagePreference)
		{
			return query.OrderByEntryName(languagePreference);
		}

		public static IQueryable<Venue> WhereHasName(this IQueryable<Venue> query, SearchTextQuery textQuery)
		{
			return query.WhereHasNameGeneric<Venue, VenueName>(textQuery);
		}

		public static IQueryable<Venue> WhereInCircle(this IQueryable<Venue> query, GeoPointQueryParams queryParams, double? radius, DistanceUnit distanceUnit)
		{

			if (!queryParams.HasValue)
				return query;

			if (radius.HasValue)
			{

				var earthRadius = GetEarthRadius(distanceUnit);

				// HACK: calculate the distance between two points on a sphere by using the haversine formula.
				query = query.Where(v => earthRadius * 2 * Math.Asin(Math.Sqrt(
					Math.Pow(Math.Sin((queryParams.Latitude.Value - Math.Abs(v.Coordinates.Latitude.Value)) * Math.PI / 180 / 2), 2)
					+ Math.Cos(queryParams.Latitude.Value * Math.PI / 180)
					* Math.Cos(Math.Abs(v.Coordinates.Latitude.Value) * Math.PI / 180)
					* Math.Pow(Math.Sin((queryParams.Longitude.Value - v.Coordinates.Longitude.Value) * Math.PI / 180 / 2), 2))) < radius.Value);

			}

			// HACK: do not use OptionalGeoPoint.HasValue here, otherwise NHibernate.QueryException will be thrown.
			return query.Where(v => v.Coordinates.Latitude.HasValue && v.Coordinates.Longitude.HasValue);

		}

	}

	public enum VenueSortRule
	{

		None,

		Name,

		Distance

	}

}
