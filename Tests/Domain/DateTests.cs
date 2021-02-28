#nullable disable

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	/// <summary>
	/// Tests for <see cref="Date"/>.
	/// </summary>
	[TestClass]
	public class DateTests
	{
		[TestMethod]
		public void Ctor_IgnoreDateTimeKind()
		{
			var first = new Date(new DateTimeOffset(2015, 3, 9, 0, 0, 0, TimeSpan.FromHours(6)));
			var second = new Date(new DateTimeOffset(2015, 3, 9, 0, 0, 0, TimeSpan.FromHours(-10)));

			first.DateTime.Should().NotBeNull("DateTime");
			first.DateTime.Value.Kind.Should().Be(DateTimeKind.Utc, "DateTimeKind");
			second.Should().Be(first, "Dates are equal despite different timezones");
		}

		[TestMethod]
		public void Ctor_IgnoreTime()
		{
			var first = new Date(new DateTimeOffset(2015, 3, 9, 12, 30, 0, TimeSpan.FromHours(0)));
			var second = new Date(new DateTimeOffset(2015, 3, 9, 3, 9, 0, TimeSpan.FromHours(0)));

			second.Should().Be(first, "Dates are equal despite different times");
		}
	}
}
