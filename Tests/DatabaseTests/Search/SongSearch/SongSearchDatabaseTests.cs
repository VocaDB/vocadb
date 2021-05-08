#nullable disable

using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.Search.SongSearch
{
	[TestClass]
	public class SongSearchDatabaseTests
	{
		private DatabaseTestContext<IDatabaseContext> _context;
		private EntryUrlParser _entryUrlParser;
		private SongQueryParams _queryParams;

		private TestDatabase Db => TestContainerManager.TestDatabase;

		private void AssertHasSong(PartialFindResult<Song> result, Song expected)
		{
			result.Items.Any(s => s.Equals(expected)).Should().BeTrue($"Found {expected}");
		}

		[TestInitialize]
		public void SetUp()
		{
			_queryParams = new SongQueryParams { SortRule = SongSortRule.Name };
			_entryUrlParser = new EntryUrlParser();
			_context = new DatabaseTestContext<IDatabaseContext>();
		}

		private PartialFindResult<Song> CallFind(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default)
		{
			return _context.RunTest(querySource =>
			{
				var search = new Model.Service.Search.SongSearch.SongSearch(querySource, languagePreference, _entryUrlParser);

				var watch = new Stopwatch();
				watch.Start();

				var result = search.Find(_queryParams);

				Console.WriteLine("Test finished in {0}ms", watch.ElapsedMilliseconds);

				return result;
			});
		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListAll()
		{
			var result = CallFind();

			result.Items.Length.Should().Be(7, "Number of results");
			result.TotalCount.Should().Be(7, "Total result count");
			AssertHasSong(result, Db.Song);
			AssertHasSong(result, Db.Song2);
			AssertHasSong(result, Db.Song3);
		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSkip()
		{
			_queryParams = _queryParams with { Paging = _queryParams.Paging with { Start = 1 } };

			var result = CallFind();

			result.Items.Length.Should().Be(6, "Number of results");
			result.TotalCount.Should().Be(7, "Total result count");
			AssertHasSong(result, Db.Song);
		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortName()
		{
			_queryParams = _queryParams with { SortRule = SongSortRule.Name };

			var result = CallFind();

			result.Items.Length.Should().Be(7, "Number of results");
			result.TotalCount.Should().Be(7, "Total result count");
			result.Items[0].DefaultName.Should().Be("Azalea");
			result.Items[1].DefaultName.Should().Be("Crystal Tears");
		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortFavorites()
		{
			_queryParams = _queryParams with { SortRule = SongSortRule.FavoritedTimes };

			var result = CallFind();

			result.Items.Length.Should().Be(7, "Number of results");
			result.TotalCount.Should().Be(7, "Total result count");
			result.Items[0].DefaultName.Should().Be("Crystal Tears");
			result.Items[1].DefaultName.Should().Be("Nebula");
		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortAdditionDate()
		{
			_queryParams = _queryParams with { SortRule = SongSortRule.AdditionDate };

			var result = CallFind();

			result.Items.Length.Should().Be(7, "Number of results");
			result.TotalCount.Should().Be(7, "Total result count");
			result.Items[0].DefaultName.Should().Be("Nebula");
			result.Items[1].DefaultName.Should().Be("Tears of Palm");
			result.Items[2].DefaultName.Should().Be("Crystal Tears");
		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNamePartial()
		{
			_queryParams = _queryParams with { Common = _queryParams.Common with { TextQuery = SearchTextQuery.Create("Tears", NameMatchMode.Partial) } };

			var result = CallFind();

			result.Items.Length.Should().Be(3, "Number of results");
			result.TotalCount.Should().Be(3, "total result count");
			AssertHasSong(result, Db.Song2);
			AssertHasSong(result, Db.Song3);
			AssertHasSong(result, Db.Song6);
		}

		/// <summary>
		/// Query by name as words.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNameWords()
		{
			_queryParams = _queryParams with { Common = _queryParams.Common with { TextQuery = SearchTextQuery.Create("Tears Crystal", NameMatchMode.Words) } };

			var result = CallFind();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "total result count");
			result.Items[0].DefaultName.Should().Be("Crystal Tears");
		}

		/// <summary>
		/// Query by name, move exact result to top.
		/// 
		/// 2 songs match, Tears of Palm and Crystal Tears.
		/// Tears of Palm is later in the results when sorted by title, 
		/// but matches from the beginning so it should be moved to first.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNameMoveExactToTop()
		{
			_queryParams = _queryParams with
			{
				Common = _queryParams.Common with
				{
					TextQuery = SearchTextQuery.Create("Tears"),
					MoveExactToTop = true,
				},
				Paging = _queryParams.Paging with { MaxEntries = 1 },
			};

			var result = CallFind();

			result.Items.Length.Should().Be(1, "Number of results");
			result.TotalCount.Should().Be(3, "Total number of results");
			result.Items[0].Should().Be(Db.Song6, "result is as expected");
		}

		/// <summary>
		/// Find song whose name contains SQL wildcards
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNameWithSqlWildcards()
		{
			_queryParams = _queryParams with
			{
				Common = _queryParams.Common with { TextQuery = SearchTextQuery.Create("Nebula [Extend RMX]") },
				Paging = _queryParams.Paging with { MaxEntries = 1 },
			};

			var result = CallFind();

			result.Items.Length.Should().Be(1, "Number of results");
			result.TotalCount.Should().Be(1, "Total number of results");
			result.Items[0].Should().Be(Db.SongWithSpecialChars, "result is as expected");
		}

		/// <summary>
		/// Query by tag
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryTag()
		{
			_queryParams = _queryParams with { Common = _queryParams.Common with { TextQuery = SearchTextQuery.Create("tag:Electronic") } };

			var result = CallFind();

			result.Items.Length.Should().Be(1, "1 result");
			result.TotalCount.Should().Be(1, "1 total count");
			result.Items[0].Should().Be(Db.Song, "result is as expected");
		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryType()
		{
			_queryParams = _queryParams with { SongTypes = new[] { SongType.Original } };

			var result = CallFind();

			result.Items.Length.Should().Be(2, "Number of results");
			result.TotalCount.Should().Be(2, "Total result count");
			result.Items[0].Should().Be(Db.Song);
		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryArtist()
		{
			_queryParams.ArtistParticipation.ArtistIds = new[] { Db.Producer.Id };

			var result = CallFind();

			result.Items.Length.Should().Be(2, "Number of results");
			result.TotalCount.Should().Be(2, "Total result count");
			AssertHasSong(result, Db.Song3);
			AssertHasSong(result, Db.Song4);
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryArtistAndName()
		{
			_queryParams.ArtistParticipation.ArtistIds = new[] { Db.Producer.Id };
			_queryParams = _queryParams with { Common = _queryParams.Common with { TextQuery = SearchTextQuery.Create("Azalea") } };

			var result = CallFind();

			result.Items.Length.Should().Be(1, "Number of results");
			result.TotalCount.Should().Be(1, "Total result count");
			result.Items.First().DefaultName.Should().Be("Azalea", "Song as expected");
		}

		/// <summary>
		/// Query songs with only PVs.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryOnlyWithPVs()
		{
			_queryParams = _queryParams with { OnlyWithPVs = true };

			var result = CallFind();

			result.Items.Length.Should().Be(2, "Number of results");
			result.TotalCount.Should().Be(2, "Total result count");
			result.Items[0].Should().Be(Db.Song, "songs are equal");
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryLyrics_SingleLanguage()
		{
			_queryParams = _queryParams with
			{
				AdvancedFilters = new[]
				{
					new AdvancedSearchFilter
					{
						FilterType = AdvancedFilterType.Lyrics,
						Param = OptionalCultureCode.LanguageCode_English,
					}
				}
			};

			var result = CallFind();

			result.TotalCount.Should().Be(1, "Total result count");
			AssertHasSong(result, Db.Song);
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryLyrics_AnyLanguage()
		{
			_queryParams = _queryParams with
			{
				AdvancedFilters = new[]
				{
					new AdvancedSearchFilter
					{
						FilterType = AdvancedFilterType.Lyrics,
						Param = AdvancedSearchFilter.Any
					}
				}
			};

			var result = CallFind();

			result.TotalCount.Should().Be(2, "Total result count");
			AssertHasSong(result, Db.Song);
			AssertHasSong(result, Db.Song2);
		}
	}
}
