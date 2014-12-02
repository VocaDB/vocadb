using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.SongSearch {

	/*
	/// <summary>
	/// Tests for <see cref="Model.Service.Search.SongSearch.SongSearch"/>.
	/// </summary>
	[TestClass]
	public class SongSearchTests {

		private Artist artist;
		private SongQueryParams queryParams;
		private QuerySourceList querySource;
		private Model.Service.Search.SongSearch.SongSearch search;
		private Song song;
		private Song songWithArtist;
		private Tag tag;

		private void AddSong(Song song) {

			querySource.Add(song);

			foreach (var name in song.Names)
				querySource.Add(name);

			foreach (var artistLink in song.AllArtists)
				querySource.Add(artistLink);

			foreach (var tagUsage in song.Tags.Usages)
				querySource.Add(tagUsage);

		}

		private PartialFindResult<Song> CallFind() {
			return search.Find(queryParams);
		}

		[TestInitialize]
		public void SetUp() {

			querySource = new QuerySourceList();

			artist = new Artist(TranslatedString.Create("Junk")) { Id = 257 };
			tag = new Tag("Electronic");

			song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) { Id = 121, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1) };
			song.Tags.Usages.Add(new SongTagUsage(song, tag));
			AddSong(song);

			songWithArtist = new Song(new LocalizedString("Crystal Tears", ContentLanguageSelection.English)) { Id = 7787, FavoritedTimes = 39, CreateDate = new DateTime(2012, 1, 1) };
			songWithArtist.AddArtist(artist);
			AddSong(songWithArtist);

			queryParams = new SongQueryParams();

			search = new Model.Service.Search.SongSearch.SongSearch(querySource, ContentLanguagePreference.Default);

		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		public void ListAll() {

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(song.DefaultName, result.Items[0].DefaultName);
			Assert.AreEqual(songWithArtist, result.Items[1]);

		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip() {

			queryParams.Paging.Start = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(songWithArtist, result.Items[0]);

		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName() {

			queryParams.SortRule = SongSortRule.Name;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortFavorites() {

			queryParams.SortRule = SongSortRule.FavoritedTimes;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortAdditionDate() {

			queryParams.SortRule = SongSortRule.AdditionDate;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("Nebula", result.Items[0].DefaultName);
			Assert.AreEqual("Crystal Tears", result.Items[1].DefaultName);

		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		public void QueryName() {

			queryParams.Common.Query = "Crystal Tears";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name as words.
		/// </summary>
		[TestMethod]
		public void QueryNameWords() {

			queryParams.Common.NameMatchMode = NameMatchMode.Words;
			queryParams.Common.Query = "Tears Crystal";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name, move exact result to top.
		/// 
		/// 2 songs match, Tears of Palm and Crystal Tears.
		/// Tears of Palm is later in the results when sorted by title, 
		/// but matches from the beginning so it should be moved to first.
		/// </summary>
		[TestMethod]
		public void QueryNameMoveExactToTop() {

			var song2 = new Song(new LocalizedString("Tears of Palm", ContentLanguageSelection.English)) {
				Id = 121, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
			};
			AddSong(song2);

			queryParams.Common.Query = "Tears";
			queryParams.Common.MoveExactToTop = true;
			queryParams.Paging.MaxEntries = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "2 total count");
			Assert.AreEqual(song2, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by tag
		/// </summary>
		[TestMethod]
		public void QueryTag() {

			queryParams.Common.Query = "tag:Electronic";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "1 total count");
			Assert.AreEqual(song, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType() {

			queryParams.SongTypes = new[] { SongType.Original };

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(song, result.Items[0]);

		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist() {

			queryParams.ArtistId = artist.Id;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(songWithArtist, result.Items[0], "songs are equal");

		}

		/// <summary>
		/// Query songs with only PVs.
		/// </summary>
		[TestMethod]
		public void QueryOnlyWithPVs() {

			queryParams.OnlyWithPVs = true;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(song, result.Items[0], "songs are equal");

		}

	}*/

}
