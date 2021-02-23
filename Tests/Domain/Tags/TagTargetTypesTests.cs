using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Tests.Domain.Tags
{
	[TestClass]
	public class TagTargetTypesTests
	{
		[DataRow(0, TagTargetTypes.Nothing)]
		[DataRow(1, TagTargetTypes.Album)]
		[DataRow(2, TagTargetTypes.Artist)]
		[DataRow(64, TagTargetTypes.Song)]
		[DataRow(1 | 2, TagTargetTypes.AlbumArtist)]
		[DataRow(1 | 64, TagTargetTypes.AlbumSong)]
		[DataRow(2 | 64, TagTargetTypes.ArtistSong)]
		[DataRow(16, TagTargetTypes.Event)]
		[DataRow(1073741823, TagTargetTypes.All)]
		[TestMethod]
		public void Value(int expected, TagTargetTypes actual)
		{
			((int)actual).Should().Be(expected);
		}
	}
}
