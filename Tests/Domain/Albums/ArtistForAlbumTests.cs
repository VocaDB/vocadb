#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Tests.Domain.Albums
{
	/// <summary>
	/// Tests for <see cref="ArtistForAlbum"/>.
	/// </summary>
	[TestClass]
	public class ArtistForAlbumTests
	{
		private Album _album;
		private Artist _artist;

		[TestInitialize]
		public void SetUp()
		{
			_album = new Album();
			_artist = new Artist();
		}

		[TestMethod]
		public void EffectiveRoles_DefaultHasArtist()
		{
			var link = new ArtistForAlbum(_album, _artist, false, ArtistRoles.Default);

			link.EffectiveRoles.Should().Be(ArtistRoles.Default, "default roles");
		}

		[TestMethod]
		public void EffectiveRoles_Custom()
		{
			var link = new ArtistForAlbum(_album, _artist, false, ArtistRoles.Mastering);

			link.EffectiveRoles.Should().Be(ArtistRoles.Mastering, "mastering role");
		}

		[TestMethod]
		public void EffectiveRoles_DefaultNoArtist()
		{
			var link = new ArtistForAlbum(_album, "Miku!", false, ArtistRoles.Default);

			link.EffectiveRoles.Should().Be(ArtistRoles.Default, "default roles");
		}

		[TestMethod]
		public void EffectiveRoles_Illustrator()
		{
			var link = new ArtistForAlbum(_album, new Artist { ArtistType = ArtistType.Illustrator }, false, ArtistRoles.Default);

			link.EffectiveRoles.Should().Be(ArtistRoles.Illustrator, "illustrator has illustrator role");
		}
	}
}
