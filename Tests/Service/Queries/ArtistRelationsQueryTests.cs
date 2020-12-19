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
		private readonly Artist _artist;
		private readonly FakeUserRepository _repository = new();
		private readonly ArtistRelationsQuery _query;
		private readonly Song _song;
		private readonly Song _song2;
		private readonly Artist _voicebank;

		public ArtistRelationsQueryTests()
		{
			_artist = _repository.Save(CreateEntry.Artist(ArtistType.Producer));
			_voicebank = _repository.Save(CreateEntry.Artist(ArtistType.Vocaloid));
			_song = _repository.Save(CreateEntry.Song());
			_song2 = _repository.Save(CreateEntry.Song());
			_repository.Save(_song.AddArtist(_artist));
			_repository.Save(_song.AddArtist(_voicebank));
			_repository.Save(_song2.AddArtist(_artist));
			_query = new ArtistRelationsQuery(_repository.CreateContext(), Model.Domain.Globalization.ContentLanguagePreference.English, new FakeObjectCache(), new InMemoryImagePersister());
		}

		[TestMethod]
		public void LatestSongs()
		{
			var result = _query.GetRelations(_artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(2, result.LatestSongs.Length, "Number of songs");
			Assert.IsTrue(result.LatestSongs.Any(s => s.Id == _song.Id), "Song as expected");
		}

		// Songs for the vocal data provider are ignored
		[TestMethod]
		public void LatestSongs_VocalDataProvider()
		{
			_song.GetArtistLink(_artist).Roles = ArtistRoles.VocalDataProvider;
			var result = _query.GetRelations(_artist, ArtistRelationsFields.LatestSongs);

			Assert.AreEqual(1, result.LatestSongs.Length, "Number of songs");
			Assert.AreEqual(_song2.Id, result.LatestSongs.First().Id, "Song as expected");
		}

		[TestMethod]
		public void TopVoicebanks()
		{
			var result = _query.GetTopVoicebanks(_artist);

			// artist has song with voicebank
			Assert.AreEqual(1, result.Length, "Number of voicebanks");
			Assert.AreEqual(_voicebank.Id, result[0].Data.Id, "Artist as expected");
		}

		// Only producer roles count
		[TestMethod]
		public void TopVoicebanks_IgnoredRoles()
		{
			// Vocal data provider role is ignored
			_song.GetArtistLink(_artist).Roles = ArtistRoles.VocalDataProvider;
			var result = _query.GetTopVoicebanks(_artist);

			Assert.AreEqual(0, result.Length, "Number of voicebanks");
		}

		[TestMethod]
		public void TopVoicebanks_IgnoredAndAllowedRoles()
		{
			// Vocal data provider role is ignored, but VoiceManipulator allows the song to be included.
			_song.GetArtistLink(_artist).Roles = ArtistRoles.VoiceManipulator | ArtistRoles.VocalDataProvider;
			var result = _query.GetTopVoicebanks(_artist);

			Assert.AreEqual(1, result.Length, "Number of voicebanks");
		}
	}
}
