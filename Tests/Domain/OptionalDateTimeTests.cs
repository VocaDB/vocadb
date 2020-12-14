#nullable disable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	/// <summary>
	/// Tests for <see cref="OptionalDateTime"/>.
	/// </summary>
	[TestClass]
	public class OptionalDateTimeTests
	{
		private OptionalDateTime Date(int? year = null, int? month = null, int? day = null)
		{
			return new OptionalDateTime(year, month, day);
		}

		private void TestCompareTo(int expected,
			OptionalDateTime first, OptionalDateTime second)
		{
			Assert.AreEqual(expected, first.CompareTo(second), "Comparing " + first + " to " + second);
		}

		private void TestEquals(bool equals,
			int? firstYear = null, int? firstMonth = null, int? firstDay = null,
			int? secondYear = null, int? secondMonth = null, int? secondDay = null)
		{
			var first = Date(firstYear, firstMonth, firstDay);
			var second = Date(secondYear, secondMonth, secondDay);

			if (equals)
				Assert.AreEqual(first, second, "Dates are equal");
			else
				Assert.AreNotEqual(first, second, "Dates are not equal");
		}

		[TestMethod]
		public void Equals_Full()
		{
			TestEquals(true, 2007, 8, 31, 2007, 8, 31);
		}

		[TestMethod]
		public void Equals_NegativeFull()
		{
			TestEquals(false, 2007, 8, 31, 2008, 8, 31);
		}

		[TestMethod]
		public void Equals_Year()
		{
			TestEquals(true, firstYear: 2007, secondYear: 2007);
		}

		[TestMethod]
		public void Equals_NegativeYear()
		{
			TestEquals(false, firstYear: 2007, secondYear: 2008);
		}

		[TestMethod]
		public void Equals_Null()
		{
			TestEquals(true);
		}

		[TestMethod]
		public void Equals_OnlyOneYear()
		{
			TestEquals(false, firstYear: 2007);
		}

		/// <summary>
		/// Tests Equals when day is specified on only one side.
		/// This should be true because dates without year are considered empty anyway.
		/// </summary>
		[TestMethod]
		public void Equals_OnlyOneDay()
		{
			TestEquals(true, firstDay: 31);
		}

		[TestMethod]
		public void CompareTo_Full_Equal()
		{
			TestCompareTo(0, Date(2017, 1, 1), Date(2017, 1, 1));
		}

		[TestMethod]
		public void CompareTo_Full_LessThan()
		{
			TestCompareTo(-1, Date(2017, 1, 1), Date(2017, 8, 31));
		}

		[TestMethod]
		public void CompareTo_Full_GreaterThan()
		{
			TestCompareTo(1, Date(2017, 8, 31), Date(2017, 1, 1));
		}

		[TestMethod]
		public void CompareTo_Null()
		{
			TestCompareTo(0, Date(), Date());
		}
	}
}
