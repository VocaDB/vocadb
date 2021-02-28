#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Service.EntryValidators
{
	/// <summary>
	/// Tests for <see cref="SongValidator"/>.
	/// </summary>
	[TestClass]
	public class SongValidatorTests
	{
		private Artist _producer;
		private Song _song;
		private Artist _vocalist;

		private void TestValidate(bool expectedResult, Song song)
		{
			new SongValidator().IsValid(song, 0).Should().Be(expectedResult);
		}

		[TestInitialize]
		public void SetUp()
		{
			_vocalist = CreateEntry.Artist(ArtistType.Vocaloid, id: 1, name: "GUMI");
			_vocalist.ArtistType = ArtistType.Vocaloid;

			_producer = CreateEntry.Artist(ArtistType.Producer, id: 2, name: "devilishP");
			_producer.ArtistType = ArtistType.Producer;

			_song = new Song(new LocalizedString("5150", ContentLanguageSelection.English)) { SongType = SongType.Original };
			_song.AddArtist(_vocalist);
		}

		[TestMethod]
		public void MissingProducer()
		{
			TestValidate(false, _song);
		}

		/// <summary>
		/// Has a producer, but not in the DB
		/// </summary>
		[TestMethod]
		public void MissingRealProducer()
		{
			_song.AddArtist("devilishP", false, ArtistRoles.Composer);

			TestValidate(false, _song);
		}

		[TestMethod]
		public void HasProducer()
		{
			_song.AddArtist(_producer);

			TestValidate(true, _song);
		}

		[TestMethod]
		public void MissingVocalist()
		{
			_song.RemoveArtist(_vocalist);

			TestValidate(false, _song);
		}

		[TestMethod]
		public void InstrumentalDoesNotNeedVocalist()
		{
			_song.AddArtist(_producer);
			_song.RemoveArtist(_vocalist);
			_song.SongType = SongType.Instrumental;
			_song.Notes.Original = "Instrumental song";

			TestValidate(true, _song);
		}

		[TestMethod]
		public void DuplicateArtist()
		{
			_song.AddArtist(_producer);
			_song.AddArtist(_producer);

			TestValidate(false, _song);
		}
	}
}
