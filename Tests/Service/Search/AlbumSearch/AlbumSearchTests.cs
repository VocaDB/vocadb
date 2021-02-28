#nullable disable

using System;
using System.Linq;
using FluentAssertions;
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

			result.Items.Length.Should().Be(2, "2 results");
			result.TotalCount.Should().Be(2, "total result count");
			result.Items[0].DefaultName.Should().Be(_album.DefaultName);
			result.Items[1].Should().Be(_albumWithArtist);
		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip()
		{
			_queryParams.Paging.Start = 1;

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(2, "total result count");
			result.Items[0].Should().Be(_albumWithArtist);
		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName()
		{
			_queryParams.SortRule = AlbumSortRule.Name;

			var result = Find();

			result.Items.Length.Should().Be(2, "2 results");
			result.TotalCount.Should().Be(2, "total result count");
			result.Items[0].DefaultName.Should().Be("DIVINE");
			result.Items[1].DefaultName.Should().Be("Synthesis");
		}

		/// <summary>
		/// List with sort by rating.
		/// </summary>
		[TestMethod]
		public void ListSortRating()
		{
			_queryParams.SortRule = AlbumSortRule.RatingAverage;

			var result = Find();

			result.Items.Length.Should().Be(2, "2 results");
			result.TotalCount.Should().Be(2, "total result count");
			result.Items[0].DefaultName.Should().Be("DIVINE");
			result.Items[1].DefaultName.Should().Be("Synthesis");
		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortAdditionDate()
		{
			_queryParams.SortRule = AlbumSortRule.AdditionDate;

			var result = Find();

			result.Items.Length.Should().Be(2, "2 results");
			result.TotalCount.Should().Be(2, "total result count");
			result.Items[0].DefaultName.Should().Be("DIVINE");
			result.Items[1].DefaultName.Should().Be("Synthesis");
		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		public void QueryName()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("DIVINE");

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].DefaultName.Should().Be("DIVINE");
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

			result.TotalCount.Should().Be(2, "2 results total");
			result.Items.Length.Should().Be(1, "1 result");
			result.Items.First().Should().Be(_albumWithArtist, "result is expected album");
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

			result.TotalCount.Should().Be(1, "Total results");
			result.Items.Length.Should().Be(1, "1 result");
			result.Items.First().Should().Be(_albumWithArtist, "result is expected album");
		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType()
		{
			_queryParams.AlbumType = DiscType.Album;

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].Should().Be(_album);
		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist()
		{
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].Should().Be(_albumWithArtist, "albums are equal");
		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundMain()
		{
			_album.AddArtist(_artist, true, ArtistRoles.Default);
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };
			_queryParams.ArtistParticipation.Participation = ArtistAlbumParticipationStatus.OnlyMainAlbums;

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].Should().Be(_albumWithArtist, "albums are equal");
		}

		[TestMethod]
		public void QueryArtistParticipationStatus_FoundCollab()
		{
			_album.AddArtist(_artist, true, ArtistRoles.Default);
			_queryParams.ArtistParticipation.ArtistIds = new[] { _artist.Id };
			_queryParams.ArtistParticipation.Participation = ArtistAlbumParticipationStatus.OnlyCollaborations;

			var result = Find();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].Should().Be(_album, "albums are equal");
		}
	}
}
