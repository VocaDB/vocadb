#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Service.VideoServices
{
	/// <summary>
	/// Unit tests for <see cref="NicoHelper"/>.
	/// </summary>
	[TestClass]
	public class NicoHelperTests
	{
		private Artist ArtistFunc(string name)
		{
			// Note: カバー is a valid artist name.
			var artistNames = new HashSet<string> { "初音ミク", "鏡音リン", "巡音ルカ", "MEIKO", "Lily", "重音テト", "波音リツキレ", "カバー" };

			if (!artistNames.Contains(name))
				return null;

			return new Artist(TranslatedString.Create(name));
		}

		private void AssertArtists(NicoTitleParseResult result, params string[] artists)
		{
			result.Artists.Count.Should().Be(artists.Length, "Number of artists");
			foreach (var artist in artists)
			{
				result.Artists.Any(a => a.DefaultName == artist).Should().BeTrue($"Has artist {artist}");
			}
		}

		private void RunParseTitle(string nicoTitle, string expectedTitle = null, SongType? expectedSongType = null, params string[] artists)
		{
			var result = CallParseTitle(nicoTitle);

			if (expectedTitle != null)
				result.Title.Should().Be(expectedTitle, "title");

			if (expectedSongType.HasValue)
				result.SongType.Should().Be(expectedSongType.Value, "song type");

			if (artists != null)
				AssertArtists(result, artists);
		}

		private NicoTitleParseResult CallParseTitle(string title)
		{
			return NicoHelper.ParseTitle(title, ArtistFunc);
		}

		/// <summary>
		/// Valid title, basic test case
		/// </summary>
		[TestMethod]
		public void ParseTitle_Valid()
		{
			RunParseTitle("【重音テト】 ハイゲインワンダーランド 【オリジナル】", "ハイゲインワンダーランド", SongType.Original, "重音テト");
		}

		/// <summary>
		/// Valid title, cover
		/// </summary>
		[TestMethod]
		public void ParseTitle_Cover()
		{
			RunParseTitle("【鏡音リン・レン】愛言葉Ⅱ【カバー】", "愛言葉Ⅱ", SongType.Cover, "鏡音リン");
		}

		[TestMethod]
		public void ParseTitle_UtauCover()
		{
			RunParseTitle("【波音リツキレ音源】Lost Destination【UTAUカバー】", "Lost Destination", SongType.Cover, "波音リツキレ");
		}

		/// <summary>
		/// Skip whitespace in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_WhiteSpace()
		{
			var result = NicoHelper.ParseTitle("【 鏡音リン 】　sister's noise　(FULL) 【とある科学の超電磁砲S】", ArtistFunc);

			result.Artists.Count.Should().Be(1, "1 artist");
			result.Artists.First().DefaultName.Should().Be("鏡音リン", "artist");
			result.Title.Should().Be("sister's noise　(FULL)", "title");
		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_SpecialChars()
		{
			RunParseTitle("【巡音ルカ･Lily】Blame of Angel", "Blame of Angel", null, "巡音ルカ", "Lily");
		}

		/// <summary>
		/// Handle special characters in artist fields.
		/// </summary>
		[TestMethod]
		public void ParseTitle_MultipleArtistFields()
		{
			var result = NicoHelper.ParseTitle("【初音ミク】恋風～liebe wind～【鏡音リン】", ArtistFunc);

			result.Artists.Count.Should().Be(2, "2 artists");
			result.Artists.First().DefaultName.Should().Be("初音ミク", "artist");
			result.Artists[1].DefaultName.Should().Be("鏡音リン", "artist");
			result.Title.Should().Be("恋風～liebe wind～", "title");
		}

		/// <summary>
		/// Handle artist with original.
		/// </summary>
		[TestMethod]
		public void ParseTitle_ArtistOriginal()
		{
			var result = NicoHelper.ParseTitle("libido / L.A.M.B 【MEIKOオリジナル】", ArtistFunc);

			result.Artists.Count.Should().Be(1, "1 artist");
			result.Artists.First().DefaultName.Should().Be("MEIKO", "artist");
			result.Title.Should().Be("libido / L.A.M.B", "title");
			result.SongType.Should().Be(SongType.Original, "song type");
		}

		[TestMethod]
		public void ParseTitle_EmptyParts()
		{
			// "オリジナル・PV" lead to an empty artist name being searched. 
			// The database collation matches this with an invalid artist, so empty artist searches are ignored.
			var result = NicoHelper.ParseTitle("【初音ミク】心闇【オリジナル・PV】", val =>
			{
				Action action = () =>
				{
					if (string.IsNullOrEmpty(val))
						throw new InvalidOperationException("Empty name not allowed");
				};
				action.Should().NotThrow<InvalidOperationException>();
				return CreateEntry.Artist(ArtistType.Producer, name: val);
			});

			result.Artists.Count.Should().Be(2, "Number of parsed artists");
		}
	}
}
