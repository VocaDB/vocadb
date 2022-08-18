using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Tests.Domain.ReleaseEvents;

[TestClass]
public class ArtistEventTypesTests
{
	[DataRow("Unspecified", nameof(EventCategory.Unspecified))]
	[DataRow("AlbumRelease", nameof(EventCategory.AlbumRelease))]
	[DataRow("Anniversary", nameof(EventCategory.Anniversary))]
	[DataRow("Club", nameof(EventCategory.Club))]
	[DataRow("Concert", nameof(EventCategory.Concert))]
	[DataRow("Contest", nameof(EventCategory.Contest))]
	[DataRow("Convention", nameof(EventCategory.Convention))]
	[DataRow("Other", nameof(EventCategory.Other))]
	[DataRow("Festival", nameof(EventCategory.Festival))]
	[TestMethod]
	public void Name(string expected, string actual)
	{
		actual.Should().Be(expected);
	}
}