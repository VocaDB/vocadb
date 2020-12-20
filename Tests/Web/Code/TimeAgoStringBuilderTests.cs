#nullable disable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Web.Code;
using VocaDb.Web.Resources.Views.Shared;

namespace VocaDb.Tests.Web.Code
{
	/// <summary>
	/// Unit tests for <see cref="TimeAgoStringBuilder"/>.
	/// </summary>
	[TestClass]
	public class TimeAgoStringBuilderTests
	{
		private DateTime _now;

		private string FormatExpected(int amount, string suffix)
		{
			return string.Format(TimeStrings.TimeAgo, amount, suffix);
		}

		private string FormatTimeAgo(TimeSpan ago)
		{
			return TimeAgoStringBuilder.FormatTimeAgo(_now, _now - ago);
		}

		[TestInitialize]
		public void SetUp()
		{
			_now = new DateTime(2013, 6, 1, 12, 0, 0);
		}

		[TestMethod]
		public void Minutes()
		{
			var result = FormatTimeAgo(TimeSpan.FromMinutes(5));
			var expected = FormatExpected(5, TimeStrings.Minutes);

			Assert.AreEqual(expected, result, "Result");
		}

		[TestMethod]
		public void Hours()
		{
			var result = FormatTimeAgo(TimeSpan.FromHours(3));
			var expected = FormatExpected(3, TimeStrings.Hours);

			Assert.AreEqual(expected, result, "Result");
		}

		[TestMethod]
		public void Days()
		{
			var result = FormatTimeAgo(TimeSpan.FromDays(10));
			var expected = FormatExpected(10, TimeStrings.Days);

			Assert.AreEqual(expected, result, "Result");
		}

		[TestMethod]
		public void Months()
		{
			var result = FormatTimeAgo(TimeSpan.FromDays(70));
			var expected = FormatExpected(3, TimeStrings.Months);

			Assert.AreEqual(expected, result, "Result");
		}

		[TestMethod]
		public void Years()
		{
			var result = FormatTimeAgo(TimeSpan.FromDays(730));
			var expected = FormatExpected(24, TimeStrings.Months);

			Assert.AreEqual(expected, result, "Result");
		}
	}
}
