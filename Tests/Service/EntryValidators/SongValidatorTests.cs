using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Service.EntryValidators {

	/// <summary>
	/// Tests for <see cref="SongValidator"/>.
	/// </summary>
	[TestClass]
	public class SongValidatorTests {

		private Artist producer;
		private Song song;
		private Artist vocalist;

		private void TestValidate(bool expectedResult, Song song) {
			Assert.AreEqual(expectedResult, new SongValidator().IsValid(song));
		}

		[TestInitialize]
		public void SetUp() {

			vocalist = CreateEntry.Artist(ArtistType.Vocaloid, id: 1, name: "GUMI");
			vocalist.ArtistType = ArtistType.Vocaloid;

			producer = CreateEntry.Artist(ArtistType.Producer, id: 2, name: "devilishP");
			producer.ArtistType = ArtistType.Producer;

			song = new Song(new LocalizedString("5150", ContentLanguageSelection.English)) { SongType = SongType.Original };
			song.AddArtist(vocalist);

		}

		[TestMethod]
		public void MissingProducer() {

			TestValidate(false, song);

		}

		/// <summary>
		/// Has a producer, but not in the DB
		/// </summary>
		[TestMethod]
		public void MissingRealProducer() {

			song.AddArtist("devilishP", false, ArtistRoles.Composer);

			TestValidate(false, song);

		}

		[TestMethod]
		public void HasProducer() {

			song.AddArtist(producer);

			TestValidate(true, song);

		}

		[TestMethod]
		public void MissingVocalist() {

			song.RemoveArtist(vocalist);

			TestValidate(false, song);

		}

		[TestMethod]
		public void InstrumentalDoesNotNeedVocalist() {

			song.AddArtist(producer);
			song.RemoveArtist(vocalist);
			song.SongType = SongType.Instrumental;
			song.Notes.Original = "Instrumental song";

			TestValidate(true, song);

		}

		[TestMethod]
		public void DuplicateArtist() {
	
			song.AddArtist(producer);
			song.AddArtist(producer);

			TestValidate(false, song);

		}

	}
}
