using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain {

	/// <summary>
	/// Tests for <see cref="OptionalDateTime"/>.
	/// </summary>
	[TestClass]
	public class OptionalDateTimeTests {

		private void TestEquals(bool equals, 
			int? firstYear = null, int? firstMonth = null, int? firstDay = null, 
			int? secondYear = null, int? secondMonth = null, int? secondDay = null) {
			
			var first = new OptionalDateTime(firstYear, firstMonth, firstDay);
			var second = new OptionalDateTime(secondYear, secondMonth, secondDay);

			if (equals)
				Assert.AreEqual(first, second, "Dates are equal");
			else
				Assert.AreNotEqual(first, second, "Dates are not equal");

		}

		[TestMethod]
		public void Equals_Full() {
			
			TestEquals(true, 2007, 8, 31, 2007, 8, 31);

		}

		[TestMethod]
		public void Equals_NegativeFull() {
			
			TestEquals(false, 2007, 8, 31, 2008, 8, 31);

		}

		[TestMethod]
		public void Equals_Year() {
			
			TestEquals(true, firstYear: 2007, secondYear: 2007);

		}

		[TestMethod]
		public void Equals_NegativeYear() {
			
			TestEquals(false, firstYear: 2007, secondYear: 2008);

		}

		[TestMethod]
		public void Equals_Null() {
			
			TestEquals(true);

		}

		[TestMethod]
		public void Equals_OnlyOneYear() {
			
			TestEquals(false, firstYear: 2007);

		}

		/// <summary>
		/// Tests Equals when day is specified on only one side.
		/// This should be true because dates without year are considered empty anyway.
		/// </summary>
		[TestMethod]
		public void Equals_OnlyOneDay() {
			
			TestEquals(true, firstDay: 31);

		}

	}

}
