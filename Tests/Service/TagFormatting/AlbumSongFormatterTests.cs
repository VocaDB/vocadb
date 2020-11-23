using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.TagFormatting
{

	/// <summary>
	/// Tests for <see cref="AlbumSongFormatter"/>.
	/// </summary>
	[TestClass]
	public class AlbumSongFormatterTests
	{

		private const string defaultFormat = "%title%%featvocalists%;%producers%;%album%;%discnumber%;%track%";
		private Album album;
		private Artist producer;
		private Song song;
		private AlbumSongFormatter target;
		private Artist vocalist;

		private string ApplyFormat(string format, ContentLanguagePreference languageSelection)
		{
			return target.ApplyFormat(album, format, null, languageSelection, false);
		}

		[TestInitialize]
		public void SetUp()
		{

			producer = new Artist(TranslatedString.Create("Tripshots")) { ArtistType = ArtistType.Producer };
			vocalist = new Artist(new TranslatedString("初音ミク", "Hatsune Miku", "Hatsune Miku")) { ArtistType = ArtistType.Vocaloid };

			song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English));
			song.AddArtist(producer);
			song.AddArtist(vocalist);
			song.UpdateArtistString();

			album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English));
			album.AddSong(song, trackNum: 5, discNum: 1);

			target = new AlbumSongFormatter(new FakeEntryLinkFactory());

		}

		[TestMethod]
		public void DefaultFormat()
		{

			var result = ApplyFormat(defaultFormat, ContentLanguagePreference.Romaji).Trim();

			Assert.AreEqual("Nebula feat. Hatsune Miku;Tripshots;Synthesis;1;5", result);

		}

		[TestMethod]
		public void NoArtists()
		{

			song.RemoveArtist(producer);
			song.RemoveArtist(vocalist);

			var result = ApplyFormat(defaultFormat, ContentLanguagePreference.Romaji).Trim();

			Assert.AreEqual("Nebula;;Synthesis;1;5", result);

		}

		[TestMethod]
		public void Semicolon()
		{

			producer.TranslatedName.Romaji = "re;mo";

			var result = ApplyFormat(defaultFormat, ContentLanguagePreference.Romaji).Trim();

			Assert.AreEqual("Nebula feat. Hatsune Miku;\"re;mo\";Synthesis;1;5", result);

		}

		[TestMethod]
		public void VocaloidsWithProducers()
		{

			var result = ApplyFormat("%title%;%artist%", ContentLanguagePreference.Romaji).Trim();

			Assert.AreEqual("Nebula;Tripshots feat. Hatsune Miku", result);
		}

	}

}
