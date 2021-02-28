#nullable disable

using System.Linq;
using System.Text;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Service.AlbumImport;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.AlbumImport
{
	/// <summary>
	/// Tests for <see cref="KarenTAlbumImporter"/>.
	/// </summary>
	[TestClass]
	public class KarenTAlbumImporterTests
	{
		private KarenTAlbumImporter _importer;
		private MikuDbAlbumContract _importedAlbum;
		private ImportedAlbumDataContract _importedData;
		private HtmlDocument _karenTDoc;

		private void AssertTrack(string expectedTitle, string expectedVocalists, ImportedAlbumTrack track)
		{
			track.Should().NotBeNull("Track was parsed successfully");
			track.Title.Should().Be(expectedTitle, "Title");
			string.Join(", ", track.VocalistNames).Should().Be(expectedVocalists, "Vocalists");
		}

		private ImportedAlbumTrack ParseTrack(string trackString)
		{
			return _importer.ParseTrackRow(1, trackString);
		}

		[TestInitialize]
		public void SetUp()
		{
			_karenTDoc = ResourceHelper.ReadHtmlDocument("KarenT_SystemindParadox.htm");
			_importer = new KarenTAlbumImporter(new PictureDownloaderStub());
			_importedAlbum = _importer.GetAlbumData(_karenTDoc, "http://");
			_importedData = _importedAlbum.Data;
		}

		[TestMethod]
		public void Title()
		{
			_importedData.Title.Should().Be("Systemind Paradox", "Title");
		}

		[TestMethod]
		public void Description()
		{
			_importedData.Description.Should().Be("Heavenz 1st Album", "Description");
		}

		[TestMethod]
		public void Artists()
		{
			_importedData.ArtistNames.Length.Should().Be(1, "1 artist");
			_importedData.ArtistNames.FirstOrDefault().Should().Be("Heavenz", "Artist name");
		}

		[TestMethod]
		public void Vocalists()
		{
			_importedData.VocalistNames.Length.Should().Be(1, "1 vocalist");
			_importedData.VocalistNames.FirstOrDefault().Should().Be("Hatsune Miku", "Vocalist name");
		}

		[TestMethod]
		public void ReleaseDate()
		{
			_importedData.ReleaseYear.Should().Be(2012, "Release year");
		}

		[TestMethod]
		public void CoverPicture()
		{
			_importedAlbum.CoverPicture.Should().NotBeNull("Cover picture downloaded");
			_importedAlbum.CoverPicture.Mime.Should().Be("https://karent.jp/npdca/1048_20120502165707.jpg", "Downloaded URL was correct");
		}

		[TestMethod]
		public void ParseTrackRow()
		{
			var result = ParseTrack("01.&nbsp;Cloud Science (feat. Hatsune Miku)");

			AssertTrack("Cloud Science", "Hatsune Miku", result);
		}

		[TestMethod]
		public void ParseTrackRow_NonWordChars()
		{
			var result = ParseTrack("05.&nbsp;yurameku - Album ver. - (feat. Hatsune Miku)");

			AssertTrack("yurameku - Album ver. -", "Hatsune Miku", result);
		}

		[TestMethod]
		public void ParseTrackRow_OffVocal()
		{
			var result = ParseTrack("11.&nbsp;Quarrel with the doppelganger -off vocal");

			AssertTrack("Quarrel with the doppelganger", "", result);
			// TODO: could also handle type as instrumental
		}

		[TestMethod]
		public void ParseTrackRow_MultipleVocalists()
		{
			var result = ParseTrack("07.&nbsp;MIRAI KOURO / LR ver (feat. Kagamine Rin&Kagamine Len)");

			AssertTrack("MIRAI KOURO / LR ver", "Kagamine Rin, Kagamine Len", result);
		}

		[TestMethod]
		public void Tracks()
		{
			_importedData.Tracks.Length.Should().Be(11, "11 tracks");
		}
	}
}
