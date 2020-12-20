#nullable disable

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

			Assert.AreEqual(ArtistRoles.Default, link.EffectiveRoles, "default roles");
		}

		[TestMethod]
		public void EffectiveRoles_Custom()
		{
			var link = new ArtistForAlbum(_album, _artist, false, ArtistRoles.Mastering);

			Assert.AreEqual(ArtistRoles.Mastering, link.EffectiveRoles, "mastering role");
		}

		[TestMethod]
		public void EffectiveRoles_DefaultNoArtist()
		{
			var link = new ArtistForAlbum(_album, "Miku!", false, ArtistRoles.Default);

			Assert.AreEqual(ArtistRoles.Default, link.EffectiveRoles, "default roles");
		}

		[TestMethod]
		public void EffectiveRoles_Illustrator()
		{
			var link = new ArtistForAlbum(_album, new Artist { ArtistType = ArtistType.Illustrator }, false, ArtistRoles.Default);

			Assert.AreEqual(ArtistRoles.Illustrator, link.EffectiveRoles, "illustrator has illustrator role");
		}
	}
}
