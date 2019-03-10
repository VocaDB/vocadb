using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
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

		[TestMethod]
		public void ParseTitle_EmptyParts() {

			// "オリジナル・PV" lead to an empty artist name being searched. 
			// The database collation matches this with an invalid artist, so empty artist searches are ignored.
			var result = NicoHelper.ParseTitle("【初音ミク】心闇【オリジナル・PV】", val => {
				if (string.IsNullOrEmpty(val)) {
					Assert.Fail("Empty name not allowed");
				}
				return CreateEntry.Artist(ArtistType.Producer, name: val);
			});

			Assert.AreEqual(2, result.Artists.Count, "Number of parsed artists");

		}

	}

}
