using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Unit tests for <see cref="NicoHelper"/>.
	/// </summary>
	[TestClass]
	public class NicoHelperTests {

		private Artist ArtistFunc(string name) {

			// Note: カバー is a valid artist name.
			var artistNames = new HashSet<string> { "初音ミク", "鏡音リン", "巡音ルカ", "MEIKO", "Lily", "重音テト", "波音リツキレ", "カバー" };

			if (!artistNames.Contains(name))
				return null;

			return new Artist(TranslatedString.Create(name));

		}

		private void AssertArtists(NicoTitleParseResult result, params string[] artists) {
			
			Assert.AreEqual(artists.Length, result.Artists.Count, "Number of artists");
			foreach (var artist in artists) {
				Assert.IsTrue(result.Artists.Any(a => a.DefaultName == artist), string.Format("Has artist {0}", artist));
			}

		}

		private void RunParseTitle(string nicoTitle, string expectedTitle = null, SongType? expectedSongType = null, params string[] artists) {
			
			var result = CallParseTitle(nicoTitle);

			if (expectedTitle != null)
				Assert.AreEqual(expectedTitle, result.Title, "title");

			if (expectedSongType.HasValue)
				Assert.AreEqual(expectedSongType.Value, result.SongType, "song type");

			if (artists != null)
				AssertArtists(result, artists);

		}

		private NicoTitleParseResult CallParseTitle(string title) {
			return NicoHelper.ParseTitle(title, ArtistFunc);
		}

		[TestInitialize]
		public void SetUp() {
			
		}

		[TestMethod]
		public void GetResponse_Ok() {
			
			NicoResponse response;
			using (var stream = ResourceHelper.GetFileStream("NicoResponse_Ok.xml")) {
				response = NicoHelper.GetResponse(stream);
			}

			var result = NicoHelper.ParseResponse(response);

			Assert.IsTrue(result.Success, "Success");
			Assert.AreEqual("【初音ミク】１７：００【オリジナル曲】", result.Title, "Title");
			Assert.AreEqual("http://tn-skr1.smilevideo.jp/smile?i=12464004", result.ThumbUrl, "ThumbUrl");
			Assert.IsNotNull(result.UploadDate, "UploadDate");
			Assert.AreEqual(new DateTime(2010, 10, 17).Date, result.UploadDate.Value.Date, "UploadDate");
			Assert.AreEqual(178, result.LengthSeconds, "LengthSeconds");
			Assert.AreEqual("14270239", result.AuthorId, "AuthorId");
			Assert.AreEqual("ProjectDIVAチャンネル", result.Author, "Author");
			Assert.AreEqual(11, result.Tags.Length, "Tags.Length");
			Assert.IsTrue(result.Tags.Contains("VOCALOID"), "Found tag");

		}

		[TestMethod]
		public void GetResponse_Error() {
			
			NicoResponse response;
			using (var stream = ResourceHelper.GetFileStream("NicoResponse_Error.xml")) {
				response = NicoHelper.GetResponse(stream);
			}

			var result = NicoHelper.ParseResponse(response);

			Assert.IsFalse(result.Success, "Success");
			Assert.AreEqual("NicoVideo (error): not found or invalid", result.Error, "Error");

		}

		[TestMethod]
		public void ParseLength_LessThan10Mins() {

			var result = NicoHelper.ParseLength("3:09");

			Assert.AreEqual(189, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan10Mins() {

			var result = NicoHelper.ParseLength("39:39");

			Assert.AreEqual(2379, result, "result");

		}

		[TestMethod]
		public void ParseLength_MoreThan60Mins() {

			var result = NicoHelper.ParseLength("339:39");

			Assert.AreEqual(20379, result, "result");

		}

		/// <summary>
		/// Valid title, basic test case
		/// </summary>
		[TestMethod]
		public void ParseTitle_Valid() {

			RunParseTitle("【重音テト】 ハイゲインワンダーランド 【オリジナル】", "ハイゲインワンダーランド", SongType.Original, "重音テト");

		}

		/// <summary>
		/// Valid title, cover
		/// </summary>
		[TestMethod]
		public void ParseTitle_Cover() {
			
			RunParseTitle("【鏡音リン・レン】愛言葉Ⅱ【カバー】", "愛言葉Ⅱ", SongType.Cover, "鏡音リン");

		}

		[TestMethod]
		public void ParseTitle_UtauCover() {

			RunParseTitle("【波音リツキレ音源】Lost Destination【UTAUカバー】", "Lost Destination", SongType.Cover, "波音リツキレ");

		}

		/// <summary>
		/// Skip whitespace in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_WhiteSpace() {

			var result = NicoHelper.ParseTitle("【 鏡音リン 】　sister's noise　(FULL) 【とある科学の超電磁砲S】", ArtistFunc);

			Assert.AreEqual(1, result.Artists.Count, "1 artist");
			Assert.AreEqual("鏡音リン", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("sister's noise　(FULL)", result.Title, "title");

		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_SpecialChars() {

			RunParseTitle("【巡音ルカ･Lily】Blame of Angel", "Blame of Angel", null, "巡音ルカ", "Lily");

		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_MultipleArtistFields() {

			var result = NicoHelper.ParseTitle("【初音ミク】恋風～liebe wind～【鏡音リン】", ArtistFunc);

			Assert.AreEqual(2, result.Artists.Count, "2 artists");
			Assert.AreEqual("初音ミク", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("鏡音リン", result.Artists[1].DefaultName, "artist");
			Assert.AreEqual("恋風～liebe wind～", result.Title, "title");

		}

		/// <summary>
		/// Handle artist with original.
		/// </summary>
		[TestMethod]
		public void ParseTitle_ArtistOriginal() {

			var result = NicoHelper.ParseTitle("libido / L.A.M.B 【MEIKOオリジナル】", ArtistFunc);

			Assert.AreEqual(1, result.Artists.Count, "1 artist");
			Assert.AreEqual("MEIKO", result.Artists.First().DefaultName, "artist");
			Assert.AreEqual("libido / L.A.M.B", result.Title, "title");
			Assert.AreEqual(SongType.Original, result.SongType, "song type");

		}

	}

}
