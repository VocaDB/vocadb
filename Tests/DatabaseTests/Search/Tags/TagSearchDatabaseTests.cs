using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.Search.Tags {

	[TestClass]
	public class TagSearchDatabaseTests {

		private readonly DatabaseTestContext context = new DatabaseTestContext();
		private readonly TagQueryParams queryParams = new TagQueryParams {
			SortRule = TagSortRule.Name, Common = new CommonSearchParams(), Paging = new PagingProperties(0, 100, true)
		};
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private void AssertHasTag(PartialFindResult<Tag> result, Tag expected) {

			Assert.IsTrue(result.Items.Any(s => s.Equals(expected)), string.Format("Found {0}", expected));

		}

		private PartialFindResult<Tag> CallFind(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default,
			bool onlyMinimalFields = false) {

			return context.RunTest(querySource => {

				var search = new TagSearch(querySource.OfType<Tag>(), languagePreference);

				var watch = new Stopwatch();
				watch.Start();

				var result = search.Find(queryParams, onlyMinimalFields);

				Console.WriteLine("Test finished in {0}ms", watch.ElapsedMilliseconds);

				return result;

			});

		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListAll() {

			var result = CallFind();

			Assert.AreEqual(4, result.Items.Length, "Number of results");
			Assert.AreEqual(4, result.TotalCount, "Total result count");
			AssertHasTag(result, Db.Tag);
			AssertHasTag(result, Db.Tag2);
			AssertHasTag(result, Db.Tag3);

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListAll_MinimalFields() {

			var result = CallFind(onlyMinimalFields: true);

			Assert.AreEqual(4, result.Items.Length, "Number of results");
			Assert.AreEqual(4, result.TotalCount, "Total result count");

			var sampleTag = result.Items[0];
			Assert.IsNotNull(sampleTag.TranslatedName, "Translated name");
			Assert.AreEqual("alternative rock", sampleTag.TranslatedName.Default, "Sample tag default name");
			Assert.AreEqual(ContentLanguageSelection.English, sampleTag.TranslatedName.DefaultLanguage, "Sample tag default language");

		}

	}

}
