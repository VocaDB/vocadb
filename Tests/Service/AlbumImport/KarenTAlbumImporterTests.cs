using System.Linq;
using System.Text;
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
		private KarenTAlbumImporter importer;
		private MikuDbAlbumContract importedAlbum;
		private ImportedAlbumDataContract importedData;
		private HtmlDocument karenTDoc;

		private void AssertTrack(string expectedTitle, string expectedVocalists, ImportedAlbumTrack track)
		{
			Assert.IsNotNull(track, "Track was parsed successfully");
			Assert.AreEqual(expectedTitle, track.Title, "Title");
			Assert.AreEqual(expectedVocalists, string.Join(", ", track.VocalistNames), "Vocalists");
		}

		private ImportedAlbumTrack ParseTrack(string trackString)
		{
			return importer.ParseTrackRow(1, trackString);
		}

		[TestInitialize]
		public void SetUp()
		{
			karenTDoc = ResourceHelper.ReadHtmlDocument("KarenT_SystemindParadox.htm");
			importer = new KarenTAlbumImporter(new PictureDownloaderStub());
			importedAlbum = importer.GetAlbumData(karenTDoc, "http://");
			importedData = importedAlbum.Data;
		}

		[TestMethod]
		public void Title()
		{
			Assert.AreEqual("Systemind Paradox", importedData.Title, "Title");
		}

		[TestMethod]
		public void Description()
		{
			Assert.AreEqual("Heavenz 1st Album", importedData.Description, "Description");
		}

		[TestMethod]
		public void Artists()
		{
			Assert.AreEqual(1, importedData.ArtistNames.Length, "1 artist");
			Assert.AreEqual("Heavenz", importedData.ArtistNames.FirstOrDefault(), "Artist name");
		}

		[TestMethod]
		public void Vocalists()
		{
			Assert.AreEqual(1, importedData.VocalistNames.Length, "1 vocalist");
			Assert.AreEqual("Hatsune Miku", importedData.VocalistNames.FirstOrDefault(), "Vocalist name");
		}

		[TestMethod]
		public void ReleaseDate()
		{
			Assert.AreEqual(2012, importedData.ReleaseYear, "Release year");
		}

		[TestMethod]
		public void CoverPicture()
		{
			Assert.IsNotNull(importedAlbum.CoverPicture, "Cover picture downloaded");
			Assert.AreEqual("https://karent.jp/npdca/1048_20120502165707.jpg", importedAlbum.CoverPicture.Mime, "Downloaded URL was correct");
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
			Assert.AreEqual(11, importedData.Tracks.Length, "11 tracks");
		}
	}
}
