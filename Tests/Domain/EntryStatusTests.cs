using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	[TestClass]
	public class EntryStatusTests
	{
		[DataRow(0, EntryStatus.Draft)]
		[DataRow(1, EntryStatus.Finished)]
		[DataRow(2, EntryStatus.Approved)]
		[DataRow(4, EntryStatus.Locked)]
		[TestMethod]
		public void Value(int expected, EntryStatus actual)
		{
			((int)actual).Should().Be(expected);
		}
	}
}
