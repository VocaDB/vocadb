#nullable disable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Queries;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Queries
{
	/// <summary>
	/// Tests for <see cref="ArtistRelationsQuery"/>.
	/// </summary>
	[TestClass]
	public class ArtistRelationsQueryTests
	{
		private readonly Artist artist;
		private readonly FakeUserRepository repository = new FakeUserRepository();
		private readonly ArtistRelationsQuery query;
		private readonly Song song;
		private readonly Song song2;
		private readonly Artist voicebank;

		public ArtistRelationsQueryTests()
		{
			artist = repository.Save(CreateEntry.Artist(ArtistType.Producer));
			voicebank = repository.Save(CreateEntry.Artist(ArtistType.Vocaloid));
			song = repository.Save(CreateEntry.Song());
			song2 = repository.Save(CreateEntry.Song());
			repository.Save(song.AddArtist(artist));
			repository.Save(song.AddArtist(voicebank));
			repository.Save(song2.AddArtist(artist));
			query = new ArtistRelationsQuery(repository.CreateContext(), Model.Domain.Globalization.ContentLanguagePreference.English, new FakeObjectCache(), new InMemoryImagePersister());
		}

		[TestMethod]
		public void LatestSongs()
		{
			var result = query.GetRelations(artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(2, result.LatestSongs.Length, "Number of songs");
			Assert.IsTrue(result.LatestSongs.Any(s => s.Id == song.Id), "Song as expected");
		}

		// Songs for the vocal data provider are ignored
		[TestMethod]
		public void LatestSongs_VocalDataProvider()
		{
			song.GetArtistLink(artist).Roles = ArtistRoles.VocalDataProvider;
			var result = query.GetRelations(artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(1, result.LatestSongs.Length, "Number of songs");
			Assert.AreEqual(song2.Id, result.LatestSongs.First().Id, "Song as expected");
		}

		[TestMethod]
		public void TopVoicebanks()
		{
			var result = query.GetTopVoicebanks(artist);

			// artist has song with voicebank
			Assert.AreEqual(1, result.Length, "Number of voicebanks");
			Assert.AreEqual(voicebank.Id, result[0].Data.Id, "Artist as expected");
		}

		// Only producer roles count
		[TestMethod]
		public void TopVoicebanks_IgnoredRoles()
		{
			// Vocal data provider role is ignored
			song.GetArtistLink(artist).Roles = ArtistRoles.VocalDataProvider;
			var result = query.GetTopVoicebanks(artist);

			Assert.AreEqual(0, result.Length, "Number of voicebanks");
		}

		[TestMethod]
		public void TopVoicebanks_IgnoredAndAllowedRoles()
		{
			// Vocal data provider role is ignored, but VoiceManipulator allows the song to be included.
			song.GetArtistLink(artist).Roles = ArtistRoles.VoiceManipulator | ArtistRoles.VocalDataProvider;
			var result = query.GetTopVoicebanks(artist);

			Assert.AreEqual(1, result.Length, "Number of voicebanks");
		}
	}
}
