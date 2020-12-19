#nullable disable

using System;
using System.Diagnostics;
using System.Linq;
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
			Assert.IsTrue(result.Items.Any(s => s.Equals(expected)), $"Found {expected}");
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

			Assert.AreEqual(7, result.Items.Length, "Number of results");
			Assert.AreEqual(7, result.TotalCount, "Total result count");
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
			_queryParams.Paging.Start = 1;

			var result = CallFind();

			Assert.AreEqual(6, result.Items.Length, "Number of results");
			Assert.AreEqual(7, result.TotalCount, "Total result count");
			AssertHasSong(result, Db.Song);
		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortName()
		{
			_queryParams.SortRule = SongSortRule.Name;

			var result = CallFind();

			Assert.AreEqual(7, result.Items.Length, "Number of results");
			Assert.AreEqual(7, result.TotalCount, "Total result count");
			Assert.AreEqual("Azalea", result.Items[0].DefaultName);
			Assert.AreEqual("Crystal Tears", result.Items[1].DefaultName);
		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortFavorites()
		{
			_queryParams.SortRule = SongSortRule.FavoritedTimes;

			var result = CallFind();

			Assert.AreEqual(7, result.Items.Length, "Number of results");
			Assert.AreEqual(7, result.TotalCount, "Total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);
		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListSortAdditionDate()
		{
			_queryParams.SortRule = SongSortRule.AdditionDate;

			var result = CallFind();

			Assert.AreEqual(7, result.Items.Length, "Number of results");
			Assert.AreEqual(7, result.TotalCount, "Total result count");
			Assert.AreEqual("Nebula", result.Items[0].DefaultName);
			Assert.AreEqual("Tears of Palm", result.Items[1].DefaultName);
			Assert.AreEqual("Crystal Tears", result.Items[2].DefaultName);
		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNamePartial()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Tears", NameMatchMode.Partial);

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "total result count");
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
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Tears Crystal", NameMatchMode.Words);

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
		[TestCategory(TestCategories.Database)]
		public void QueryNameMoveExactToTop()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Tears");
			_queryParams.Common.MoveExactToTop = true;
			_queryParams.Paging.MaxEntries = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total number of results");
			Assert.AreEqual(Db.Song6, result.Items[0], "result is as expected");
		}

		/// <summary>
		/// Find song whose name contains SQL wildcards
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryNameWithSqlWildcards()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Nebula [Extend RMX]");
			_queryParams.Paging.MaxEntries = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "Number of results");
			Assert.AreEqual(1, result.TotalCount, "Total number of results");
			Assert.AreEqual(Db.SongWithSpecialChars, result.Items[0], "result is as expected");
		}

		/// <summary>
		/// Query by tag
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryTag()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("tag:Electronic");

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "1 total count");
			Assert.AreEqual(Db.Song, result.Items[0], "result is as expected");
		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryType()
		{
			_queryParams.SongTypes = new[] { SongType.Original };

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.Song, result.Items[0]);
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

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			AssertHasSong(result, Db.Song3);
			AssertHasSong(result, Db.Song4);
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryArtistAndName()
		{
			_queryParams.ArtistParticipation.ArtistIds = new[] { Db.Producer.Id };
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Azalea");

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "Number of results");
			Assert.AreEqual(1, result.TotalCount, "Total result count");
			Assert.AreEqual("Azalea", result.Items.First().DefaultName, "Song as expected");
		}

		/// <summary>
		/// Query songs with only PVs.
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryOnlyWithPVs()
		{
			_queryParams.OnlyWithPVs = true;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.Song, result.Items[0], "songs are equal");
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryLyrics_SingleLanguage()
		{
			_queryParams.AdvancedFilters = new[] { new AdvancedSearchFilter {
				FilterType = AdvancedFilterType.Lyrics,
				Param = OptionalCultureCode.LanguageCode_English
			} };

			var result = CallFind();

			Assert.AreEqual(1, result.TotalCount, "Total result count");
			AssertHasSong(result, Db.Song);
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void QueryLyrics_AnyLanguage()
		{
			_queryParams.AdvancedFilters = new[] { new AdvancedSearchFilter {
				FilterType = AdvancedFilterType.Lyrics,
				Param = AdvancedSearchFilter.Any
			} };

			var result = CallFind();

			Assert.AreEqual(2, result.TotalCount, "Total result count");
			AssertHasSong(result, Db.Song);
			AssertHasSong(result, Db.Song2);
		}
	}
}
