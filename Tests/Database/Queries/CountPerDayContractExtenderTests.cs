using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Aggregate;

namespace VocaDb.Tests.Database.Queries {

	/// <summary>
	/// Tests for <see cref="CountPerDayContractExtender"/>.
	/// </summary>
	[TestClass]
	public class CountPerDayContractExtenderTests {

		private CountPerDayContract[] AddPrevious(TimeUnit timeUnit, params CountPerDayContract[] points) {
			return points.AddPreviousValues(true, timeUnit);
		}

		[TestMethod]
		public void AddZeros_Day() {

			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2039, 1, 3, 393),
				new CountPerDayContract(2039, 1, 9, 3939)
			};

			var result = points.AddZeros(true, TimeUnit.Day);

			Assert.AreEqual(9, result.Length, "Number of data points");
			Assert.AreEqual(39, result[0].Count, "First point (assigned)");
			Assert.AreEqual(1, result[0].Day, "First point day");
			Assert.AreEqual(0, result[1].Count, "Second point (generated, zero)");
			Assert.AreEqual(2, result[1].Day, "Second point day");
			Assert.AreEqual(393, result[2].Count, "Third point (assigned)");

		}

		[TestMethod]
		public void AddZeros_Month() {

			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2040, 1, 1, 3939)
			};

			var result = points.AddZeros(true, TimeUnit.Month);

			Assert.AreEqual(13, result.Length, "Number of data points");
			Assert.AreEqual(39, result[0].Count, "First point (assigned)");
			Assert.AreEqual(0, result[1].Count, "Second point (generated, zero)");

		}

		[TestMethod]
		public void AddZeros_OnePoint() {

			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
			};

			var result = points.AddZeros(true, TimeUnit.Day);

			Assert.AreEqual(1, result.Length, "Number of data points");
			Assert.AreEqual(39, result[0].Count, "First point (assigned)");

		}

		[TestMethod]
		public void AddZeros_NoPoints() {

			var points = new CountPerDayContract[0];

			var result = points.AddZeros(true, TimeUnit.Day);

			Assert.AreEqual(0, result.Length, "Number of data points");

		}

		[TestMethod]
		public void AddPrevious_NoPoints() {

			var result = AddPrevious(TimeUnit.Day);

			Assert.AreEqual(0, result.Length, "Number of data points");

		}

		[TestMethod]
		public void AddPrevious_OnePoint() {

			var result = AddPrevious(TimeUnit.Day, new CountPerDayContract(2039, 1, 1, 39));

			Assert.AreEqual(1, result.Length, "Number of data points");
			Assert.AreEqual(39, result[0].Count, "First point (assigned)");

		}

		[TestMethod]
		public void AddPrevious_WithGaps() {

			var result = AddPrevious(TimeUnit.Day,
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2039, 1, 3, 393),
				new CountPerDayContract(2039, 1, 9, 3939)
			);

			Assert.AreEqual(9, result.Length, "Number of data points");
			Assert.AreEqual(39, result[0].Count, "First point (assigned)");
			Assert.AreEqual(1, result[0].Day, "First point day");
			Assert.AreEqual(39, result[1].Count, "Second point (generated, previous)");
			Assert.AreEqual(2, result[1].Day, "Second point day");
			Assert.AreEqual(393, result[2].Count, "Third point (assigned)");
			Assert.AreEqual(393, result[3].Count, "4th point (generated, previous)");

		}
	}

}
