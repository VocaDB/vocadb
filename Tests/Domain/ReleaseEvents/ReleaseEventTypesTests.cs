using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Tests.Domain.ReleaseEvents;

[TestClass]
public class ArtistEventTypesTests
{
	[DataRow(0, EventCategory.Unspecified)]
	[DataRow(1, EventCategory.AlbumRelease)]
	[DataRow(2, EventCategory.Anniversary)]
	[DataRow(4, EventCategory.Club)]
	[DataRow(8, EventCategory.Concert)]
	[DataRow(16, EventCategory.Contest)]
	[DataRow(32, EventCategory.Convention)]
	[DataRow(64, EventCategory.Other)]
	[DataRow(128, EventCategory.Festival)]
	[TestMethod]
	public void Value(int expected, EventCategory actual)
	{
		((int) actual).Should().Be(expected);
	}
}