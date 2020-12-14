#nullable disable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	/// <summary>
	/// Tests for <see cref="OptionalGeoPoint"/>
	/// </summary>
	[TestClass]
	public class OptionalGeoPointTests
	{
		private OptionalGeoPoint GeoPoint(double? latitude, double? longitude) => new OptionalGeoPoint { Latitude = latitude, Longitude = longitude };

		private void TestEquals(bool equals, double? firstLatitude = null, double? firstLongitude = null, double? secondLatitude = null, double? secondLongitude = null)
		{
			var first = GeoPoint(firstLatitude, firstLongitude);
			var second = GeoPoint(secondLatitude, secondLongitude);

			if (equals)
				Assert.AreEqual(first, second, "GeoPoints are equal");
			else
				Assert.AreNotEqual(first, second, "GeoPoints are not equal");
		}

		[TestMethod]
		public void Equals()
		{
			TestEquals(true, 39, 39, 39, 39);
		}

		[TestMethod]
		public void Equals_Negative()
		{
			TestEquals(false, 39, 39, 8, 31);
		}

		[TestMethod]
		public void Equals_Null()
		{
			TestEquals(true);
		}
	}
}
