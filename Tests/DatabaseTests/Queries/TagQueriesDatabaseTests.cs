using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.Queries {

	/// <summary>
	/// Database tests for <see cref="TagQueries"/>.
	/// </summary>
	[TestClass]
	public class TagQueriesDatabaseTests {

		private readonly DatabaseTestContext<ISessionFactory> context = new DatabaseTestContext<ISessionFactory>();
		private TestDatabase Db => TestContainerManager.TestDatabase;

		[TestMethod]
		public void Update_ReplaceName() {

			var contract = new TagForEditContract(Db.Tag, false, ContentLanguagePreference.English);
			contract.Names[0] = new LocalizedStringWithIdContract {
				Value = "electronic", Language = ContentLanguageSelection.Japanese
			};
			var permissionContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			var result = context.RunTest(sessionFactory => {

				var repository = new TagNHibernateRepository(sessionFactory, permissionContext);

				var queries = new TagQueries(repository, permissionContext, new FakeEntryLinkFactory(), new InMemoryImagePersister(),
					new FakeUserIconFactory());

				var updated = queries.Update(contract, null);

				return queries.GetTagForEdit(updated.Id);

			});

			Assert.AreEqual(1, result.Names.Length, "Number of names");
			var name = result.Names[0];
			Assert.AreEqual(ContentLanguageSelection.Japanese, name.Language, "Name language");
			Assert.AreEqual("electronic", name.Value, "Name value");
			Assert.AreNotEqual(0, name.Id, "Id was assigned");

		}

	}

}
