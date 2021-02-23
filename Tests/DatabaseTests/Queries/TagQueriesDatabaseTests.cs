#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries
{
	/// <summary>
	/// Database tests for <see cref="TagQueries"/>.
	/// </summary>
	[TestClass]
	public class TagQueriesDatabaseTests
	{
		private readonly DatabaseTestContext<ITagRepository> _context = new();
		private readonly FakePermissionContext _userContext;
		private TestDatabase Db => TestContainerManager.TestDatabase;

		public TagQueriesDatabaseTests()
		{
			_userContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		}

		private TagForApiContract Merge(int sourceId, int targetId)
		{
			var permissionContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			return _context.RunTest(repository =>
			{
				var queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), new InMemoryImagePersister(), new InMemoryImagePersister(),
					new FakeUserIconFactory(), new EnumTranslations(), new FakeObjectCache());

				queries.Merge(sourceId, targetId);

				var result = queries.LoadTag(targetId, t => new TagForApiContract(t, ContentLanguagePreference.English, TagOptionalFields.None));

				return result;
			});
		}

		private TagForEditContract Update(TagForEditContract contract)
		{
			var permissionContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			return _context.RunTest(repository =>
			{
				var queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), new InMemoryImagePersister(), new InMemoryImagePersister(),
					new FakeUserIconFactory(), new EnumTranslations(), new FakeObjectCache());

				var updated = queries.Update(contract, null);

				return queries.GetTagForEdit(updated.Id);
			});
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Merge_MoveUsages()
		{
			var target = Merge(Db.Tag.Id, Db.Tag2.Id);

			target.UsageCount.Should().Be(1, "UsageCount for the target tag");
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_ReplaceName()
		{
			var contract = new TagForEditContract(Db.Tag, false, _userContext);
			contract.Names[0] = new LocalizedStringWithIdContract
			{
				Value = "electronic",
				Language = ContentLanguageSelection.Japanese
			};

			var result = Update(contract);

			result.Names.Length.Should().Be(1, "Number of names");
			var name = result.Names[0];
			name.Language.Should().Be(ContentLanguageSelection.Japanese, "Name language");
			name.Value.Should().Be("electronic", "Name value");
			name.Id.Should().NotBe(0, "Id was assigned");
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_SwapNameTranslations()
		{
			var contract = new TagForEditContract(Db.Tag2, false, _userContext);
			contract.Names[0].Value = "ロック"; // Swap values
			contract.Names[1].Value = "rock";

			var result = Update(contract);

			result.Names.Length.Should().Be(2, "Number of names");
			var name = result.Names[0];
			name.Value.Should().Be("ロック", "Name value");
		}
	}
}
