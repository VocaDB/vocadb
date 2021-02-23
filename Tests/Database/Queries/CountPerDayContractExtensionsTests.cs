#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Aggregate;

namespace VocaDb.Tests.Database.Queries
{
	/// <summary>
	/// Tests for <see cref="CountPerDayContractExtensions"/>.
	/// </summary>
	[TestClass]
	public class CountPerDayContractExtensionsTests
	{
		private CountPerDayContract[] AddPrevious(TimeUnit timeUnit, params CountPerDayContract[] points)
		{
			return points.AddPreviousValues(true, timeUnit);
		}

		[TestMethod]
		public void AddZeros_Day()
		{
			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2039, 1, 3, 393),
				new CountPerDayContract(2039, 1, 9, 3939)
			};

			var result = points.AddZeros(true, TimeUnit.Day);

			result.Length.Should().Be(9, "Number of data points");
			result[0].Count.Should().Be(39, "First point (assigned)");
			result[0].Day.Should().Be(1, "First point day");
			result[1].Count.Should().Be(0, "Second point (generated, zero)");
			result[1].Day.Should().Be(2, "Second point day");
			result[2].Count.Should().Be(393, "Third point (assigned)");
		}

		[TestMethod]
		public void AddZeros_Month()
		{
			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2040, 1, 1, 3939)
			};

			var result = points.AddZeros(true, TimeUnit.Month);

			result.Length.Should().Be(13, "Number of data points");
			result[0].Count.Should().Be(39, "First point (assigned)");
			result[1].Count.Should().Be(0, "Second point (generated, zero)");
		}

		[TestMethod]
		public void AddZeros_OnePoint()
		{
			var points = new[] {
				new CountPerDayContract(2039, 1, 1, 39),
			};

			var result = points.AddZeros(true, TimeUnit.Day);

			result.Length.Should().Be(1, "Number of data points");
			result[0].Count.Should().Be(39, "First point (assigned)");
		}

		[TestMethod]
		public void AddZeros_NoPoints()
		{
			var points = new CountPerDayContract[0];

			var result = points.AddZeros(true, TimeUnit.Day);

			result.Length.Should().Be(0, "Number of data points");
		}

		[TestMethod]
		public void AddPrevious_NoPoints()
		{
			var result = AddPrevious(TimeUnit.Day);

			result.Length.Should().Be(0, "Number of data points");
		}

		[TestMethod]
		public void AddPrevious_OnePoint()
		{
			var result = AddPrevious(TimeUnit.Day, new CountPerDayContract(2039, 1, 1, 39));

			result.Length.Should().Be(1, "Number of data points");
			result[0].Count.Should().Be(39, "First point (assigned)");
		}

		[TestMethod]
		public void AddPrevious_WithGaps()
		{
			var result = AddPrevious(TimeUnit.Day,
				new CountPerDayContract(2039, 1, 1, 39),
				new CountPerDayContract(2039, 1, 3, 393),
				new CountPerDayContract(2039, 1, 9, 3939)
			);

			result.Length.Should().Be(9, "Number of data points");
			result[0].Count.Should().Be(39, "First point (assigned)");
			result[0].Day.Should().Be(1, "First point day");
			result[1].Count.Should().Be(39, "Second point (generated, previous)");
			result[1].Day.Should().Be(2, "Second point day");
			result[2].Count.Should().Be(393, "Third point (assigned)");
			result[3].Count.Should().Be(393, "4th point (generated, previous)");
		}
	}
}
