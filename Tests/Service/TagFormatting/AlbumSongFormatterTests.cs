#nullable disable

using FluentAssertions;
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
		private const string Format = "%title%%featvocalists%;%producers%;%album%;%discnumber%;%track%";
		private Album _album;
		private Artist _producer;
		private Song _song;
		private AlbumSongFormatter _target;
		private Artist _vocalist;

		private string ApplyFormat(string format, ContentLanguagePreference languageSelection)
		{
			return _target.ApplyFormat(_album, format, null, languageSelection, false);
		}

		[TestInitialize]
		public void SetUp()
		{
			_producer = new Artist(TranslatedString.Create("Tripshots")) { ArtistType = ArtistType.Producer };
			_vocalist = new Artist(new TranslatedString("初音ミク", "Hatsune Miku", "Hatsune Miku")) { ArtistType = ArtistType.Vocaloid };

			_song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English));
			_song.AddArtist(_producer);
			_song.AddArtist(_vocalist);
			_song.UpdateArtistString();

			_album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English));
			_album.AddSong(_song, trackNum: 5, discNum: 1);

			_target = new AlbumSongFormatter(new FakeEntryLinkFactory());
		}

		[TestMethod]
		public void DefaultFormat()
		{
			var result = ApplyFormat(Format, ContentLanguagePreference.Romaji).Trim();

			result.Should().Be("Nebula feat. Hatsune Miku;Tripshots;Synthesis;1;5");
		}

		[TestMethod]
		public void NoArtists()
		{
			_song.RemoveArtist(_producer);
			_song.RemoveArtist(_vocalist);

			var result = ApplyFormat(Format, ContentLanguagePreference.Romaji).Trim();

			result.Should().Be("Nebula;;Synthesis;1;5");
		}

		[TestMethod]
		public void Semicolon()
		{
			_producer.TranslatedName.Romaji = "re;mo";

			var result = ApplyFormat(Format, ContentLanguagePreference.Romaji).Trim();

			result.Should().Be("Nebula feat. Hatsune Miku;\"re;mo\";Synthesis;1;5");
		}

		[TestMethod]
		public void VocaloidsWithProducers()
		{
			var result = ApplyFormat("%title%;%artist%", ContentLanguagePreference.Romaji).Trim();

			result.Should().Be("Nebula;Tripshots feat. Hatsune Miku");
		}
	}
}
