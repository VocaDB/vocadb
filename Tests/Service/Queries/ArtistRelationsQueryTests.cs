using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Queries;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Queries {

	/// <summary>
	/// Tests for <see cref="ArtistRelationsQuery"/>.
	/// </summary>
	[TestClass]
	public class ArtistRelationsQueryTests {

		private readonly Artist artist;
		private readonly FakeUserRepository repository = new FakeUserRepository();
		private readonly ArtistRelationsQuery query;
		private readonly Song song;
		private readonly Song song2;

		public ArtistRelationsQueryTests() {
			artist = repository.Save(CreateEntry.Artist(ArtistType.Producer));
			song = repository.Save(CreateEntry.Song());
			song2 = repository.Save(CreateEntry.Song());
			repository.Save(song.AddArtist(artist));
			repository.Save(song2.AddArtist(artist));
			query = new ArtistRelationsQuery(repository.CreateContext(), Model.Domain.Globalization.ContentLanguagePreference.English, new FakeObjectCache());
		}

		[TestMethod]
		public void LatestSongs() {

			var result = query.GetRelations(artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(2, result.LatestSongs.Length, "Number of songs");
			Assert.IsTrue(result.LatestSongs.Any(s => s.Id == song.Id), "Song as expected");

		}

		// Songs for the vocal data provider are ignored
		[TestMethod]
		public void LatestSongs_VocalDataProvider() {

			artist.AllSongs.First(s => s.Song == song).Roles = ArtistRoles.VocalDataProvider;
			var result = query.GetRelations(artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(1, result.LatestSongs.Length, "Number of songs");
			Assert.AreEqual(song2.Id, result.LatestSongs.First().Id, "Song as expected");

		}

	}

}
