using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries {

	/// <summary>
	/// Database tests for <see cref="TagQueries"/>.
	/// </summary>
	[TestClass]
	public class TagQueriesDatabaseTests {

		private readonly DatabaseTestContext<ITagRepository> context = new DatabaseTestContext<ITagRepository>();
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private TagForApiContract Merge(int sourceId, int targetId) {

			var permissionContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			return context.RunTest(repository => {

				var queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), new InMemoryImagePersister(),
					new FakeUserIconFactory(), new EnumTranslations());

				queries.Merge(sourceId, targetId);

				var result = queries.GetTag(targetId, t => new TagForApiContract(t, ContentLanguagePreference.English, TagOptionalFields.None));

				return result;

			});

		}

		private TagForEditContract Update(TagForEditContract contract) {

			var permissionContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			return context.RunTest(repository => {

				var queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), new InMemoryImagePersister(),
					new FakeUserIconFactory(), new EnumTranslations());

				var updated = queries.Update(contract, null);

				return queries.GetTagForEdit(updated.Id);

			});

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Merge_MoveUsages() {

			var target = Merge(Db.Tag.Id, Db.Tag2.Id);

			Assert.AreEqual(1, target.UsageCount, "UsageCount for the target tag");

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_ReplaceName() {

			var contract = new TagForEditContract(Db.Tag, false, ContentLanguagePreference.English);
			contract.Names[0] = new LocalizedStringWithIdContract {
				Value = "electronic", Language = ContentLanguageSelection.Japanese
			};

			var result = Update(contract);

			Assert.AreEqual(1, result.Names.Length, "Number of names");
			var name = result.Names[0];
			Assert.AreEqual(ContentLanguageSelection.Japanese, name.Language, "Name language");
			Assert.AreEqual("electronic", name.Value, "Name value");
			Assert.AreNotEqual(0, name.Id, "Id was assigned");

		}
		
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_SwapNameTranslations() {

			var contract = new TagForEditContract(Db.Tag2, false, ContentLanguagePreference.English);
			contract.Names[0].Value = "ロック"; // Swap values
			contract.Names[1].Value = "rock";

			var result = Update(contract);

			Assert.AreEqual(2, result.Names.Length, "Number of names");
			var name = result.Names[0];
			Assert.AreEqual("ロック", name.Value, "Name value");

		}
	}

}
