

namespace VocaDb.Tests.Service.EntryValidators {

	/*
	/// <summary>
	/// Tests for <see cref="SongValidator"/>.
	/// </summary>
	[TestClass]
	public class SongValidatorTests {

		private Artist producer;
		private Song song;
		private Artist vocalist;

		private void AssertContains(ValidationResult validationResult, string errorMsg) {
			Assert.IsTrue(validationResult.Errors.Contains(errorMsg), string.Format("Validation result contains error '{0}'", errorMsg));
		}

		private void AssertDoesNotContain(ValidationResult validationResult, string errorMsg) {
			Assert.IsFalse(validationResult.Errors.Contains(errorMsg), string.Format("Validation result contains error '{0}'", errorMsg));
		}

		private ValidationResult Validate(Song song) {
			return SongValidator.Validate(song);
		}

		[TestInitialize]
		public void SetUp() {

			vocalist = new Artist(TranslatedString.Create("GUMI"));
			vocalist.ArtistType = ArtistType.Vocaloid;

			producer = new Artist(TranslatedString.Create("devilishP"));
			producer.ArtistType = ArtistType.Producer;

			song = new Song(new LocalizedString("5150", ContentLanguageSelection.English));
			song.AddArtist(vocalist);

		}

		[TestMethod]
		public void MissingProducer() {

			var result = Validate(song);

			AssertContains(result, SongValidationErrors.NeedProducer);
			AssertDoesNotContain(result, SongValidationErrors.NonInstrumentalSongNeedsVocalists);

		}

		/// <summary>
		/// Has a producer, but not in the DB
		/// </summary>
		[TestMethod]
		public void MissingRealProducer() {

			song.AddArtist("devilishP", false, ArtistRoles.Composer);
			var result = Validate(song);

			AssertContains(result, SongValidationErrors.NeedProducer);

		}

		[TestMethod]
		public void HasProducer() {

			song.AddArtist(producer);
			var result = Validate(song);

			AssertDoesNotContain(result, SongValidationErrors.NeedProducer);
			AssertDoesNotContain(result, SongValidationErrors.NonInstrumentalSongNeedsVocalists);

		}

		[TestMethod]
		public void MissingVocalist() {

			song.RemoveArtist(vocalist);
			var result = Validate(song);

			AssertContains(result, SongValidationErrors.NonInstrumentalSongNeedsVocalists);

		}

		[TestMethod]
		public void InstrumentalDoesNotNeedVocalist() {

			song.RemoveArtist(vocalist);
			song.SongType = SongType.Instrumental;
			var result = Validate(song);

			AssertDoesNotContain(result, SongValidationErrors.NonInstrumentalSongNeedsVocalists);

		}

	}*/
}
