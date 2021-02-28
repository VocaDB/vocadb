using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	[TestClass]
	public class EntryTypeTests
	{
		[DataRow(0, EntryType.Undefined)]
		[DataRow(1, EntryType.Album)]
		[DataRow(2, EntryType.Artist)]
		[DataRow(4, EntryType.DiscussionTopic)]
		[DataRow(8, EntryType.PV)]
		[DataRow(16, EntryType.ReleaseEvent)]
		[DataRow(32, EntryType.ReleaseEventSeries)]
		[DataRow(64, EntryType.Song)]
		[DataRow(128, EntryType.SongList)]
		[DataRow(256, EntryType.Tag)]
		[DataRow(512, EntryType.User)]
		[DataRow(1024, EntryType.Venue)]
		[TestMethod]
		public void Value(int expected, EntryType actual)
		{
			((int)actual).Should().Be(expected);
		}
	}
}
