using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain {

	/// <summary>
	/// Tests for <see cref="Date"/>.
	/// </summary>
	[TestClass]
	public class DateTests {

		[TestMethod]
		public void Ctor_IgnoreDateTimeKind() {
			
			var first = new Date(new DateTimeOffset(2015, 3, 9, 0, 0, 0, TimeSpan.FromHours(6)));
			var second = new Date(new DateTimeOffset(2015, 3, 9, 0, 0, 0, TimeSpan.FromHours(-10)));

			Assert.IsNotNull(first.DateTime, "DateTime");
			Assert.AreEqual(DateTimeKind.Utc, first.DateTime.Value.Kind, "DateTimeKind");
			Assert.AreEqual(first, second, "Dates are equal despite different timezones");

		}

		[TestMethod]
		public void Ctor_IgnoreTime() {
			
			var first = new Date(new DateTimeOffset(2015, 3, 9, 12, 30, 0, TimeSpan.FromHours(0)));
			var second = new Date(new DateTimeOffset(2015, 3, 9, 3, 9, 0, TimeSpan.FromHours(0)));

			Assert.AreEqual(first, second, "Dates are equal despite different times");

		}

	}

}
