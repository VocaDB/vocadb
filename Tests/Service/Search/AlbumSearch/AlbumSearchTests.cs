#nullable disable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.AlbumSearch
{
	/// <summary>
	/// Tests for <see cref="Model.Service.Search.AlbumSearch.AlbumSearch"/>.
	/// </summary>
	[TestClass]
	public class AlbumSearchTests
	{
		private Artist _artist;
		private AlbumQueryParams _queryParams;
		private QuerySourceList _querySource;
		private Model.Service.Search.AlbumSearch.AlbumSearch _search;
		private Album _album;
		private Album _albumWithArtist;

		private void AddAlbum(Album album)
		{
			_querySource.Add(album);

			foreach (var name in album.Names)
				_querySource.Add(name);

			foreach (var artistLink in album.AllArtists)
				_querySource.Add(artistLink);
		}

		private void CreateName(Album album, string val, ContentLanguageSelection language)
		{
			var name = album.CreateName(val, language);
			_querySource.Add(name);
		}

		private PartialFindResult<Album> Find()
		{
			return _search.Find(_queryParams);
		}

		[TestInitialize]
		public void SetUp()
		{
			_querySource = new QuerySourceList();

			_artist = new Artist(TranslatedString.Create("XenonP")) { Id = 64 };
			_querySource.Add(_artist);

			_album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English)) { Id = 1, DiscType = DiscType.Album, CreateDate = new DateTime(2011, 1, 16) };
			AddAlbum(_album);

			_albumWithArtist = new Album(new LocalizedString("DIVINE", ContentLanguageSelection.English)) { Id = 1010, DiscType = DiscType.Unknown, RatingAverage = 4.5, CreateDate = new DateTime(2012, 1, 15) };
			_albumWithArtist.AddArtist(_artist);
			AddAlbum(_albumWithArtist);

			_queryParams = new AlbumQueryParams();

			_search = new Model.Service.Search.AlbumSearch.AlbumSearch(_querySource, ContentLanguagePreference.Default);
		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		public void ListAll()
		{
			var result = Find();

			Assert.AreEqual(2, result.Items.Length, "2 results");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(_album.DefaultName, result.Items[0].DefaultName);
			Assert.AreEqual(_albumWithArtist, result.Items[1]);
		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip()
		{
			_queryParams.Paging.Start = 1;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "total result count");
			Assert.AreEqual(_albumWithArtist, result.Items[0]);
		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName()
		{
			_queryParams.SortRule = AlbumSortRule.Name;

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
		public void ListSortRating()
		{
			_queryParams.SortRule = AlbumSortRule.RatingAverage;

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
		public void ListSortAdditionDate()
		{
			_queryParams.SortRule = AlbumSortRule.AdditionDate;

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
		public void QueryName()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("DIVINE");

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
		public void QueryNameWords_SkipFirstPage()
		{
			CreateName(_album, "Synthesis Miku", ContentLanguageSelection.Unspecified);
			CreateName(_albumWithArtist, "DIVINE Miku", ContentLanguageSelection.Unspecified);

			_queryParams.Common.TextQuery = SearchTextQuery.Create("Miku Miku");
			_queryParams.Paging.Start = 1;       // Skip the first result
			_queryParams.Paging.MaxEntries = 1;

			var result = Find();

			Assert.AreEqual(2, result.TotalCount, "2 results total");
			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(_albumWithArtist, result.Items.First(), "result is expected album");
		}

		[TestMethod]
		public void QueryCatNum()
		{
			CreateName(_album, "Synthesis", ContentLanguageSelection.Unspecified);
			CreateName(_albumWithArtist, "DIVINE", ContentLanguageSelection.Unspecified);
			_album.OriginalRelease.CatNum = "KRHS-90035";
			_albumWithArtist.OriginalRelease.CatNum = "XMCD-1003";
			_querySource.Add(CreateEntry.Album(name: "Reverberations"));

			_queryParams.Common.TextQuery = SearchTextQuery.Create("XMCD-1003");

			var result = Find();

			Assert.AreEqual(1, result.TotalCount, "Total results");
			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(_albumWithArtist, result.Items.First(), "result is expected album");
		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType()
		{
			_queryParams.AlbumType = DiscType.Album;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(_album, result.Items[0]);
		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist()
		{
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(_albumWithArtist, result.Items[0], "albums are equal");
		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundMain()
		{
			_album.AddArtist(_artist, true, ArtistRoles.Default);
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };
			_queryParams.ArtistParticipation.Participation = ArtistAlbumParticipationStatus.OnlyMainAlbums;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(_albumWithArtist, result.Items[0], "albums are equal");
		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundCollab()
		{
			_album.AddArtist(_artist, true, ArtistRoles.Default);
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };
			_queryParams.ArtistParticipation.Participation = ArtistAlbumParticipationStatus.OnlyCollaborations;

			var result = Find();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual(_album, result.Items[0], "albums are equal");
		}
	}
}
