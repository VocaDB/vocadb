using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.Tags
{
	/// <summary>
	/// Tests for <see cref="TagSearch"/>.
	/// </summary>
	[TestClass]
	public class TagSearchTests
	{
		private readonly FakeTagRepository repository = new FakeTagRepository();

		private PartialFindResult<Tag> CallFind(TagQueryParams queryParams, bool onlyMinimalFields)
		{
			return repository.HandleQuery(ctx => new TagSearch(ctx, ContentLanguagePreference.English).Find(queryParams, onlyMinimalFields));
		}

		[TestInitialize]
		public void SetUp()
		{
			repository.Save(
				CreateEntry.Tag("electronic"), CreateEntry.Tag("rock"), CreateEntry.Tag("alternative rock"), CreateEntry.Tag("techno"));
		}

		public void Find_ByName()
		{
			var result = CallFind(new TagQueryParams(new CommonSearchParams(SearchTextQuery.Create("rock"), false, false), new PagingProperties(0, 100, true))
			{
				SortRule = TagSortRule.Name
			}, false);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(2, result.Items.Length, "Number of items returned");
			Assert.AreEqual(2, result.TotalCount, "Total number of items");
			Assert.AreEqual("alternative rock", result.Items[0].DefaultName, "First tag name");
			Assert.AreEqual("rock", result.Items[1].DefaultName, "Second tag name");
		}

		[TestMethod]
		public void Find_MinimalFields()
		{
			var result = CallFind(new TagQueryParams(new CommonSearchParams(SearchTextQuery.Empty, false, false), new PagingProperties(0, 100, true))
			{
				SortRule = TagSortRule.Name
			}, true);

			Assert.AreEqual(4, result.Items.Length, "Number of items returned");
			Assert.AreEqual("alternative rock", result.Items[0].DefaultName, "First tag name");
		}
	}
}
