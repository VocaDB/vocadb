using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.AlbumSearch {

	/// <summary>
	/// Tests for <see cref="Model.Service.Search.AlbumSearch.AlbumSearch"/>.
	/// </summary>
	[TestClass]
	public class AlbumSearchTests {

		private Artist artist;
		private AlbumQueryParams queryParams;
		private QuerySourceList querySource;
		private Model.Service.Search.AlbumSearch.AlbumSearch search;
		private Album album;
		private Album albumWithArtist;

		private void AddAlbum(Album album) {

			querySource.Add(album);

			foreach (var name in album.Names)
				querySource.Add(name);

			foreach (var artistLink in album.AllArtists)
				querySource.Add(artistLink);

		}

		private void CreateName(Album album, string val, ContentLanguageSelection language) {

			var name = album.CreateName(val, language);
			querySource.Add(name);

		}

		private PartialFindResult<Album> Find() {
			return search.Find(queryParams);
		}

		[TestInitialize]
		public void SetUp() {

			querySource = new QuerySourceList();

			artist = new Artist(TranslatedString.Create("XenonP")) { Id = 64 };
			querySource.Add(artist);

			album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English)) { Id = 1, DiscType = DiscType.Album, CreateDate = new DateTime(2011, 1, 16) };
			AddAlbum(album);

			albumWithArtist = new Album(new LocalizedString("DIVINE", ContentLanguageSelection.English)) { Id = 1010, DiscType = DiscType.Unknown, RatingAverage = 4.5, CreateDate = new DateTime(2012, 1, 15) };
			albumWithArtist.AddArtist(artist);
			AddAlbum(albumWithArtist);

			queryParams = new AlbumQueryParams();

			search = new Model.Service.Search.AlbumSearch.AlbumSearch(querySource, ContentLanguagePreference.Default);

		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		public void ListAll() {

			var result = Find();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(album.DefaultName, result.Items[0].DefaultName);
			Assert.AreEqual(albumWithArtist, result.Items[1]);

		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip() {

			queryParams.Paging.Start = 1;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(albumWithArtist, result.Items[0]);

		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName() {

			queryParams.SortRule = AlbumSortRule.Name;

			var result = Find();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("DIVINE", result.Items[0].DefaultName);
			Assert.AreEqual("Synthesis", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by rating.
		/// </summary>
		[TestMethod]
		public void ListSortRating() {

			queryParams.SortRule = AlbumSortRule.RatingAverage;

			var result = Find();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("DIVINE", result.Items[0].DefaultName);
			Assert.AreEqual("Synthesis", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortAdditionDate() {

			queryParams.SortRule = AlbumSortRule.AdditionDate;

			var result = Find();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual("DIVINE", result.Items[0].DefaultName);
			Assert.AreEqual("Synthesis", result.Items[1].DefaultName);

		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		public void QueryName() {

			queryParams.Common.TextQuery = SearchTextQuery.Create("DIVINE");

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("DIVINE", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Bug discovered on 12.3.2013
		/// 
		/// When:
		/// - Name match mode is "Auto" or "Words".
		/// - Matched name is an alias (not primary name).
		/// - Number of results exceeds page size.
		/// - User navigates to the second page of results.
		/// 
		/// Expected:
		/// Search returns the second page of matched results.
		/// 
		/// Actual result:
		/// Search returns only primary name matches, entries matched by alias are missing.
		/// 
		/// Reason: 
		/// Primary name search doesn't support "Words" match mode and aliases aren't matched past the first page.
		/// </summary>
		[TestMethod]
		public void QueryNameWords_SkipFirstPage() {

			CreateName(album, "Synthesis Miku", ContentLanguageSelection.Unspecified);
			CreateName(albumWithArtist, "DIVINE Miku", ContentLanguageSelection.Unspecified);

			queryParams.Common.TextQuery = SearchTextQuery.Create("Miku Miku");
			queryParams.Paging.Start = 1;		// Skip the first result
			queryParams.Paging.MaxEntries = 1;

			var result = Find();

			Assert.AreEqual(2, result.TotalCount, "2 results total");
			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(albumWithArtist, result.Items.First(), "result is expected album");

		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType() {

			queryParams.AlbumType = DiscType.Album;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(album, result.Items[0]);

		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist() {

			queryParams.ArtistId = artist.Id;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(albumWithArtist, result.Items[0], "albums are equal");

		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundMain() {
			
			album.AddArtist(artist, true, ArtistRoles.Default);
			queryParams.ArtistId = artist.Id;
			queryParams.ArtistParticipationStatus = ArtistAlbumParticipationStatus.OnlyMainAlbums;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(albumWithArtist, result.Items[0], "albums are equal");

		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundCollab() {
			
			album.AddArtist(artist, true, ArtistRoles.Default);
			queryParams.ArtistId = artist.Id;
			queryParams.ArtistParticipationStatus = ArtistAlbumParticipationStatus.OnlyCollaborations;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(album, result.Items[0], "albums are equal");

		}

	}

}
